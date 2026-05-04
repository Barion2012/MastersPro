using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Заполнение не является действительным адресом электронной почты.")]
        public string Email { get; set; }
    }
}
