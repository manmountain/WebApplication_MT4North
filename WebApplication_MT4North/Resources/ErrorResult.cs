using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication_MT4North.Controllers
{
    public class ErrorResult
    {

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }

    }
}
