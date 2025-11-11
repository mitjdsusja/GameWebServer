namespace WebServerProject.Models.Request
{

    public class GachaListRequest
    {
        
    }

    public class GachaDrawRequest
    {
        public int userId { get; set; }
        public string gachaCode { get; set; }
    }
}
