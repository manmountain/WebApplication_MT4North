using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Project
    {
        public Project()
        {
            RegisteredUserProjects = new HashSet<RegisteredUserProject>();
        }

        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual InnovationModel InnovationModel { get; set; }
        public virtual ICollection<RegisteredUserProject> RegisteredUserProjects { get; set; }
    }
}
