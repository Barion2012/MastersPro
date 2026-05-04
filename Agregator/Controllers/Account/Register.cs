using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Agregator.Data;

namespace Agregator.Controllers
{
    using Models;
    
    partial class AccountController
    {

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPartial(MainLoginModel mainLogiModel, string ReturnUrl = null)
        {
            var Input = mainLogiModel.Register;
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AgregatorUser { UserName = mainLogiModel.Register.Email, Email = mainLogiModel.Register.Email,
                    Photo=Properties.Resources.photo_placeholder_man
                };
                var result = await _userManager.CreateAsync(user, mainLogiModel.Register.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User created a new account with password.{user.Id}");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync( Input.Email, "Подтверждение адреса электронной почты",
                        $"Для подтверджения адреса Вашей электронной почты и завершения регистрации, пожалуйста, перейдите по <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        ModelState.AddModelError("Conformation", "Для потверждения регистрации Вашего аккаунта "+ mainLogiModel.Register.Email + " проверьте электронную почту.");
//                        return RedirectToAction("RegisterConfirmation", new { email = mainLogiModel.Register.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: mainLogiModel.RememberMe);
                        return LocalRedirect(ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return PartialView();
        }


        [TempData]
        public string StatusMessage { get; set; }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, bool? rememberMe)
        {
            if (userId != null && code != null)
            {

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userId}'.");
                }

                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);
                StatusMessage = result.Succeeded ? "Благодарим за подтверждение вашей электронной почты." : "Ошибка подтверждения вашего адреса электронной почты.";

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: rememberMe??true );
                }




                //            logger.LogInformation($"{user.UserName} {user.Id} {user.Email}");

                //           using (HttpClient httpClient = new HttpClient())
                //         {
                //             httpClient.BaseAddress = new Uri("http://akrus.techstroy.su:9999");
                /*
                using (HttpContent httpContext = new StringContent(
                    JsonConvert.SerializeObject(new {
                    step= "Identity",id=user.Id,email=user.Email
                    })
                    ))
                {

                    var res = await httpClient.PostAsync("/localapi/Register", httpContext);

                    logger.LogInformation($"StatusCode={res.StatusCode}");

                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        user.IdentityReady = true;
                        await _userManager.UpdateAsync(user);
                        //                      await _signInManager.RefreshSignInAsync(user);
                    }
                }
                //            }
                */
            }
            
            return LocalRedirect("/");
            
        }
    }
}
