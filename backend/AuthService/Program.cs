var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "AuthService API",
            Description = "API для управления аутентификацией и авторизацией пользователей"
        });
    });

    builder.Services.AddSwaggerExamplesFromAssemblyOf<AuthService.Errors.ApiError409ConflictExample>();
}

DotnetEnv.Env.Load();

string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
string? dbName = Environment.GetEnvironmentVariable("DB_NAME");
string? dbUser = Environment.GetEnvironmentVariable("DB_USER");
string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string? dbSchema = Environment.GetEnvironmentVariable("DB_SCHEMA");
string? connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SearchPath={dbSchema}";

builder.Services.AddDbContext<AuthServiceContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
