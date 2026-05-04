//    Compiling a query which loads related collections for more than one collection navigation either via 'Include' or through projection but no 'QuerySplittingBehavior' has been configured. By default Entity Framework will use 'QuerySplittingBehavior.SingleQuery' which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Kernel.Font;
using iText.IO.Font;
using System.Globalization;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;

namespace Agregator.Controllers
{
    using Data;
    using Models;
    using Services;

    [Authorize]

#if !DESIGN
    [RequireHttps]
#endif

    public partial class AccountController : Controller
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly IFormatProvider _format;
        private readonly SignInManager<AgregatorUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
#if DESIGN
        private ApplicationDbContext _dbContext;
        public AccountController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public AccountController(
            ApplicationStoreContext dbContext
#endif

            , UserManager<AgregatorUser> userManager
            , SignInManager<AgregatorUser> signinManager
            , ILogger<AccountController> logger
            , IEmailSender emailSender
	    , IFormatProvider format
            , UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signinManager;
            _format = format;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
        }
/*
        [Route("loginPhone")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> LoginPhone(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var model = new MainLoginModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/"),
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                RememberMe = true,
                loginModel = new MainLoginModel.LoginModel(),
                externalLogin = new MainLoginModel.ExternalLoginModel()

            };

            return View(model);
        }
*/
       

        [Route("login")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var model = new MainLoginModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/"),
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                RememberMe = true,
                loginModel = new MainLoginModel.LoginModel(),
                externalLogin = new MainLoginModel.ExternalLoginModel()

            };

            return View(model);
        }


        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(MainLoginModel model, string returnUrl = null)
        {


            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.loginModel.Email, model.loginModel.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Авторизация не выполнена.");
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginPartial(MainLoginModel model, string ReturnUrl = null)
        {


            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.loginModel.Email, model.loginModel.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Ok(ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return Ok(Url.Action("LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = model.RememberMe }));
                    //return RedirectToAction("LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Авторизация не выполнена.");
                }
            }

            return PartialView();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword() => await Task.FromResult(View());
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordConfirmation() => await Task.FromResult(View());
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code = null)
        { 
            if (code == null)
            {
                return BadRequest("Для сброса пароля необходимо указать код.");
            }
            else
            {
                return await Task.FromResult(View(new ResetPasswordModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                }));
            }
        }



        [HttpPost]
        [AllowAnonymous]
 //       [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel Input, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    //                    return RedirectToAction("ResetPasswordConfirmation");
                    return RedirectToAction("Login", new { ReturnUrl = returnUrl });
                }

                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Input.Code));
                var result = await _userManager.ResetPasswordAsync(user, code, Input.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user,false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    //return RedirectToAction("ForgotPasswordConfirmation");
                    return RedirectToAction("Login", new { ReturnUrl = returnUrl });
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ResetPassword",   "Account",
                    values: new {  code, ReturnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Сброс пароля",
                    $"Для сброса пароля перейдите по <a href='{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>.");

                //  foreach(var err in (await _userManager.ResetPasswordAsync(user, code, ")QnQh4ZpYKiY")).Errors)
                //      _logger.LogWarning($"res={err.Description + err.Code}");
                ViewBag.Email = model.Email;
                return RedirectToAction("ForgotPasswordConfirmation");
            }

         
            _logger.LogInformation($"model.Email={model.Email}");
            return View();
        }

	
        [Route("logout")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
             return LocalRedirect(returnUrl);
		
        }


    }
}

            //            _logger.LogWarning($"Password={model.Password} emale={model.Email} remember={model.RememberMe}");


            //          return View();

            /*
            if (!ModelState.IsValid)
            {
                TempData["loginModel"] = model;
                TempData["viewData"] = ViewData;
                //                return View(model);
                return RedirectToAction("logreg", new { ReturnUrl = returnUrl });
            }
*/
            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            //      if (SignInManager.UserManager.IsEmailConfirmed)
        /*    var player = await SignInManager.UserManager.FindByNameAsync(model.Player);
            if (player == null)
                player = await SignInManager.UserManager.FindByEmailAsync(model.Player);
            if (player == null) goto error;

            await SetSID(player.Id, model.sid);

            if (player.EmailConfirmed)
            {
                var result = await SignInManager.PasswordSignInAsync(player.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:

                        return RedirectToLocal(returnUrl);

                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    //                    case SignInStatus.
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        goto error;
                        //                    return View(model);
                }
            }
            else
            {
                return View("DisplayEmail");
            }


        error:
            ModelState.AddModelError("", "Invalid login attempt.");
            TempData["loginModel"] = model;
            TempData["viewData"] = ViewData;
            return RedirectToAction("logreg", new { ReturnUrl = returnUrl });
            //return View("logreg");

            */
