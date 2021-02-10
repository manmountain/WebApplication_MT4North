using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class RegisteredUser
    {
        public RegisteredUser()
        {
            Notes = new HashSet<Note>();
            RegisteredUserProjects = new HashSet<RegisteredUserProject>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdress { get; set; }
        public string Sex { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<RegisteredUserProject> RegisteredUserProjects { get; set; }
    }
}
