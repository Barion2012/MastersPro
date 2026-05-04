using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class FeedBackStatus
    {
        public FeedBackStatus()
        {
            Feedbacks = new HashSet<Feedback>();
        }

        public int RowId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
