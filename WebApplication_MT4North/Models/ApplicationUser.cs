using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication_MT4North.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<RegisteredUserProject> RegisteredUserProjects { get; set; }

    }
}
