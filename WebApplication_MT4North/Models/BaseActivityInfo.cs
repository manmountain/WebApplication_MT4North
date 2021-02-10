using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class BaseActivityInfo
    {
        public BaseActivityInfo()
        {
            Activities = new HashSet<Activity>();
        }

        public int BaseInfoId { get; set; }
        public int? ThemeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Phase { get; set; }

        public virtual Theme Theme { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
