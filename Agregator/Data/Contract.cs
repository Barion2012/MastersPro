using System;
using System.Collections.Generic;

namespace Agregator.Data
{
    public partial class Contract
    {
        public Contract()
        {
            ContractAttachments = new HashSet<ContractAttachment>();
        }
        public int RowId { get; set; }
        public string DocNum { get; set; }
        public DateTime? DocDate { get; set; }

        public DateTime? SignDate { get; set; }
        public ContractStatus Status { get; set; }
        public int Type { get; set; }
        public virtual string RegistreInfo { get; set; }
        public string DocUri { get; set; }
        public string SignUri { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public virtual AgregatorUser Issuer { set; get; }
        public Guid? IssuerId { get; set; }
    //    public virtual Frecipient Recipient { set; get; }
        public virtual AgregatorUser Customer { set; get; }
        public Guid? CustomerId { get; set; }

        public string Remark { get; set; }

        public decimal Fee { get; set; }
        public decimal ToPay { get; set; }

        public DateTime? PaidDate { get; set; }
        public string FNS_uri { get; set; }
        public int? reestrId { get; set; }
        public Guid? payCorelationId { get; set; }
        public string signtwId { get; set; }
        public string inOperId { get; set; }
        public string outOperId { get; set; }

        public virtual ICollection<ContractAttachment> ContractAttachments { get; set; }
    }
}
