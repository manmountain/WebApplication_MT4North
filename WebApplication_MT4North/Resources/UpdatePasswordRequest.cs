using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Resources
{
    public class UpdatePasswordRequest
    {
        [Required]
        [JsonPropertyName("new_password")]
        public string NewPassword { get; set; }

        [Required]
        [JsonPropertyName("current_password")]
        public string CurrentPassword { get; set; }
    }
}
