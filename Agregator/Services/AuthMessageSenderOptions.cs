using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agregator.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }

    public class TwilioVerifySettings
    {
        public string VerificationServiceSID { get; set; }
    }
}
