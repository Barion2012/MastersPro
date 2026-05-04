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
using System.Security.Claims;
using System.Net;

namespace Agregator.Controllers
{
    using Data;
    using Models;
    using Services;


    [AllowAnonymous]
    partial class AccountController
    {
        [HttpGet]
        public IActionResult ExternalLogin()
        {
            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(MainLoginModel logModel, string ReturnUrl = null)
        {
            // Request a redirect to the external login provider.
            //Microsoft.AspNetCore.Mvc.Routing.UrlActionContext context = new Microsoft.AspNetCore.Mvc.Routing.UrlActionContext();

            //Url.Action()
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
//            _logger.LogWarning($"loginModeel rememeber me = {loginModel.RememberMe} {}" );

            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", values: new { ReturnUrl=ReturnUrl, RememberMe=logModel.RememberMe });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(logModel.ProviderName, redirectUrl);
            return new ChallengeResult(logModel.ProviderName, properties);
        }

     [TempData]
      string ErrorMessage { set; get; }

        public async Task<IActionResult> ExternalLoginCallback(string ReturnUrl = null, string remoteError = null, bool? RememberMe=null )
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", new { ReturnUrl = ReturnUrl });
            }

            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToAction("Login", new { ReturnUrl = ReturnUrl });
            }

          //  _logger.LogWarning($"remember??{RememberMe ?? false}");
            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider,loginInfo.ProviderKey, isPersistent: RememberMe??false);


            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", loginInfo.Principal.Identity.Name, loginInfo.LoginProvider);
                return LocalRedirect(ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa", new { ReturnUrl = ReturnUrl, RememberMe = RememberMe??true });
            }
            else
            {
                MainLoginModel model = new MainLoginModel
                {
                    ReturnUrl = ReturnUrl,
                    ProviderName = loginInfo.ProviderDisplayName,
                    externalLogin = new MainLoginModel.ExternalLoginModel()
                };



            // If the user does not have an account, then ask the user to create an account.
            //ViewBag.Email = loginInfo.Principal..Email;
                if (loginInfo.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    model.externalLogin.Email= loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                }
                return View("ExternalLogin",model);
                //return Ok();

                /*
                if (loginInfo.Principal.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email)
                    };
                }
                return Page();

                                 case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                 */
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLoginConfirmation(MainLoginModel mainLoginModel, string ReturnUrl = null)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToAction("Login", new { ReturnUrl = ReturnUrl });
            }

            foreach (var c in info.Principal.Claims)
            {
                _logger.LogInformation($"{c.Type} {c.Value}");
            }

            if (ModelState.IsValid)
            {
                var user = new AgregatorUser
                {
                    UserName = mainLoginModel.externalLogin.Email,
                    Email = mainLoginModel.externalLogin.Email
                    //      , Name = info.Principal.Identity.Name
                    ,
                    Photo = Properties.Resources.photo_placeholder_man

                };

                var claim = info.Principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.GivenName);
                if (claim != default(Claim))
                {
                    user.Name = claim.Value;
                }
                claim = info.Principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Surname);
                if (claim != default(Claim))
                {
                    user.Surname = claim.Value;
                }
                claim = info.Principal.Claims.FirstOrDefault(f => f.Type == "urn:mailru:profileimage");
                if (claim != default(Claim))
                {
                    using (WebClient cln = new WebClient())
                    {
                        user.Photo = await cln.DownloadDataTaskAsync(claim.Value);
                    }
                }
                mainLoginModel.ReturnUrl = ReturnUrl;
                mainLoginModel.ProviderName = info.ProviderDisplayName;

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {

                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action(
                            "ConfirmEmail","Account",
                            values: new { userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(mainLoginModel.externalLogin.Email, "Подтверждение адреса электронной почты",
                            $"Для подтверджения адреса Вашей электронной почты и завершения регистрации, пожалуйста, перейдите по <a href='{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            ModelState.AddModelError(string.Empty, "Отправлена ссылка на почту");

                            return View("ExternalLogin", mainLoginModel);
                            //return RedirectToAction("RegisterConfirmation", new { Email = mainLoginModel.externalLogin.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

      //      ViewBag.ProviderDisplayName = info.ProviderDisplayName;
        //    ViewBag.ReturnUrl = returnUrl;
       //     mainLoginModel.ReturnUrl= returnUrl;
       //     mainLoginModel.ProviderName = info.ProviderDisplayName;
            return View("ExternalLogin", mainLoginModel);
        }


    }
}

 