using MainService;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// Регистрируем генератор Swagger только вне продакшена
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

DotNetEnv.Env.Load();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbSchema = Environment.GetEnvironmentVariable("DB_SCHEMA");
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SearchPath={dbSchema}";

builder.Services.AddDbContext<MainServiceContext>(options =>
    options.UseNpgsql(connectionString));
var app = builder.Build();

// если база данных не существует, она будет создана, 
// а если существует, то будут применены все миграции
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MainServiceContext>();
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

app.Run();


