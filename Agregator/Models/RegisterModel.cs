using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Models
{
    partial class MainLoginModel
    {
        public class RegisterModel
        {
	    [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
            [EmailAddress]
            [Display(Name = "Почта")]
            public string Email { get; set; }

            [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
            [StringLength(100, ErrorMessage = "{0} должно иметь не менее {2} и не более {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароль и подтверждение не совпадают.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
