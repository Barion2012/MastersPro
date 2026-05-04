using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class DevelopmentPlan
    {
        public DevelopmentPlan()
        {

        }

        public int RowId { get; set; }
        public int? ParentId { get; set; }
        public virtual DevelopmentPlan Parent { get; set; }


        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PlanDate { get; set; }
        public DateTime? FactDate { get; set; }

        public  Guid WhoResponsible { get; set; }
        public virtual AgregatorUser WhoResponsibleNavigation { get; set; }


        public Guid? Creator { get; set; }
        public DateTime? Created { get; set; }
        public Guid? Updater { get; set; }
        public DateTime? Updated { get; set; }
    }
}
