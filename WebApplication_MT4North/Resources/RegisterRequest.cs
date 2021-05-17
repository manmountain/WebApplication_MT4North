using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Resources
{
    public class RegisterRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }
    }
}
