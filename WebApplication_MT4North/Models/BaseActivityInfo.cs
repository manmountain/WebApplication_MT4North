using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{

    public enum ActivityPhase
    {
        CONCEPTUALIZATION,
        VALIDATION,
        DEVELOPMENT,
        LAUNCH
    }

    public partial class BaseActivityInfo
    {
        public BaseActivityInfo()
        {
            //Activities = new HashSet<Activity>();
        }

        [JsonPropertyName("baseactivityid")]
        public int BaseActivityInfoId { get; set; }

        [JsonPropertyName("themeid")]
        public int? ThemeId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        /*TODO: Enums
         CONCEPTUALIZATION = "Konceptualisering",
         VALIDATION = "Konceptvalidering",
         DEVELOPMENT = "Produktutveckling",
         LAUNCH = "Produktlansering"
        */
        [JsonPropertyName("phase")]
        public ActivityPhase Phase { get; set; }

        [JsonPropertyName("theme")]
        public virtual Theme Theme { get; set; }
        //public virtual ICollection<Activity> Activities { get; set; }
    }
}
