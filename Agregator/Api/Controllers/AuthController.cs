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
//using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Twilio.Rest.Api.V2010.Account;

namespace Agregator.Api.Controllers
{
    using Api.Settings;
    using Api.Resources;
    using Data;
    using System.Web;
    using Services;

    [Route("api/v2/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AgregatorUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JwtSettings _jwtSettings;

        private readonly TwilioVerifySettings _twiloSettings;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<AgregatorUser> _signInManager;
        private readonly ISignService _signService;


        public AuthController
            (
            IMapper mapper
            , UserManager<AgregatorUser> userManager
            , RoleManager<Role> roleManager
            , IOptionsSnapshot<JwtSettings> jwtSettings
            , IOptions<TwilioVerifySettings> twiloSettings
            , ILogger<AuthController> logger
            , SignInManager<AgregatorUser> signInManager
            , ISignService signService
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _twiloSettings = twiloSettings.Value;
            _logger = logger;
            _signInManager = signInManager;
            _signService = signService;
        }

        [HttpPost("registerbatch")]
        public async Task<IActionResult> RegistrBatch()
        {

            try
            {
                var bodyData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                JArray jsone = (JArray)JsonConvert.DeserializeObject(bodyData);

                foreach(var rec in jsone)
                {
                    //_logger.LogInformation(rec.ToString());
                    bool isnew = true;
                    var user =  await _userManager.FindByNameAsync(Convert.ToString(rec["Email"]));
                    if (user == default(AgregatorUser))
                        user = new AgregatorUser() { UserName = (rec["Email"] ?? rec["Phone"]).ToString() };
                    else
                        isnew = false;

                    user.Name = rec["firstName"].ToString();
                    user.Surname = rec["lastName"].ToString();
                    user.Middlename = rec["middleName"]?.ToString();
                    user.PhoneNumber = rec["Phone"]?.ToString();
                    user.Email = rec["Email"]?.ToString();
                    user.Male = true;
                    user.Birthday = rec["Birthday"]==null? null: Convert.ToDateTime(rec["Birthday"]);
                    user.CountryCitizenshipId = rec["Citizenship"]==null? null: (int?)Convert.ToInt32(rec["Citizenship"]);
                    user.BankId = Guid.Parse("4E876077-7800-4274-963B-D6F8BCD47894");
                    user.INN = rec["INN"]?.ToString();
                    user.Photo = Properties.Resources.photo_placeholder_man;
                    user.ProfileReady = true;
                    user.agreeRegFNS = true;
                    
                    if (isnew)
                        await _userManager.CreateAsync(user);
                    else
                        await _userManager.UpdateAsync(user);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new StatusCodeResult(500);
            }

            return Ok();

        }


        [AllowAnonymous]
        [HttpPost("loginphone")]
        public async Task<IActionResult> LoginWithPhone()
        {

            try
            {
                var bodyData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                JObject jsone = (JObject)JsonConvert.DeserializeObject(bodyData);
                var verification = await VerificationResource.CreateAsync(to: jsone["phone"].ToString()
                                , channel: "sms"

                                , pathServiceSid: _twiloSettings.VerificationServiceSID
                                , locale: "ru"
                                );
                

                if (verification.Status == "pending")
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }



            return new StatusCodeResult(500);
        }

        [HttpPost("signinwithphone")]
        [AllowAnonymous]
        public async Task<ActionResult> SignInWithPhone()
        {
            try
            {

                

                var bodyData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                JObject jsone = (JObject)JsonConvert.DeserializeObject(bodyData);

                var verification = await VerificationCheckResource.CreateAsync(to: jsone["phone"].ToString(),
                  code: jsone["code"].ToString(),
                  pathServiceSid: _twiloSettings.VerificationServiceSID
                  );
                //                _logger.LogWarning($"Code={code},VerificationStatus={verification.Status}");
                if (verification.Status == "approved")
                {

                    var phone = jsone["phone"].ToString();

                    var user = await _userManager.FindByNameAsync(phone);
                    if (user == default(AgregatorUser))
                        user = _userManager.Users.FirstOrDefault(f => f.PhoneNumber == phone);

                    if (!user.PhoneNumberConfirmed)
                    {
                        user.PhoneNumberConfirmed = true;
                        await _userManager.UpdateAsync(user);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: Convert.ToBoolean(jsone["rememberMe"]));
                    

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok();
        }

/*
        [HttpPost("regphone")]
        public async Task<IActionResult> RegPhone()
        {
            //            Console.WriteLine(User.Identity.Name);
            //            return new JsonResult(await Task.FromResult(_userManager.FindByEmailAsync(User.Identity.Name)));
            var phone = "+79152635032";
            var user = new AgregatorUser() { UserName = phone
                , Photo=Properties.Resources.photo_placeholder_man 
                , PhoneNumber = phone
            };
            await _userManager.CreateAsync(user);


            return Ok(user.Id);


        }
*/

        //        [HttpGet("info")]
        //        [HttpPost()]
        [Route("info")]
        public async Task<IActionResult> Info()
        {
            
            return new JsonResult(await Task.FromResult(_userManager.FindByEmailAsync(User.Identity.Name)));
        }

        [Route("name")]
        public async Task<IActionResult> Name()
        {
          //  return await _userManager.GetUserAsync(User);
            return Content((await _userManager.FindByEmailAsync(User.Identity.Name)).Name);
        }


        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(UserSignUpResource userSignUpResource)
        {
            var user = _mapper.Map<UserSignUpResource, AgregatorUser>(userSignUpResource);

            var userCreateResult = await _userManager.CreateAsync(user, userSignUpResource.Password);

            if (userCreateResult.Succeeded)
            {
                return Created(string.Empty, string.Empty);
            }

            return Problem(userCreateResult.Errors.First().Description, null, 500);
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn()
        {

            var r = new System.IO.StreamReader(HttpContext.Request.Body);
            var ps = HttpUtility.ParseQueryString(await r.ReadToEndAsync());

            UserLoginResource userLoginResource = new UserLoginResource
            {
                Email = ps["Email"] 
                , Password = ps["Password"]
            };
            

            
           Console.WriteLine("{0} {1}",userLoginResource?.Email, userLoginResource?.Password);
            
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);
            if (user is null)
            {
                return NotFound("User not found");
            }

            var userSigninResult = await _userManager.CheckPasswordAsync(user, userLoginResource.Password);

            if (userSigninResult)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine("OK.{0}", string.Join(";", roles));
                return Ok(GenerateJwt(user, roles));
            }
            Console.WriteLine("Email or password incorrect.");
            return BadRequest("Email or password incorrect.");
        }

        [HttpPost("Roles/{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            Console.WriteLine("CreateRole({0})", roleName);

            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new Role
            {
                Name = roleName
            };

            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        [HttpPost("UserToRole/{userEmail}/{roleName}")]
        public async Task<IActionResult> AddUserToRole(string userEmail, string roleName)
        {
        //    Console.WriteLine("userEmail={0},Count={1}", userEmail, _userManager.Users.Count());
/*
            AgregatorUser user = null;
            foreach(var u in _userManager.Users)
            {
                if (string.Compare(u.UserName, userEmail + "@yandex.ru")==0)
                {
                    user = u;
                    break;
                }
            }
            Console.WriteLine("{0} {1}", user.UserName, string.Compare(user.UserName, userEmail + "@yandex.ru"));
*/

            var user = _userManager.Users.SingleOrDefault(u => u.UserName.Contains(userEmail, StringComparison.OrdinalIgnoreCase));
//            Console.WriteLine("userName={0} roleName={1}",user.UserName, roleName);
            var result =  await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }


        private string GenerateJwt(AgregatorUser user, IList<string> roles)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpirationInDays));

            var token = new JwtSecurityToken
            (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
