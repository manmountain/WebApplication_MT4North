using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class CustomActivityInfo
    {
        public int CustomInfoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ThemeId { get; set; }
        public string Phase { get; set; }
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }
        public virtual Theme Theme { get; set; }
    }
}
