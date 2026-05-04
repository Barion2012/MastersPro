using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class VacancyBuildObjectCt
    {
        public int Vacancy { get; set; }
        public int Object { get; set; }

        public virtual BuildObject ObjectNavigation { get; set; }
        public virtual Vacancy VacancyNavigation { get; set; }
    }
}
