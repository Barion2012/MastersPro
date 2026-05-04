using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    
        public class Role : IdentityRole<Guid>
        {
            public ICollection<AppUserRole> UserRoles { get; set; }
        }
    
}
