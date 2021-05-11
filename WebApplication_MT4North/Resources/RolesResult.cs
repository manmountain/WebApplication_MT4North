using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Resources
{
    public class RolesResult
    {

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

    }
}
