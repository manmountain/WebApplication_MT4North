using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Controllers
{
    public class UserResult
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("LastName")]
        public string LastName { get; set; }

        [JsonPropertyName("Gender")]
        public string Gender { get; set; }
    }
}
