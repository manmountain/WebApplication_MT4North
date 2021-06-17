using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Note
    {
        [JsonPropertyName("noteid")]
        public int NoteId { get; set; }

        [JsonPropertyName("activityid")]
        public int? ActivityId { get; set; }

        [JsonPropertyName("userid")]
        public string UserId { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("activity")]
        public virtual Activity Activity { get; set; }

        [JsonPropertyName("user")]
        public virtual ApplicationUser User { get; set; }
    }
}
