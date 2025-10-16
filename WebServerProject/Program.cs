using Microsoft.EntityFrameworkCore;
using WebServerProject.Data;

var builder = WebApplication.CreateBuilder(args);

// 컨트롤러 추가
builder.Services.AddControllers();

var connectionString = "server=localhost;port=3306;database=gamedb;user=root;password=sungdls200o!";
// DbContext 등록
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


var app = builder.Build();
app.UseRouting();
app.MapControllers();

app.Run("http://0.0.0.0:5000"); // 모바일 기기에서 접근 가능하도록 모든 IP 허용