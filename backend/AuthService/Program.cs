using AuthService;
using AuthService.Redis;

using Microsoft.EntityFrameworkCore;

using StackExchange.Redis;
using AuthService.Lib.Utils;
using AuthService.Lib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using AuthService.Errors;
using Microsoft.AspNetCore.Mvc;
using AuthService.ResponseExample;
using Serilog;
using Serilog.Context;
using Microsoft.AspNetCore.Mvc.ModelBinding;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.ExampleFilters();
            options.OperationFilter<ApiErrorExampleOperationFilter>();
            options.OperationFilter<ResponseExampleOperationFilter>();

            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = "AuthService API",
                Description = "API для управления аутентификацией и авторизацией пользователей"
            });
        });

    builder.Services.AddSwaggerExamplesFromAssemblyOf<ApiError>();
}

DotNetEnv.Env.Load();

// Логгирирование
string env = builder.Environment.EnvironmentName;
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext() // для контекстных свойств
    .Enrich.WithProperty("Environment", env)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// слушаем только локальный интерфейс
// TODO: В релизе переделать на локал хост (сейчас так чтобы видеть свагер)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9000);
});

// Редис
string? dbRedisHost = Environment.GetEnvironmentVariable("DB_REDIS_HOST");
string? dbRedisPort = Environment.GetEnvironmentVariable("DB_REDIS_PORT");
string? dbRedisAbortConnect = Environment.GetEnvironmentVariable("DB_REDIS_ABORT_CONNECT");
string? redisConnectionString = $"{dbRedisHost}:{dbRedisPort},abortConnect={dbRedisAbortConnect}";

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<RedisRefreshTokenBlackList>();
builder.Services.AddScoped<RedisAccessTokenBlackList>();
builder.Services.AddScoped<RedisAccessTokenList>();

string? jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT_KEY environment variable is not set.");
}
builder.Services.AddSingleton(new TokenService(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudiences = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(jwtKey),
            ClockSkew = TimeSpan.Zero
        };
    });

// БД Auth
string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string? dbSchema = Environment.GetEnvironmentVariable("DB_SCHEMA");
string? connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SearchPath={dbSchema}";

builder.Services.AddDbContext<AuthServiceContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрируем контроллеры и OpenAPI
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
{
    // пустое тело
    if (context.ModelState.TryGetValue(string.Empty, out var bodyEntry) && bodyEntry.Errors.Count > 0)
    {
        return new BadRequestObjectResult(new ApiError
        {
            StatusCode = "0.1.1",
            Title = "Неверный запрос",
            Message = bodyEntry.Errors.First().ErrorMessage,
            Field = "BODY"
        });
    }

    // поля с ошибками
    List<string> fields = [];
    foreach (KeyValuePair<string, ModelStateEntry?> kvp in context.ModelState.Where(k => k.Value?.Errors.Count > 0))
    {
        string key = kvp.Key;
        if (!fields.Contains(key))
        {
            fields.Add(key);
        }
    }

    ApiError apiError = new()
    {
        StatusCode = "0.2.1",
        Title = "Неверный запрос",
        Message = $"Полe(я) {string.Join(", ", fields)} не могут быть пустыми",
        Field = string.Join(",", fields),
    };
    return new BadRequestObjectResult(apiError);

});

builder.Services.AddEndpointsApiExplorer();

// Регистрируем источник активности для OpenTelemetry(пока это всё через логи)
// TODO: добавить OpenTelemetry и экспортировать трейсинги в Jaeger, Zipkin или другой бэкенд для трейсинга
string serviceName = Environment.GetEnvironmentVariable("AUTHSERVICE_NAME") ?? "auth-service";
builder.Services.AddSingleton(Tracing.ActivitySource(serviceName));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:9000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
WebApplication app = builder.Build();
app.UseCors("AllowSpecificOrigin");
// Применение миграции подключение к бд
int retry = 0;
int maxRetry = 10;
while (true)
{
    try
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            AuthServiceContext db = scope.ServiceProvider.GetRequiredService<AuthServiceContext>();
            var pending = db.Database.GetPendingMigrations();
            if (pending != null && pending.Any())
            {
                Log.Information("Найдены ожидающие миграции: {Count}", pending.Count());
                db.Database.Migrate();
            }
            else
            {
                Log.Information("Миграций не обнаружено, пропускаем ApplyMigrations");
            }
        }
        break; // если миграция прошла успешно, выходим из цикла
    }
    catch (Exception ex)
    {
        retry++;
        Log.Error(ex, "Ошибка при миграции базы данных. Попытка {Retry}/{MaxRetry}", retry, maxRetry);
        if (retry >= maxRetry)
        {
            Log.Fatal("Превышено максимальное количество попыток миграции базы данных. Завершение работы.");
            throw;
        }
        Thread.Sleep(5000); // ждем 5 секунд перед следующей попыткой
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware: если пришёл заголовок traceparent — положим его в LogContext как IncomingTraceParent
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("traceparent", out var tp) && !string.IsNullOrWhiteSpace(tp))
    {
        using (LogContext.PushProperty("TraceParent", tp.ToString()))
        {
            await next();
        }
    }
    else
    {
        await next();
    }
});

// Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.MapControllers();

try
{
    Log.Information("Запуск веб‑хоста");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Хост завершился с ошибкой");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
