using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace Agregator.Data
{
    public class AgregatorUser : IdentityUser<Guid>
    {
        public override Guid Id { get => base.Id; set => base.Id = value; }
        public ICollection<AppUserRole> UserRoles { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Middlename { get; set; }
        public DateTime? Birthday { get; set; }
//        public string PlaceOfBirth { get; set; }
        public byte[] Photo { get; set; }
        public bool ProfileReady { get; set; }
        public bool IdentityReady { get; set; }
        public bool? Male { get; set; }
        //  public string Citizenship { get; set; }
        public int? CountryCitizenshipId { get; set; }
        public virtual Country CountryCitizenship { get; set; }
        public string INN { get; set; }
        [PersonalData]
        public string Account { get; set; }
        public virtual Bank Bank { get; set; }
        public Guid? BankId { get; set; }

        //----

        public virtual ICollection<BlobStorage> BlobsData { set; get; }

        public string ClientID { set; get; }
        public DateTime Created { set; get; }
        public DateTime? Updated { set; get; }

        public bool? agreePrivacy { get; set; }
        public bool? agreeRegFNS { get; set; }


    }
}