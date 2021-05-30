using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApplication_MT4North.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string CompanyName { get; set; }

        public string Country { get; set; }

        public string ProfilePicture { get; set; }

        public string UserType { get; set; }

        public string UserRole { get; set; }

        [JsonIgnore]
        public override string PasswordHash
        {
            get { return base.PasswordHash; }
            set { base.PasswordHash = value; }
        }

        [JsonIgnore]
        public override bool EmailConfirmed
        {
            get { return base.EmailConfirmed; }
            set { base.EmailConfirmed = value; }
        }

        [JsonIgnore]
        public override string SecurityStamp
        {
            get { return base.SecurityStamp; }
            set { base.SecurityStamp = value; }
        }

        [JsonIgnore]
        public override string ConcurrencyStamp
        {
            get { return base.ConcurrencyStamp; }
            set { base.ConcurrencyStamp = value; }
        }

        [JsonIgnore]
        public override string PhoneNumber
        {
            get { return base.PhoneNumber; }
            set { base.PhoneNumber = value; }
        }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed
        {
            get { return base.PhoneNumberConfirmed; }
            set { base.PhoneNumberConfirmed = value; }
        }

        [JsonIgnore]
        public override bool TwoFactorEnabled
        {
            get { return base.TwoFactorEnabled; }
            set { base.TwoFactorEnabled = value; }
        }

        [JsonIgnore]
        public override DateTimeOffset? LockoutEnd
        {
            get { return base.LockoutEnd; }
            set { base.LockoutEnd = value; }
        }

        [JsonIgnore]
        public override bool LockoutEnabled
        {
            get { return base.LockoutEnabled; }
            set { base.LockoutEnabled = value; }
        }

        [JsonIgnore]
        public override int AccessFailedCount
        {
            get { return base.AccessFailedCount; }
            set { base.AccessFailedCount = value; }
        }

    }
}
