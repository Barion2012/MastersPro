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

namespace Agregator.Controllers
{
    using Data;
    using Services;

    [Authorize]
    public class ManagerController : Controller
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly IFormatProvider _appFormats;
        private readonly PdfUtil _pdfUtil;
        private readonly ILogger<ManagerController> _logger;



#if DESIGN
        private ApplicationDbContext _dbContext;
        public ManagerController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public ManagerController(
            ApplicationStoreContext dbContext
#endif
            , UserManager<AgregatorUser> userManager
            , IFormatProvider appFormats
            , PdfUtil pdfUtil
            , ILogger<ManagerController> logger
        )
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _appFormats = appFormats;
            _pdfUtil = pdfUtil;
            _logger = logger;
        }

        public async Task<object> GetContracts(DataSourceLoadOptions loadOptions) => await DataSourceLoader.LoadAsync(_dbContext.Contracts, loadOptions);
        public IActionResult ContractList() => View();
        public async Task<object> GetBanks(DataSourceLoadOptions loadOptions) => await DataSourceLoader.LoadAsync(_dbContext.Banks, loadOptions);
        public async Task<IActionResult> CustomerInfo() => View(await _userManager.GetUserAsync(User));
         public async Task<object> GetFeedbacks(DataSourceLoadOptions loadOptions) => await DataSourceLoader.LoadAsync(_dbContext.Feedbacks, loadOptions);
        public async Task<object> GetFeedbackStatus(DataSourceLoadOptions loadOptions) => await DataSourceLoader.LoadAsync(_dbContext.FeedBackStatus, loadOptions);
        public async Task<object> GetVacancies(DataSourceLoadOptions loadOptions) => await DataSourceLoader.LoadAsync(_dbContext.Vacancies, loadOptions);
        public IActionResult VacanciesList() => View();

        [HttpGet("rec/{uid}")]
        public IActionResult EditRecipent(Guid? uid)
        {
            return RedirectToAction("PersonalInfo", "User", new { user_id = uid.Value });
        }



        public IActionResult FeedbackList() => View();
        public IActionResult View2() => View();
        [HttpPost]
        public async Task<IActionResult> FeedbackList(AgregatorUser user)
        {
            //  Console.WriteLine(user.Id.ToString());
            var saveUser = await _userManager.FindByIdAsync(user.Id.ToString());
            //JsonConvert.PopulateObject( JsonConvert.SerializeObject(user), saveUser);
            /*
            saveUser.Zip = user.Zip;
            saveUser.Address = user.Address;
            saveUser.DocSerie = user.DocSerie;
            saveUser.DocNum = user.DocNum;
            saveUser.DocDate = user.DocDate;
            saveUser.DocIssuerCode = user.DocIssuerCode;
            saveUser.DocIssuerFullName = user.DocIssuerFullName;
            */
            saveUser.INN = user.INN;
            saveUser.Account = user.Account;
            saveUser.BankId = user.BankId;

            await _userManager.UpdateAsync(saveUser);

            return View();
        }

        [Route("contract/sign/{kontract_id}")]
        public async Task<IActionResult> SignContract(int? kontract_id)
        {
            //                   => File(await _pdfUtil.ContractMake(kontract_id.Value), "application/pdf");

            var contract = await _dbContext.Contracts.SingleAsync(f => f.RowId == kontract_id.Value);

            if (contract.Status == ContractStatus.Init)
            {
                contract.Status = ContractStatus.StartProcess;
                await _dbContext.SaveChangesAsync();
            }
            return Ok();

        }

        [Route("contract/sendprofile/{user_id}")]
        public async Task<IActionResult> SendProfile(Guid? user_id)
        {
            //                   => File(await _pdfUtil.ContractMake(kontract_id.Value), "application/pdf");

            var contract = await _dbContext.Recipients.SingleAsync(f => f.AppUserId == user_id.Value);
            contract.correlationId=Guid.NewGuid();
            await _dbContext.SaveChangesAsync();
            return Json(new { Error = "Ok-400" });

        }

        [Route("contract/show/{kontract_id}")]
        public async Task<IActionResult> ShowContract(int? kontract_id)
                               => File(await _pdfUtil.ContractMake(kontract_id.Value), "application/pdf");


        [HttpPut]
        public async Task<IActionResult> UpdateFeedback(int key, string values)
        {
            var row = await _dbContext.Feedbacks.FirstAsync(a => a.RowId == key);
            Console.WriteLine(values);
            JsonConvert.PopulateObject(values, row);
            row.Updated = DateTime.Now;

//            if (!TryValidateModel(row))
//                return BadRequest(ModelState.GetFullErrorMessage());

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerInfo(AgregatorUser userpass)
        {
            var user = await _userManager.GetUserAsync(User);
            /*
            user.DocName = userpass.DocName;
            user.DocSerie = userpass.DocSerie; 
            user.DocNum = userpass.DocNum;

            user.DocDate = userpass.DocDate;
            user.DocIssuerFullName = userpass.DocIssuerFullName;
            user.DocIssuerCode = userpass.DocIssuerCode;
            */
            user.INN = userpass.INN;
            //user.Address = userpass.Address;
            user.Account = userpass.Account;
            //user.Zip = userpass.Zip;
            user.BankId = userpass.BankId;

            await _userManager.UpdateAsync(user);

            return View(user); 
        }
    }
}
