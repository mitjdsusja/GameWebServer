namespace WebServerProject.CSR.Services.Auth
{
    public class RegisterResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int? userId { get; set; }
    }
}
