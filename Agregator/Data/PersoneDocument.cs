using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class PersoneDocument
    {
        
        public int RowId { set; get; }

        public Guid? AppUserId { set; get; }
        public virtual AgregatorUser AppUser { set; get; }

        [PersonalData]
        public PersoneDocumentType typeName { get; set; }
        [PersonalData]
        public string serial { get; set; }
        [PersonalData]
        public string number { get; set; }
        [PersonalData]
        public DateTime? date { get; set; }
        [PersonalData]
        public DateTime? expireDate { get; set; }
        [PersonalData]
        public string organization { get; set; }
        [PersonalData]
        public string division { get; set; }


    }
}
