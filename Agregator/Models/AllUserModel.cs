using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agregator.Models
{
    public class AllUserModel
    {
        public Guid Id { set; get; }
        public string ShortName { set; get; }
        public string EMail { set; get; }
        public string UserRole { set; get; }
    }
}
