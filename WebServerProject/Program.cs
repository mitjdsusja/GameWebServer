using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Reflection;
using WebServerProject.CSR.Contollers.Dev;
using WebServerProject.CSR.Repositories.Character;
using WebServerProject.CSR.Repositories.Deck;
using WebServerProject.CSR.Repositories.Enemy;
using WebServerProject.CSR.Repositories.Gacha;
using WebServerProject.CSR.Repositories.Stage;
using WebServerProject.CSR.Repositories.User;
using WebServerProject.CSR.Services;
using WebServerProject.CSR.Services.Auth;
using WebServerProject.CSR.Services.Character;
using WebServerProject.CSR.Services.Deck;
using WebServerProject.CSR.Services.Gacha;
using WebServerProject.CSR.Services.Stage;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

// Swagger/OpenAPI 등록 (자동 API 문서 생성)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // XML 주석을 Swagger에 포함시키기 (Controller 주석 자동 반영)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

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

builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IDeckRepository, DeckRepository>();

builder.Services.AddScoped<IStageService, StageService>();  
builder.Services.AddScoped<IStageRepository, StageRepository>();
builder.Services.AddScoped<IEnemyRepository, EnemyRepository>();
builder.Services.AddScoped<IBattleService, BattleService>();

// 2. SQLKata / QueryFactory DI 등록
builder.Services.AddScoped<QueryFactory>(sp =>
{
    // 내부에서 IConfiguration을 꺼내 쓰면 더 유연합니다.
    var configuration = sp.GetRequiredService<IConfiguration>();

    // appsettings.json이나 환경변수(DB_HOST, DB_USER 등)에서 값을 알아서 찾아옵니다.
    var dbHost = configuration["DB_HOST"] ?? "localhost";
    var dbUser = configuration["DB_USER"] ?? "root";
    var dbPass = configuration["DB_PASS"];

    // 문자열 보간법으로 연결 문자열 생성
    var connectionString = $"server={dbHost};port=3306;database=gamedb;user={dbUser};password={dbPass};Pooling=true;Max Pool Size=100;";

    var connection = new MySqlConnection(connectionString);
    var compiler = new MySqlCompiler();

    return new QueryFactory(connection, compiler);
});

var app = builder.Build();

// Swagger 미들웨어 추가
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebServerProject API v1");
        c.RoutePrefix = string.Empty; // 루트(/)에서 Swagger 바로 열기
    });
}

app.UseRouting();
app.MapControllers();

app.Run("http://0.0.0.0:80");
