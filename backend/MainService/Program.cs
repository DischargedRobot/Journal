
using Microsoft.EntityFrameworkCore;
using MainService;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MainServiceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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


