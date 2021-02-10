using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Activity
    {
        public Activity()
        {
            Notes = new HashSet<Note>();
        }

        public int ActivityId { get; set; }
        public int? ThemeId { get; set; }
        public bool? IsBaseActivity { get; set; }
        public bool IsExcluded { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phase { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public string Resources { get; set; }

        public virtual Theme Theme { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
