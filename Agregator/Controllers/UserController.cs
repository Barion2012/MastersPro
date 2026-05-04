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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Agregator.Controllers
{
    using Data;
    using Models;
    using Services;

    [Authorize]

#if !DESIGN
    [RequireHttps]
#endif
    public partial class UserController : Controller
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly IFormatProvider _format;
        private readonly SignInManager<AgregatorUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

#if DESIGN
        private ApplicationDbContext _dbContext;
        public UserController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public UserController(
            ApplicationStoreContext dbContext
#endif
            , UserManager<AgregatorUser> userManager
            , SignInManager<AgregatorUser> signinManager
            , ILogger<AccountController> logger
            , IEmailSender emailSender
           
            , IFormatProvider format)
        {
            _userManager = userManager;
            _signInManager = signinManager;
            _format = format;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> MyContracts() //=> View(await Task.FromResult(new List<MyContractModel>()));
        {


            IList<MyContractModel> contracts = new List<MyContractModel>();

            try
            {
                var user = await _userManager.GetUserAsync(User);


                
                contracts = await _dbContext.Contracts.Join(_dbContext.Enums, c => (int)c.Status, e => e.RowId,
                    (c, e) => new MyContractModel
                    {
                        RowId = c.RowId
                        ,
                        Num = c.DocNum ?? c.RowId.ToString()
                        ,
                        Remark = c.Remark
                        ,
                        CreateText = c.Created.ToString("dd.MM.yyyy")
                        ,
                        DateText = c.SignDate.Value.ToString("dd.MM.yyyy")
                        ,
                        DocUri = c.DocUri
                        ,
                        SignUri = c.SignUri
                    ,
                        StatusText = e.name
                    ,
                        CustomerId = c.CustomerId.Value
                    ,
                        Status = c.Status
                    ,
                        Created = c.Created

                    })
                    .Where(
                    f => f.CustomerId == user.Id
                    && (
                        f.Status == ContractStatus.ToSign
                        || f.Status == ContractStatus.Active
                        || f.Status == ContractStatus.Finished
                        || f.Status == ContractStatus.ToPay
                        || f.Status == ContractStatus.Paid
                    )

                    ).OrderBy(f => f.Created).ToListAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return View(contracts);
        }

        public async Task<IActionResult> MyMoney() => await Task.FromResult( View(new MyMoneyModel { contract="Договор подряда № 101", sum=11200.50M , date=DateTime.Parse("2021-03-25") }));
        static readonly Guid BOT = Guid.Parse(Properties.Resources.BOT);

        public async Task<IActionResult> Chat()
        {

            var user = await _userManager.GetUserAsync(User);

            var data = _dbContext.Messages.Where(f => f.SendFromId == user.Id)
                    .Select(f => new ChatMessage { text = f.Text, who = Sender.Me, user = user.Name ?? user.UserName, time = f.Created })
                .Union(_dbContext.Messages.Include(f => f.SendTo).Where(f => f.SendToId == user.Id)
                    .Select(f => new ChatMessage { text = f.Text, who = f.SendFromId == BOT ? Sender.Robot : Sender.Manager, user = f.SendFrom.Name ?? f.SendFrom.UserName,time=f.Created }))
                .OrderBy(f=>f.time);

            return View(await data.ToListAsync());


        }

        [AllowAnonymous]
        [Route("applicant")]
        public async Task<IActionResult> Applicant()
        {

            var data = await _dbContext.Applicants.FirstOrDefaultAsync();
            return View(data);

        }

        public async Task<IActionResult> PersonalInfo(Guid? user_id)
        {
            if (!user_id.HasValue)
            {
                user_id = Guid.Parse(User.Claims.First(f => f.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            }

            await _dbContext.CheckRecipient(user_id.Value);
            var data = await _dbContext.RecipientsView.SingleOrDefaultAsync(f => f.appUserId==user_id.Value);

            return View(data);
        }

        private async Task<ProfileModel> LoadAsync(AgregatorUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = (await _userManager.GetPhoneNumberAsync(user));

            //var Username = userName;

            return new ProfileModel
            {
                UserName = userName,

                Name = user.Name,
                Surname = user.Surname,
                Middlename = user.Middlename,
                Birthday = user.Birthday,
                PhoneNumber = phoneNumber,
                Face = user.Photo,
                Male = user.Male.HasValue ? (user.Male.Value ? "on" : "off") : "on",
                Female = user.Male.HasValue ? (user.Male.Value ? "off" : "on") : "off",
                Citizenship = user.CountryCitizenshipId ?? (int)Countries.RUS, 
                Countries = _dbContext.Countries.Where(f => f.UseProfile == 1).ToList(),
                phoneNumberConfirmed = user.PhoneNumberConfirmed,

                agreeRegFNS = user.agreeRegFNS ?? false

                

            };
        }
         

        [HttpGet]
        public object GetUserMenu(DataSourceLoadOptions loadOptions)
        {
            return Content(JsonConvert.SerializeObject(DataSourceLoader.Load(MenuData.userMenu, loadOptions)), "application/json");
        }

        [HttpGet]
        [ResponseCache(Duration=1000)]
        public async Task<IActionResult> Avatar()
        {
            var user = await _userManager.GetUserAsync(User);

            string avatar = $@"<svg width=""64"" height=""64"" viewBox=""0 0 64 64"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">
<image x=""-7"" y=""3"" width=""60"" height=""60"" xlink:href = ""data:image/jpeg;base64,{Convert.ToBase64String(await PdfUtil.MakePrevImg(user.Photo))}"" />
<circle cx=""32"" cy=""32"" r=""30"" fill=""none"" stroke=""#F9E547"" stroke-width=""5"" />
<circle cx=""32"" cy=""32"" r=""43"" fill=""none"" stroke=""#FFFFFF"" stroke-width=""25"" />
<defs>
<linearGradient id=""paint0_linear"" x1=""9.06667"" y1=""7.46667"" x2=""52.2667"" y2=""60.2667"" gradientUnits=""userSpaceOnUse"" >
<stop stop-color=""#F9E547"" />
<stop offset=""0.254525"" stop-color=""#FFEE8F"" />
<stop offset=""0.496265"" stop-color=""#FED800"" />
<stop offset=""1"" stop-color=""#F9E547"" />
</linearGradient>
</defs>
</svg>";


            return Content(avatar, "image/svg+xml");
            
            
        }
        [HttpGet]
        public async Task<IActionResult> AvatarFace()
        {
            var user = await _userManager.GetUserAsync(User);
            return new FileStreamResult(new System.IO.MemoryStream(await PdfUtil.MakePrevImg(user.Photo)), "image/jpg");

        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Пользователя с идентификатором '{_userManager.GetUserId(User)}' нет.");
            }

           
            return View(await LoadAsync(user));
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileModel Input)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Пользователя с идентификатором '{ _userManager.GetUserId(User)}' нет.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model={ModelState.IsValid}");
                
                return View(await LoadAsync(user));
            }

            string phoneNumber = await _userManager.GetPhoneNumberAsync(user),
                inputPhoneNumber = $"+7{Input.PhoneNumber}";
          
            if (inputPhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, inputPhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Произошла непредвиденная ошибка при установке номера телефона для пользователя с идентификатором '{userId}'.");
                }
            }

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }
            if (Input.Surname != user.Surname)
            {
                user.Surname = Input.Surname;
            }
            if (Input.Middlename != user.Middlename)
            {
                user.Middlename = Input.Middlename;
            }
            if (Input.Birthday != user.Birthday)
            {
                user.Birthday = Input.Birthday;
            }
            /*
            if (Input.PlaceOfBirth != user.PlaceOfBirth)
            {
                user.PlaceOfBirth = Input.PlaceOfBirth;
            }
            */
            if (Input.Photo != null)
            {
                using (var binaryReader = new System.IO.BinaryReader(Input.Photo.OpenReadStream()))
                {
                    user.Photo = binaryReader.ReadBytes((int)Input.Photo.Length);
                }
            }

            if (Input.Male == "on" && (!user.Male.HasValue || !user.Male.Value))
            {
                user.Male = true;
            }
            if (Input.Female == "on" && (!user.Male.HasValue || user.Male.Value))
            {
                user.Male = false;
            }

            if (!user.Male.HasValue) user.Male = true;

            if (Input.Citizenship != user.CountryCitizenshipId)
            {
                user.CountryCitizenshipId = Input.Citizenship;
            }
            //      log.LogInformation($"Mall={user.Male??true} {Input.Male??"fdfdfds"}");

            if (Input.agreeRegFNS != user.agreeRegFNS)
            {
                user.agreeRegFNS = Input.agreeRegFNS;
            }



            if (!user.ProfileReady) user.ProfileReady = true;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);


            TempData["StatusMessage"] = "Ваш профиль обновлен, заранее благодаим за актуальные данные. Теперь Вы можете отправлять заявки на вакансии.";


            if (user.IdentityReady)
            {

                using (HttpContent httpContext = new StringContent(
                    JsonConvert.SerializeObject(
                    new
                    {
                        step = "Profile",
                        id = user.Id,
                        nam = user.Name,
                        fam = user.Surname,
                        snm = user.Middlename,
                        birthday = user.Birthday?.ToString("yyyy-MM-dd"),
                    //    birthPlace = user.PlaceOfBirth,
                        photo = user.Photo == null ? string.Empty : Convert.ToBase64String(user.Photo),
                        male = user.Male.Value ? 0 : 1,
                        citizenship = user.CountryCitizenshipId,
                        phoneNumber = user.PhoneNumberConfirmed ? user.PhoneNumber : "",
                        email = user.Email
                    })))
                {
                    /*
                    var res = await httpClient.PostAsync("/localapi/Register", httpContext);

                    //                _logger.LogInformation($"StatusCode={res.StatusCode}");

                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        StatusMessage = "Ваш профиль был успешно передан работодателю";

                        //user.ProfileReady = true;
                        //await _userManager.UpdateAsync(user);
                        //await _signInManager.RefreshSignInAsync(user);

                    }
                */
                }

            }
            return RedirectToAction();
        }
    }
}

