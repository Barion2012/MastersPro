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
using System.Security.Claims;

namespace Agregator.Controllers
{
    using Data;
    using Models;
    using Services;

    [Authorize]
#if !DESIGN
    [RequireHttps]
#endif
    public partial class VacancyController : Controller
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly IFormatProvider _format;
        private readonly SignInManager<AgregatorUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;


#if DESIGN
        private readonly ApplicationDbContext _dbContext;
        public VacancyController(
            ApplicationDbContext dbContext
#else
        private readonly ApplicationStoreContext _dbContext;
        public VacancyController(
            ApplicationStoreContext dbContext
#endif

            , UserManager<AgregatorUser> userManager
            , SignInManager<AgregatorUser> signinManager
            , ILogger<AccountController> logger
            , IEmailSender emailSender
            , IFormatProvider format
            )
        {
            _userManager = userManager;
            _signInManager = signinManager;
            _format = format;
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Start() 
        {
            if (User.Identity.IsAuthenticated)
                return await Task.FromResult(RedirectToAction("VacancyIndex"));
            else
                return await Task.FromResult(View());
        }
        

        [HttpGet]
        public async Task<IActionResult> VacancyIndex()
        {
       //    _logger.LogInformation("{Name}", User.Identity.Name);
            var user = await _userManager.GetUserAsync(User);
//            _logger.LogInformation("{name} {prof}", user?.Name, user?.ProfileReady);
            var vacancies = new VacancyListModel();
            vacancies.profileReady = user.ProfileReady;

            foreach (var x in await _dbContext.Vacancies.Include(v => v.PositionNavigation).Include(v => v.Feedbacks).Include(v => v.VacancyBuildObjectCts).ThenInclude(t => t.ObjectNavigation).ToListAsync())
            {
                foreach (var z in x.VacancyBuildObjectCts)
                {
                    var vacance = new VacancyModel
                    {
                        techObject = z.ObjectNavigation,
                        vacancy = x,

                        fideBackStatus = x.Feedbacks.SingleOrDefault(f => f.UserId.Equals(
                            Guid.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value)))?.Status
                    };


                    string FormStez(string fld)
                    {
                        StringBuilder sb = new StringBuilder();
                        bool once = true;
                        foreach (var stez in fld.Split('\n'))
                        {
                            if (!string.IsNullOrWhiteSpace(stez))
                                if (once)
                                {
                                    sb.AppendFormat(@"<br><span style=""font-size: 15pt;line-height: 70%"">&bull;&emsp;</span>{0}{1};", stez[0].ToString().ToUpper(), stez.Substring(1));
                                    once = false;
                                }
                                else
                                {
                                    sb.AppendFormat(@"<br><span style=""font-size: 15pt;line-height: 70%"">&bull;&emsp;</span>{0};", stez);
                                }
                        }
                        if (sb.Length > 0) sb[sb.Length - 1] = '.';
                        return sb.ToString();

                    }

                    for (int ntab = 1; ntab < 4; ++ntab)
                    {

                        switch (ntab)
                        {
                            case 1:

                                vacance.accordPages.Add(new AccordPage()
                                {
                                    HtmlTitle = "Обязанности",
                                    HtmlBody = FormStez(x.DescriptionTab1)
                                });

                                break;
                            case 2:

                                vacance.accordPages.Add(new AccordPage()
                                {
                                    HtmlTitle = "Требования",
                                    HtmlBody = FormStez(x.DescriptionTab2)
                                });
                                break;
                            case 3:

                                vacance.accordPages.Add(new AccordPage()
                                {
                                    HtmlTitle = "Условия",
                                    HtmlBody = FormStez(x.DescriptionTab3)
                                });
                                break;
                        }
                    }
                    vacancies.Add(vacance);
                }

            }

            return View(vacancies);
        }
    }
}