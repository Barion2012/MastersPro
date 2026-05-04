using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Models
{
    partial class MainLoginModel
    {
        public class ExternalLoginModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
