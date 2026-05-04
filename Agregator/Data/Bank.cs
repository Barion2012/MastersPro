using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    public class Bank
    {
        public Guid Id { set; get; }
        public string Name { get; set; }
        public string City { get; set; }
        public string BIK { get; set; }
        public string CorAccount { get; set; }
    }
}
