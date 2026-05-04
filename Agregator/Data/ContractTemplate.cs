using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class ContractTemplate
    {
        public ContractTemplate()
        {
        }
[System.ComponentModel.DataAnnotations.Key]
        public Guid RowId { get; set; }
        public byte[] Templ { get; set; }
        public byte[] Core { get; set; }

        public virtual string nameTempate { get; set; }
        public virtual string fullNameTempate { get; set; }
        public virtual string description { get; set; }

    }
}
