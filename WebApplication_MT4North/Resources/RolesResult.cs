using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Controllers
{
    public class RolesResult
    {

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

    }
}
