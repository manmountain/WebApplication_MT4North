using System.Collections.Generic;
using System.Text.Json.Serialization;
//using WebApplication_MT4North.Services;

namespace WebApplication_MT4North.Controllers
{
    public class RegisterResult
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        /*[JsonPropertyName("role")]
        public string Role { get; set; }*/

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }
        public IList<string> Role { get; internal set; }
    }
}
