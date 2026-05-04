using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class Recipient
    {
        public int number { set; get; }
        public Guid? AppUserId { set; get; }
        public virtual AgregatorUser AppUser { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string middleName { set; get; }
        public DateTime? birthDate { set; get; }
        public string birthPlace { set; get; }
        public int? citizenship { set; get; }
        public string latinFirstName { set; get; }
        public string latinLastName { set; get; }
        public int? recipientId { set; get; }
        public Guid? correlationId { set; get; }

        public string accountStatus { set; get; }
        public string selfEmployedStatus { set; get; }
        public string accountNumber { set; get; }
        public string agreementNumber { set; get; }

        public DateTime created { set; get; }
        public DateTime? updated { set; get; }
    }
}
