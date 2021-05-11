using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Resources
{
    public class StatusResult
    {
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

    }
}
