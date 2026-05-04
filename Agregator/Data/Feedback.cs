using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class Feedback
    {
        public int RowId { get; set; }
        public Guid UserId { get; set; }
        public virtual AgregatorUser User { get; set; }

        public int Vacancy { get; set; }
        public int Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual FeedBackStatus StatusNavigation { get; set; }
        public virtual Vacancy VacancyNavigation { get; set; }
    }
}
