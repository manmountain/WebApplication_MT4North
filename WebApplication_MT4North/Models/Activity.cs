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
        public bool IsExcluded { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public string Resources { get; set; }
        public int ProjectId { get; set; }
        public int? BaseInfoId { get; set; }
        //public int? CustomActivityInfoId { get; set; }
        public virtual BaseActivityInfo BaseInfo { get; set; }
        public virtual Project Project { get; set; }
        //public virtual CustomActivityInfo CustomActivityInfo { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }
}
