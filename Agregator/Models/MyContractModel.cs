using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Agregator.Models
{
    using Agregator.Data;

    public class MyContractModel
    {
        public int RowId { get; set; }
        public string Num { get; set; }
        public string DateText { get; set; }
        public ContractStatus Status { get; set; }
        //        public int Type { get; set; }
        public string StatusText { get; set; }
        public string DocUri { get; set; }
        public string SignUri { get; set; }
        public string CreateText { get; set; }
        public DateTime Created{ get; set; }

        public Guid CustomerId { get; set; }

        public string Remark { get; set; }
    }

}



