using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Theme
    {
        public Theme()
        {
            Activities = new HashSet<Activity>();
            BaseActivityInfos = new HashSet<BaseActivityInfo>();
            CustomActivityInfos = new HashSet<CustomActivityInfo>();
        }

        [JsonPropertyName("themeid")]
        public int ThemeId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("activities")]
        public virtual ICollection<Activity> Activities { get; set; }

        [JsonPropertyName("baseactivityinfos")]
        public virtual ICollection<BaseActivityInfo> BaseActivityInfos { get; set; }

        [JsonPropertyName("customactivityinfos")]
        public virtual ICollection<CustomActivityInfo> CustomActivityInfos { get; set; }
    }
}
