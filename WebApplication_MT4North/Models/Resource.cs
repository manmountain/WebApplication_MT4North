using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApplication_MT4North.Models
{
    public class Resource
    {
         public Resource()
        {
        }

        [JsonPropertyName("resourceid")]
        public int ResourceId { get; set; }

        [JsonPropertyName("activityid")]
        public int? ActivityId { get; set; }

        [JsonPropertyName("url")]
        public String? Url { get; set; } 

    }
}
