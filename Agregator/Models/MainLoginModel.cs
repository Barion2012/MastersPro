using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace Agregator.Models
{
    public partial class MainLoginModel
    {
        public LoginModel loginModel { set; get; }
        public ExternalLoginModel externalLogin { set; get; }
        public RegisterModel Register{ set; get; }

        [Display(Name = "оставаться в системе")]
        public bool RememberMe { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ProviderName { set; get; }
        public string ReturnUrl { get; set; }



    }


}
