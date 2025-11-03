using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;
using WebServerProject.Repositories;
using WebServerProject.Services;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// MySQL 연결 설정
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");

var connectionString = $"server={dbHost};port=3306;database=gamedb;user={dbUser};password={dbPass}";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

// DbContext 등록
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

var app = builder.Build();
app.UseRouting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate(); // 마이그레이션 자동 적용
}

app.Run("http://0.0.0.0:5000");