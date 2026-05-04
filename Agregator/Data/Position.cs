using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class Position
    {
        public Position()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public int RowId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual ICollection<Vacancy> Vacancies { get; set; }

        public bool RostScope { get; set; }
    }
}
