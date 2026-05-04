using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class PersoneAddress
    {
        [Key]
        
        public int RowId { set; get; }

        public Guid? AppUserId { set; get; }
        public virtual AgregatorUser AppUser { set; get; }
        //        public Guid RowGuid { set; get; }
        //        public Guid PersoneDocType { set; get; }//"type": "Иностранный паспорт",
        /*
         "country": "КИРГИЗИЯ",
        "postalCode": "000012",
        "state": "МОСКВА",
        "city": "МОСКВА",
        "district": "МОСКВА",
        "settlement": "МОСКВА",
        "street": "ул. Маршола Прошлякова",
        "house": "26",
        "building": "3",
        "construction": "7",
        "apartment":"415"
        */
        public PersoneAddressType typeName { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string settlement { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string building { get; set; }
        public string construction { get; set; }
        public string apartment { get; set; }
        public int? countryId { get; set; }
        public virtual Country country { get; set; }

    }
}
