namespace WebServerProject.Models.Request
{

    public class GachaListRequest
    {
        
    }

    public class GachaDrawRequest
    {
        public string gachaCode { get; set; }
        public int userId { get; set; }
    }
}
