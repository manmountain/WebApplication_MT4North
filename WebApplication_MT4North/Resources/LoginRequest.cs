using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
//using WebApplication_MT4North.Services;

namespace WebApplication_MT4North.Controllers
{
    public class LoginRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
