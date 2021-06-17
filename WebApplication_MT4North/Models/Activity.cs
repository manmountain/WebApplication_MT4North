using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Activity
    {
        public Activity()
        {
            Notes = new HashSet<Note>();
        }

        [JsonPropertyName("activityid")]
        public int ActivityId { get; set; }

        [JsonPropertyName("isexcluded")]
        public bool IsExcluded { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("startdata")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("finishdate")]
        public DateTime? FinishDate { get; set; }

        [JsonPropertyName("deadlinedate")]
        public DateTime? DeadlineDate { get; set; }

        [JsonPropertyName("resources")]
        public string Resources { get; set; }

        [JsonPropertyName("projectid")]
        public int ProjectId { get; set; }

        [JsonPropertyName("baseinfoid")]
        public int? BaseInfoId { get; set; }
        //public int? CustomActivityInfoId { get; set; }

        [JsonPropertyName("baseinfo")]
        public virtual BaseActivityInfo BaseInfo { get; set; }

        [JsonPropertyName("project")]
        public virtual Project Project { get; set; }
        //public virtual CustomActivityInfo CustomActivityInfo { get; set; }

        [JsonPropertyName("notes")]
        public virtual ICollection<Note> Notes { get; set; }
    }
}
