using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class UserProject
    {
        public int UserProjectId { get; set; }
        public string Role { get; set; }
        public string Rights { get; set; }
        
        public int ProjectId { get; set; }
        //public int UserId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
