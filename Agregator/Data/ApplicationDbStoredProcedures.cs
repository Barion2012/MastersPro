using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Agregator.Data
{
    partial class ApplicationDbContext
    {
#if DESIGN
        internal async Task CheckRecipient(Guid user_id) => await Task.FromResult(user_id);
        internal async Task RequestValidateCode(string json) =>  await Task.FromResult(json);
        internal async Task CheckValidateCode(string json) => await Task.FromResult(json);
#endif
    }
}
