using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using WebServerProject.CSR.Repositories;
using WebServerProject.CSR.Services;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IGachaService, GachaService>();
builder.Services.AddScoped<IGachaRepository, GachaRepository>();
builder.Services.AddScoped<IGachaRandomizer, GachaRandomizer>();

// MySQL 연결 설정
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");

var connectionString = $"server={dbHost};port=3306;database=gamedb;user={dbUser};password={dbPass}";

// SQLKata 초기화
var connection = new MySqlConnection(connectionString);
var compiler = new MySqlCompiler();
var queryFactory = new QueryFactory(connection, compiler);

// DI 등록
builder.Services.AddSingleton<QueryFactory>(queryFactory);

var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run("http://0.0.0.0:5000");