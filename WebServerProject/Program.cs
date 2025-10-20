using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

var connectionString = "server=mysql-server;port=3306;database=gamedb;user=root;password=sungdls200o!";
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

// DbContext 등록
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);


var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run("http://0.0.0.0:5000"); // 모바일 기기에서 접근 가능하도록 모든 IP 허용