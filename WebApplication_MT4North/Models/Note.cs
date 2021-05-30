using System;
using System.Collections.Generic;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class Note
    {
        public int NoteId { get; set; }
        public int? ActivityId { get; set; }
        public int? UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }

        public virtual Activity Activity { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
