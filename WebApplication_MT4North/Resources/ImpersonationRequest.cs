using System.Text.Json.Serialization;
//using WebApplication_MT4North.Services;

namespace WebApplication_MT4North.Controllers
{
    public class ImpersonationRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
