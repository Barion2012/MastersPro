using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agregator.Models
{
    public class TwoFactorAuthenticationModel
    {
      


        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

    }
}