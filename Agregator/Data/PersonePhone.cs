using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public class PersonePhone
    {
        public int RowId { set; get; }

        public Guid? AppUserId { set; get; }
        public virtual AgregatorUser AppUser { set; get; }
        public PersonePhoneType typeName { get; set; }

        public string number { set; get; }

    }
}
