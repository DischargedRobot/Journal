using MainService;
using MainService.Errors;

using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Filters;

using Serilog;
using MainService.Lib;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(9010);
});

// Генерация по шаблону для всех контроллеров 
// на случай когда нету обязательно параметра в теле запроса
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            Dictionary<string, string[]> errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                );

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new ApiError
            {
                StatusCode = "0.2.1",
                Title = "Неверный запрос",
                Message = string.Join("; ", errors.SelectMany(e => e.Value)),
                Field = "BODY"
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
// Регистрируем источник активности для трейсинга
string serviceName = Environment.GetEnvironmentVariable("MAINSERVICE_NAME") ?? "main-service";
builder.Services.AddSingleton(Tracing.ActivitySource(serviceName));
// Регистрируем генератор Swagger только вне продакшена
// if (builder.Environment.IsDevelopment())
// {
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.ExampleFilters();
    // Регистрируем наш фильтр после ExampleFilters, чтобы он мог перезаписать
    // или добавить примеры для ответов, если генераторы примеров тоже их установили.
    options.OperationFilter<ApiErrorExampleOperationFilter>();
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "MainService API",
        Description = "API для управления журналом успеваемости студентов"
    });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<ApiError>();
// }

DotNetEnv.Env.Load();

string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string? dbSchema = Environment.GetEnvironmentVariable("DB_SCHEMA");
string? connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SearchPath={dbSchema}";

builder.Services.AddDbContext<MainServiceContext>(options =>
    options.UseNpgsql(connectionString));
WebApplication app = builder.Build();

// если база данных не существует, она будет создана, 
// а если существует, то будут применены все миграции
// Применение миграции подключение к бд
int retry = 0;
int maxRetry = 10;
while (true)
{
    try
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            MainServiceContext db = scope.ServiceProvider.GetRequiredService<MainServiceContext>();
            // сверяем наличие миграций и применяем их, если они есть
            IEnumerable<string> pending = db.Database.GetPendingMigrations();
            if (pending != null && pending.Any())
            {
                Log.Information("Найдены ожидающие миграции: {Count}", pending.Count());
                db.Database.Migrate();
            }
            else
            {
                Log.Information("Не применённых миграций не обнаружено, пропускаем ApplyMigrations");
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


// только при разработке включаем Swagger
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

// перенаправляем с http на https
app.UseHttpsRedirection();

app.MapControllers();

app.Run();


