using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Project
    {
        /*public Project()
        {
            Activities = new HashSet<Activity>();
            InnovationModels = new HashSet<InnovationModel>();
            RegisteredUserProjects = new HashSet<RegisteredUserProject>();
        }

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<InnovationModel> InnovationModels { get; set; }
        public virtual ICollection<RegisteredUserProject> RegisteredUserProjects { get; set; }*/

        [JsonPropertyName("projectid")]
        public int ProjectId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
