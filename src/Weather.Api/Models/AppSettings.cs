namespace Weather.Api.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
