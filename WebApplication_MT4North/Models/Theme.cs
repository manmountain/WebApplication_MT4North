using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Theme
    {
        public Theme()
        {
            Activities = new HashSet<Activity>();
            BaseActivityInfos = new HashSet<BaseActivityInfo>();
            CustomActivityInfos = new HashSet<CustomActivityInfo>();
        }

        public int ThemeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? InnovationModelId { get; set; }

        public virtual InnovationModel InnovationModel { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<BaseActivityInfo> BaseActivityInfos { get; set; }
        public virtual ICollection<CustomActivityInfo> CustomActivityInfos { get; set; }
    }
}
