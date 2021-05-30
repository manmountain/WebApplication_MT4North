using System;
using System.Collections.Generic;

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

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
