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
    public class ContractApiController : Controller
    {
        private readonly UserManager<AgregatorUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<ContractApiController> _logger;

#if DESIGN
        private ApplicationDbContext _dbContext;
        public ContractApiController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public ContractApiController   (
            ApplicationStoreContext dbContext
#endif
    	    , UserManager<AgregatorUser> userManager
            , RoleManager<Role> roleManager
            , ILogger<ContractApiController> logger
        )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

       [HttpGet] 
        public async Task<object> Get(DataSourceLoadOptions loadOptions)
        {
            return await DataSourceLoader.LoadAsync(_dbContext.Contracts, loadOptions);
        }

       [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var row = await _dbContext.Contracts.FirstAsync(f=>f.RowId==key);
            JsonConvert.PopulateObject(values, row);
            
            await _dbContext.SaveChangesAsync();

            return Ok(row);
        }


       [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            Console.WriteLine(await (new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync()));
            var row = new Contract() { RowId=0};
            JsonConvert.PopulateObject(values, row);
            _dbContext.Contracts.Add(row);
            //user.UserName = user.Email;
            //var res = await _userManager.CreateAsync(user, ";~K6g6W4wMPu");
            await _dbContext.SaveChangesAsync();
            return Ok(row);


            /*
            _logger.LogInformation($"{string.Join(";", res.Errors)}");

            if (res.Succeeded)
                return Ok(user);
            else
                return StatusCode(500);
            */
        }

        [HttpDelete]
        public async Task Delete(int key)
        {
            var row = await _dbContext.Contracts.SingleAsync(f => f.RowId == key);
            _dbContext.Remove(row);
            await _dbContext.SaveChangesAsync();
        }

    }

}
