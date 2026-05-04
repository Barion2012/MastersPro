using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    public class TwilioLog
    {
        public int RowId { set; get; }
        public string Sid { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }
    }
}
