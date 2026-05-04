using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    public class AppUserRole : IdentityUserRole<Guid>
    {
        public virtual AgregatorUser User { get; set; }
        public virtual Role Role { get; set; }
    }
}
