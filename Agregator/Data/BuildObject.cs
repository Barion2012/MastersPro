using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class BuildObject
    {
        public BuildObject()
        {
            VacancyBuildObjectCts = new HashSet<VacancyBuildObjectCt>();
        }

        public int RowId { get; set; }
        public int? ParentId { get; set; }
        public sbyte Status { get; set; }
        public sbyte AppId { get; set; }
        public string Name { get; set; }
        public string Map { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public virtual ICollection<VacancyBuildObjectCt> VacancyBuildObjectCts { get; set; }
    }
}
