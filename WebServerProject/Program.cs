using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Repositories;
using WebServerProject.Services;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<HomeService>();

builder.Services.AddScoped<PlayerRepository>();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");

var connectionString = $"server={dbHost};port=3306;database=gamedb;user={dbUser};password={dbPass}";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

// DbContext 등록
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);


var app = builder.Build();
app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.Migrate(); // 마이그레이션 자동 적용
}

app.Run("http://0.0.0.0:5000"); // 모바일 기기에서 접근 가능하도록 모든 IP 허용