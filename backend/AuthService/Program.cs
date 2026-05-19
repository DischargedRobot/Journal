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

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.ExampleFilters();
            options.OperationFilter<ApiErrorExampleOperationFilter>();
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

string? dbRedisHost = Environment.GetEnvironmentVariable("DB_REDIS_HOST");
string? dbRedisPort = Environment.GetEnvironmentVariable("DB_REDIS_PORT");
string? dbRedisAbortConnect = Environment.GetEnvironmentVariable("DB_REDIS_ABORT_CONNECT");
string? redisConnectionString = $"{dbRedisHost}:{dbRedisPort},abortConnect={dbRedisAbortConnect}";

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<RedisRefreshTokenBlackList>();
builder.Services.AddScoped<RedisAccessTokenBlackList>();

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Регистрируем источник активности для OpenTelemetry(пока это всё через логи)
// TODO: добавить OpenTelemetry и экспортировать трейсинги в Jaeger, Zipkin или другой бэкенд для трейсинга
string serviceName = Environment.GetEnvironmentVariable("AUTHSERVICE_NAME") ?? "auth-service";
builder.Services.AddSingleton(Tracing.ActivitySource(serviceName));

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    AuthServiceContext db = scope.ServiceProvider.GetRequiredService<AuthServiceContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.MapControllers();

app.Run();
