using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class Work
    {
        public Work()
        {
            ContractAttachments = new HashSet<ContractAttachment>();
        }

        public int RowId { get; set; }
        public sbyte Status { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int? ParentId { get; set; }

        public Guid? work_id { get; set; }

        public virtual ICollection<ContractAttachment> ContractAttachments { get; set; }
    }

}

