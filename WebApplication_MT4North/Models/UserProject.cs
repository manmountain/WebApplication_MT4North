using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public enum UserProjectStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public partial class UserProject
    {
        [JsonPropertyName("userprojectid")]
        public int UserProjectId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("rights")]
        public string Rights { get; set; }

        [JsonPropertyName("projectid")]
        public int ProjectId { get; set; }

        [JsonPropertyName("userid")]
        public string UserId { get; set; }

        [JsonPropertyName("status")]
        public UserProjectStatus Status { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
