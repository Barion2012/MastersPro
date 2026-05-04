using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Agregator.Data
{
    partial class ApplicationStoreContext
    {
#if !DESIGN
        internal async Task CheckRecipient(Guid user_id) => await 

            Database.ExecuteSqlRawAsync("EXEC CheckRecipient @appUserId", 
                new SqlParameter("@appUserId", user_id) 
            );

        internal async Task RequestValidateCode(string json) => await

            Database.ExecuteSqlRawAsync("EXECUTE [dbo].[RequestValidateCode] @data",
               new SqlParameter("@data", json)
            );

        internal async Task CheckValidateCode(string json) => await

            Database.ExecuteSqlRawAsync("EXECUTE [dbo].[CheckValidateCode] @data",
                new SqlParameter("@data", json)
            );


#endif
    }
}
