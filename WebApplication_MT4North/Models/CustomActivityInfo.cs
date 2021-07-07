using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{

    public partial class CustomActivityInfo
    {
        [JsonPropertyName("customactivityinfoid")]
        public int CustomActivityInfoId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("themeid")]
        public int? ThemeId { get; set; }

        [JsonPropertyName("phase")]
        public ActivityPhase Phase { get; set; }

        [JsonPropertyName("theme")]
        public virtual Theme Theme { get; set; }
    }
}
