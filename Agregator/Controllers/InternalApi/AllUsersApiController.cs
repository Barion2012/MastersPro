using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Security.Principal;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace Agregator.Controllers.InternalApi
{
    using Data;
    using System.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Models;


    [Authorize]
    [ApiController]
    [Route("api/internal/[controller]/[action]")]
    public class AllUsersApiController : Controller
    {
        private readonly UserManager<AgregatorUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<AllUsersApiController> _logger;

        public AllUsersApiController(
             UserManager<AgregatorUser> userManager
            , RoleManager<Role> roleManager
            , ILogger<AllUsersApiController> logger
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet] 
        public async Task<object> Users(DataSourceLoadOptions loadOptions)
        {
 
            var ds = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Select(f=> new AllUserModel {
                Id= f.Id
                ,ShortName=$"{(string.IsNullOrEmpty(f.Surname)? string.Empty:f.Surname)} {(string.IsNullOrEmpty(f.Name)?string.Empty:f.Name.Substring(0,1)+".")} {(string.IsNullOrEmpty(f.Middlename) ? string.Empty : f.Middlename.Substring(0, 1) + ".")}"
                ,EMail=f.Email
                ,UserRole=(f.UserRoles.FirstOrDefault()==default(AppUserRole))? "": f.UserRoles.First().Role.Name 
            });

            return await DataSourceLoader.LoadAsync<AllUserModel>(ds, loadOptions);
        }

        [HttpPost]
        public async Task<IActionResult> NewUser()
        {
            var q = HttpUtility.ParseQueryString(await (new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync()));

            var user = new AgregatorUser
            {
                UserName = "",
                Email = "",
                Photo = Properties.Resources.photo_placeholder_man
            };

            JsonConvert.PopulateObject(q["values"], user);
            user.UserName = user.Email;
            var res = await _userManager.CreateAsync(user, ";~K6g6W4wMPu");
            
            _logger.LogInformation($"{string.Join(";", res.Errors)}");

            if (res.Succeeded)
                return Ok(user);
            else
                return StatusCode(500);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder() 
        {
            var q = HttpUtility.ParseQueryString(await (new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync()));
           
            var user = await _userManager.FindByIdAsync(q["key"]);
            JsonConvert.PopulateObject(q["values"], user);
            await _userManager.UpdateAsync(user);
            
            return Ok(user);
        }

        [HttpDelete]
        public async Task DeleteOrder()
        {
            var q = HttpUtility.ParseQueryString(await (new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync()));
            var user = await _userManager.FindByIdAsync(q["key"]);
            await _userManager.DeleteAsync(user);
        }

    }

}
