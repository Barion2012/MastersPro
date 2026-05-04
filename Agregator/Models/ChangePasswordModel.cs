using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceName = nameof(Properties.Resources.FieldIsRequired),
            ErrorMessageResourceType = typeof(Properties.Resources))]
        [Display(Name = "Текущий пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = nameof(Properties.Resources.FieldIsRequired),
            ErrorMessageResourceType = typeof(Properties.Resources))]
        [Display(Name="Новый пароль")]
        [StringLength(100, ErrorMessage = "{0} должен иметь не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароль и подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}