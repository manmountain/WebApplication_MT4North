using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public enum ActivityStatus
    {
        STATUS_NOTSTARTED,
        STATUS_ONGOING,
        STATUS_FINISHED
    }

    public partial class Activity
    {
        public Activity()
        {
            Notes = new HashSet<Note>();
            Resources = new HashSet<Resource>();
        }

        [JsonPropertyName("activityid")]
        public int ActivityId { get; set; }

        [JsonPropertyName("isexcluded")]
        public bool IsExcluded { get; set; }

        [JsonPropertyName("status")]
        public ActivityStatus Status { get; set; }

        [JsonPropertyName("startdate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("finishdate")]
        public DateTime? FinishDate { get; set; }

        [JsonPropertyName("deadlinedate")]
        public DateTime? DeadlineDate { get; set; }

        [JsonPropertyName("resources")]
        public virtual ICollection<Resource> Resources { get; set; }

        [JsonPropertyName("projectid")]
        public int ProjectId { get; set; }

        [JsonPropertyName("baseactivityinfoid")]
        public int? BaseActivityInfoId { get; set; }

        [JsonPropertyName("customactivityinfoid")]
        public int? CustomActivityInfoId { get; set; }

        [JsonPropertyName("baseactivityinfo")]
        public virtual BaseActivityInfo BaseActivityInfo { get; set; }

        [JsonPropertyName("customactivityinfo")]
        public virtual CustomActivityInfo CustomActivityInfo { get; set; }

        [JsonPropertyName("project")]
        public virtual Project Project { get; set; }
        
        [JsonPropertyName("notes")]
        public virtual ICollection<Note> Notes { get; set; }
    }
}
