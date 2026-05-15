using MainService;
using MainService.Errors;

using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


// Генерация по шаблону для всех контроллеров 
// на случай когда нету обязательно параметра в теле запроса
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
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
// Регистрируем генератор Swagger только вне продакшена
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.ExampleFilters();
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "MainService API",
            Description = "API для управления журналом успеваемости студентов"
        });
    });
    builder.Services.AddSwaggerExamplesFromAssemblyOf<ApiError>();
}

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
using (IServiceScope scope = app.Services.CreateScope())
{
    MainServiceContext db = scope.ServiceProvider.GetRequiredService<MainServiceContext>();
    db.Database.Migrate();
}

// только при разработке включаем Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// перенаправляем с http на https
app.UseHttpsRedirection();

app.MapControllers();

app.Run();


