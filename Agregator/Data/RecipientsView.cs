using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class Frecipient
    {
        [Key]
        public int number { set; get; }
        public Guid appUserId { set; get; }
        public bool? Male { set; get; }
        public string INN { set; get; }
        public string Account { set; get; }
        public Guid? BankId { set; get; }
        public string Email { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public string middleName { set; get; }
        public DateTime? birthDate { set; get; }
        
        public string birthPlace { set; get; }
        public int? citizenship { set; get; }
        public string latinFirstName { set; get; }
        public string latinLastName { set; get; }
        public int? recipientId { set; get; }
        public string PhoneNumber { set; get; }
        public string docSerial { set; get; }
        public string docNumber { set; get; }
        public DateTime? docDate { set; get; }
        public string organization { set; get; }
        public string division { set; get; }
        public string mgMumber { set; get; }
        public string mgSerial { set; get; }
        public DateTime? expireDate { set; get; }
        public DateTime? mgDate { set; get; }
        public DateTime? mgExpireDate { set; get; }
        public string mgOrganization { set; get; }
        public string mgDivision { set; get; }
        public string postalCode { set; get; }
        public string state { set; get; }
        public string city { set; get; }
        public string district { set; get; }
        public string settlement { set; get; }
        public string street { set; get; }
        public string house { set; get; }
        public string building { set; get; }
        public string construction { set; get; }
        public string apartment { set; get; }
        public int? countryId { set; get; }
        public string regPostalCode { set; get; }
        public virtual string regAddress { get; set; }
        public string regState { set; get; }
        public string regCity { set; get; }
        public string regDistrict { set; get; }
        public string regSettlement { set; get; }
        public string regStreet { set; get; }
        public string regHouse { set; get; }
        public string regBuilding { set; get; }
        public string regConstruption { set; get; }
        public string regApartment { set; get; }
        public int?  regCountryId { set; get; }

        
  //      public virtual string numDog { set; get; }
  //      public virtual DateTime dateDog { set; get; }
        public virtual string fioName { set; get; }
        //var name = recipient.firstName?
        //public virtual Bank bank { set; get; }
        //public virtual string Contractor_FullName { set; get; }
        public virtual string docTypeName { set; get; }
        

    }
}
