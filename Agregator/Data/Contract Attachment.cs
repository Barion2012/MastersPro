 using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class ContractAttachment
    {

        public int RowId { get; set; }
        public int ContractId { get; set; }
        public short RowNum { get; set; }
        public int WorkId { get; set; }
        public decimal? Quant { get; set; }
        public decimal? Price { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public Guid? UserId { get; set; }

        public string Unit { get; set; }
        public string Path { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Work Work { get; set; }


        public Guid object_id { set; get; }
        public string object_name { set; get; }
        public int floor { set; get; }
        public int section { set; get; }
        public int room { set; get; }

        public int building { set; get; }
        public string app_type { set; get; }
        public string path_string { set; get; }
    }
}
