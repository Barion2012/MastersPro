using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class Applicant
    {
        public int RowId { set; get; }
        //public Guid? AppUserId { set; get; }
        //public virtual AgregatorUser AppUser { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string MiddleName { set; get; }
        public DateTime? BirthDate { set; get; }
        public string BirthPlace { set; get; }
        public bool? Male { set; get; }
        public int? CitizenshipId { set; get; }
        public virtual Country Citizenship { set; get; }

        public string ActualPlace { set; get; }
        public string Education { set; get; }
        public string MarriedStatus { set; get; }
        public string DeviceNumber { set; get; }
        public int? ProfessionId { set; get; }
        public virtual Position Profession { set; get; }
        public string Skils { set; get; }
        public string Tools { set; get; }
        public bool? Hostel { set; get; }
        public bool? Agreement { set; get; }
        public string Сonfirmation { set; get; }
        
        public DateTime Created { set; get; }
        public string Creator { set; get; }
    }
}
