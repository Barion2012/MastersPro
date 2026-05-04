using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Models
{
    partial class MainLoginModel
    {
        public class LoginModel
        {
	        [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
            [EmailAddress]
            [Display(Name = "Почта")]
            public string Email { get; set; }

            [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
	        [Display(Name = "Пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            /*
         //   [Required(ErrorMessage = RussianIdentityErrorDescriber.DefaultErrorMessage)]
            [Phone]
            [Display(Name = "Телефон")]
            public string Phone { get; set; }
            public string Code { get; set; }
            */

        }
    }
}
