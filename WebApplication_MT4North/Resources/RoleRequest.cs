using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Resources
{
    public class RoleRequest
    {
        [Required]
        [JsonPropertyName("rolename")]
        public string RoleName { get; set; }

    }
}
