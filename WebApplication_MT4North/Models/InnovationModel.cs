using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class InnovationModel
    {
        public InnovationModel()
        {
            Themes = new HashSet<Theme>();
        }

        public int InnovationModelId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<Theme> Themes { get; set; }
    }
}
