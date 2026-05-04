using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ServiceModel;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Diagnostics;
using System.IO.Compression;
using Agregator.Data;
using SendGrid;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HtmlAgilityPack;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;
using System.Data;
using Microsoft.Data.SqlClient;


namespace Agregator.Controllers
{
    using Services;
    using Extensions;

    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly TwilioVerifySettings _twiloSettings;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<ApiController> _logger;
        private readonly PdfUtil _pdfUtil;
        private readonly SignInManager<AgregatorUser> _signInManager;
        private readonly ISignService _signService;

#if DESIGN
        private ApplicationDbContext _dbContext;
        public ApiController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public ApiController(
            ApplicationStoreContext dbContext
#endif
            , UserManager<AgregatorUser> userManager
            , IOptions<TwilioVerifySettings> tw
            , IHttpContextAccessor accessor
            , ILogger<ApiController> logger
            , PdfUtil pdfUtil
            , SignInManager<AgregatorUser> signInManager
            , ISignService signService
            )
        {
            _dbContext = dbContext;
            //            httpClient = c.CreateClient("AKRUS");
            _twiloSettings = tw.Value;

            _accessor = accessor;
            _logger = logger;
            _pdfUtil = pdfUtil;
            _signInManager = signInManager;
            _signService = signService;
            _userManager=userManager;
        }

#if !DESIGN
        [HttpPost("ComplateSignTask")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Администратор")]
        public async Task<IActionResult> ComplateSignTask()
        {
            string postData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            await _signService.Complete(postData);
            return new JsonResult(@"{""ok"":""OK""}");
        }


        SignTaskEvent signTask = new SignTaskEvent();

        void SetDelegate(object o, ComplatedEventArgs e)
        {
            if (signTask.ConversationID.Equals(e.CoversationID))
            {
                signTask.Set(e.Result);
            }
        }


        [AllowAnonymous]
        [HttpPost("loginphone")]
        public async Task<IActionResult> LoginWithPhone()
        { 

            try
            {

                var bodyData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                JObject jsone = (JObject)JsonConvert.DeserializeObject(bodyData);

                var phone = jsone["phone"].ToString();

                if (!await _userManager.Users.AnyAsync(f => f.PhoneNumber == phone) 
                    &&
                    !await _userManager.Users.AnyAsync(f => f.UserName == phone))
                {
                    return NotFound();
                }


                _signService.Add(SetDelegate);

                jsone.Add("task_id",  JToken.FromObject(signTask.ConversationID.ToString()));

                await _dbContext.RequestValidateCode(jsone.ToString());

                signTask.Wait(100000);
                _signService.Remove(SetDelegate);

                if (signTask.EventData == "pending")
                    return Ok();
                else
                    return new StatusCodeResult(StatusCodes.Status205ResetContent);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return new StatusCodeResult(500);
        }

        [HttpPost("codeforsign")]
        [Authorize]
        public async Task<IActionResult> SignContractSendCode()
        {

            AgregatorUser user = await _userManager.GetUserAsync(User);

            try
            {
                var jsone = JObject.Parse(@$"{{""phone"":""{user.PhoneNumber}""}}");

                _signService.Add(SetDelegate);

                jsone.Add("task_id", JToken.FromObject(signTask.ConversationID.ToString()));
                _logger.LogError(jsone.ToString());

                await _dbContext.RequestValidateCode(jsone.ToString());

                signTask.Wait(100000);
                _signService.Remove(SetDelegate);


                _logger.LogError(signTask.EventData);

                if (signTask.EventData == "pending")
                    return Ok();


                /*
                var verification = await VerificationResource.CreateAsync(to: user.PhoneNumber,
                            channel: "sms",
                            pathServiceSid: _twiloSettings.VerificationServiceSID
                            , locale: "ru");

                if (verification.Status == "pending")
                    return Ok();
                */
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


                _signService.Add(SetDelegate);

                jsone.Add("task_id", JToken.FromObject(signTask.ConversationID.ToString()));
                await _dbContext.CheckValidateCode(jsone.ToString());

                signTask.Wait(100000);
                _signService.Remove(SetDelegate);

                if (signTask.EventData != "approved")
                    return new StatusCodeResult(StatusCodes.Status205ResetContent);

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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok();
        }


        [HttpPost("signing/{kontract_id}/{code}")]
        [Authorize]
        public async Task<IActionResult> SignContract(int kontract_id, string code)
        {

            try
            {

                AgregatorUser user = await _userManager.GetUserAsync(User);
                var jsone = JObject.Parse(@$"{{""phone"":""{user.PhoneNumber}""}}");
                jsone.Add("kontract_id", JToken.FromObject(kontract_id));
                jsone.Add("code", JToken.FromObject(code));

                _signService.Add(SetDelegate);

                jsone.Add("task_id", JToken.FromObject(signTask.ConversationID.ToString()));
                await _dbContext.CheckValidateCode(jsone.ToString());

                signTask.Wait(100000);
                _signService.Remove(SetDelegate);

                if (signTask.EventData == "approved")
                {
                    var contract = _dbContext.Contracts.Single(f => f.RowId == kontract_id);
                    contract.Status = ContractStatus.Active;
                    contract.SignDate = DateTime.Now;
                    await _dbContext.SaveChangesAsync();

                    return new JsonResult(new { Error = string.Empty, SignDateText = contract.SignDate.Value.ToString("dd.MM.yyyy") });

                }
                else
                {
                    return new JsonResult(new { Error = "Код потверждения введен неверно." });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok();

        }




#else

        [HttpPost("signing/{kontract_id}/{code}")]
        [Authorize]
        public async Task<IActionResult> SignContract(int kontract_id, string code)
        {
            AgregatorUser user = await _userManager.GetUserAsync(User);


            var verification = await VerificationCheckResource.CreateAsync(
    to: user.PhoneNumber,
    code: code,
    pathServiceSid: _twiloSettings.VerificationServiceSID
);

            if (verification.Status == "approved")
            {

                var contract = _dbContext.Contracts.Single(f => f.RowId == kontract_id);
                contract.Status = ContractStatus.Active;
                contract.SignDate = DateTime.Now;
                contract.DocUri = "LocalStorage";
                await _dbContext.SaveChangesAsync();

                return new JsonResult(new { Error = string.Empty, SignDateText = contract.SignDate.Value.ToString("dd.MM.yyyy") });
            }
            else
            {
                return new JsonResult(new { Error = "Код потверждения введен неверно." });
            }
        }


        [HttpPost("codeforsign")]
        [Authorize]
        public async Task<IActionResult> SignContractSendCode(string kontract_id)
        {

            AgregatorUser user = await _userManager.GetUserAsync(User);

            try
            {
                var verification = await VerificationResource.CreateAsync(to: user.PhoneNumber,
                            channel: "sms",
                            pathServiceSid: _twiloSettings.VerificationServiceSID
                            , locale: "ru");

                if (verification.Status == "pending")
                    return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return new StatusCodeResult(500);
        }

        [AllowAnonymous]
        [HttpPost("loginphone")]
        public async Task<IActionResult> LoginWithPhone()
        {

            try
            {
                var bodyData = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                JObject jsone = (JObject)JsonConvert.DeserializeObject(bodyData);

                var phone = jsone["phone"].ToString();

                if (!await _userManager.Users.AnyAsync(f => f.PhoneNumber == phone) 
                    &&
                    !await _userManager.Users.AllAsync(f => f.UserName == phone))
                {
                    return NotFound();
                }

                var verification = await VerificationResource.CreateAsync(
                    to: phone
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
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return Ok();
        }

#endif



        [HttpPost("whatsapp")]
        [AllowAnonymous]
        public async Task<IActionResult> WhatsApp()
        {
            try
            {
                await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0}\n{1}\n", HttpContext.Request.Method, Request.QueryString.HasValue? Request.QueryString.Value:"NOPATH"));
                //var json = await HttpContext.Request.Body.GetBody();

                using (var s = new System.IO.StreamReader(Request.Body))
                {
                    var req = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(await s.ReadToEndAsync());
                    /*
                    foreach (var r in req)
                    {
                        await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0}={1}\n", r.Key,r.Value));
                    }*/


                    /*
                    var response = new MessagingResponse();
                    var message = new Message();
                    message.Body("Hello World!");
                    response.Append(message);
                    //response.Redirect(url: new Uri("https://demo.twilio.com/welcome/sms/"));

                   // _logger.LogInformation(response.ToString());

                    response.Redirect();

                    //var xml = new XDocument(response.ToString());

                    return Content(response.ToString(),"text/xml");
                    */

                    if ( req.Keys.Any(f=>f== "SmsMessageSid") && req.SingleOrDefault(f=>f.Key=="Body").Value == "/help")
                    {
                        var res = new Microsoft.Data.SqlClient.SqlParameter("@res", System.Data.SqlDbType.VarChar, 2000);
                        res.Direction = System.Data.ParameterDirection.Output;
                        await _dbContext.Database.ExecuteSqlRawAsync("EXECUTE [dbo].[SendWhatsapp] @tel,@body,@res OUTPUT",
                            new Microsoft.Data.SqlClient.SqlParameter("@tel", "+79777518648")
                            , new Microsoft.Data.SqlClient.SqlParameter("@body", "Hello from Twilio")
                            , res
                            );
                    }
                }
                return Ok();


            }
            catch (Exception ex)
            {
                //      throw;

                _logger.LogError(ex, ex.ToString());

                return new JsonResult(new
                {
                    task = new
                    {
                        error = ex.Message
                    ,
                        info = ex.ToString()
                    ,
                        trace = ex.StackTrace
                    }
                });
            }

        }


        [HttpPost("tasks")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Администратор")]
        public async Task<IActionResult> Tasks()
        {
            int task_id = 0;
            try
            {
                await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0}\r\n", HttpContext.Request.Method));
                var json = await HttpContext.Request.Body.GetBody();
                await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0}\r\n", json.ToString()));

                Guid issuerId = Guid.Parse("222EA056-E7A2-4118-5531-08D91ABE147E");

                var user = await _dbContext.Users.FirstOrDefaultAsync(f => f.PhoneNumber == json["executor"].ToString());
                if (user == null)
                {
                    user = new AgregatorUser() { UserName = json["executor"].ToString() };

                    user.Name = json["executorInfo"]["firstName"].ToString();
                    _logger.LogInformation($"{user.Name}");
                    user.Surname = json["executorInfo"]["lastName"].ToString();
                    user.Middlename = json["executorInfo"]["middleName"]?.ToString();
                    user.PhoneNumber = json["executor"]?.ToString();
//                    user.Email = json["executorInfo"]["Email"]?.ToString();
                    user.Male = true;
                    user.Birthday = json["executorInfo"]["Birthday"] == null ? null : Convert.ToDateTime(json["executorInfo"]["Birthday"]);
//                    user.CountryCitizenshipId = json["executorInfo"]["Citizenship"] == null ? null : (int?)Convert.ToInt32(json["executorInfo"]["Citizenship"]);
//                    user.BankId = Guid.Parse("4E876077-7800-4274-963B-D6F8BCD47894");
                    user.Account = json["executorInfo"]["accountNumber"]?.ToString();
                    user.Photo = Properties.Resources.photo_placeholder_man;
                    user.ProfileReady = true;
                    user.agreeRegFNS = true;

                    await _userManager.CreateAsync(user);

                    //user.Id;


                }
          //      _logger.LogInformation($"{user.Id}");

                var contract = new Contract()
                {
                    IssuerId = issuerId
                    ,
                    CustomerId = user.Id
                    ,
                    DocDate = DateTime.Parse(json["date"].ToString())
                    ,
//                    Status = ContractStatus.Init
                    Status = ContractStatus.ToSign
                    ,
                    Type = 1

                    ,
                    Remark = string.Empty
                };
                _dbContext.Contracts.Add(contract);
                await _dbContext.SaveChangesAsync();



                short num = 0;
          //      _logger.LogInformation($"{num}");
                var ar = (JArray)json["metadata"]["legal"]["agreement"];
            //    _logger.LogInformation($"{num} {ar} {ar.LongCount()}");

                foreach (var x in  ar)
                {
              //      _logger.LogInformation(x.ToString());
                    

                    var work = await _dbContext.Works.FirstOrDefaultAsync(f => f.work_id.HasValue && f.work_id.Value == Guid.Parse(Convert.ToString(x["work_id"])));
//                    var work = await _dbContext.Works.FirstOrDefaultAsync(f => f.Name == Convert.ToString(x["work_name"]));

                    if (work == default(Work))
                    {
                        work = new Work
                        {
                            Name = Convert.ToString(x["work_name"]),
                            work_id = Guid.Parse(Convert.ToString(x["work_id"]))
                        };

                        await _dbContext.AddAsync(work);
                        await _dbContext.SaveChangesAsync();

                    }
                    _logger.LogInformation(work.Name);

                    var attachment_row = new ContractAttachment
                    {
                        ContractId = contract.RowId
                       ,
                        RowNum = ++num
                      // , UserId = null
                      ,
                        Quant = Convert.ToDecimal(x["quant"])
                      ,
                        Price = Convert.ToDecimal(x["price"])
                        ,Unit = Convert.ToString(x["unit"])
                      ,
                        WorkId = work.RowId
                        , object_id = Guid.Parse(Convert.ToString(x["full_path"]["object_id"]))
                        , object_name = Convert.ToString(x["full_path"]["object_name"])
                        , floor = Convert.ToInt32(x["full_path"]["floor"])
                        , section = Convert.ToInt32(x["full_path"]["section"])
                        , room = Convert.ToInt32(x["full_path"]["room"])
                        ,
                        building = Convert.ToInt32(x["full_path"]["building"])
                        ,
                        app_type = Convert.ToString(x["full_path"]["app_type"])
                        ,
                        path_string = Convert.ToString(x["full_path"]["path_string"])


                    };

                    _logger.LogInformation(attachment_row.object_name);

                    await _dbContext.ContractAttachments.AddAsync(attachment_row);

                }

                await _dbContext.SaveChangesAsync();

                task_id = contract.RowId;

                var res = new SqlParameter("@res", SqlDbType.VarChar, 2000);
                res.Direction = System.Data.ParameterDirection.Output;
                await _dbContext.Database.ExecuteSqlRawAsync("EXECUTE [dbo].[SendWhatsapp] @tel,@body,@res OUTPUT",
                    new SqlParameter("@tel", user.PhoneNumber)
                    , new SqlParameter("@body", $@"Ув. {user.Name} {user.Surname ?? string.Empty}!
Вам открыт доступ в личный каб. YourJob по номеру телефона. 
Для продолжения работы с сервисом необходимо выполнить вход, 
и подписать договор-подряда № {contract.RowId} с помощью телефона (Цифровая подпись)"));


                /*
                var messageOptions = new CreateMessageOptions(
new PhoneNumber(user.PhoneNumber)
);
                
                messageOptions.MessagingServiceSid = "MG89563c918dc64b4b27b92d4fd4ab1191";
                messageOptions.Body = $@"Ув. {user.Name} {user.Surname ?? string.Empty}!
Вам открыт доступ в личный каб. YourJob по номеру телефона. 
Для продолжения работы с сервисом необходимо выполнить вход, 
и подписать договор-подряда № {contract.RowId} с помощью телефона (Цифровая подпись)

https://qlabfirst.ru/User/MyContracts
";

                var message = MessageResource.Create(messageOptions);
                

                return new JsonResult( new { task = new { id = contract.RowId, status= "draft", ToSignCMS = message.Status } });
            */
                var recipt = await _dbContext.Recipients.SingleOrDefaultAsync(f=>f.AppUserId==user.Id);

                return new JsonResult(new { task = new { id = task_id, status = "draft", selfEmployedStatus = recipt==null? "NONE": recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus } });


            }
            catch (Exception ex)
            {
          //      throw;

                _logger.LogError(ex,ex.ToString());

                return new JsonResult(new
                {
                    task = new
                    {
                        id = task_id
                    ,
                        status = "failed"
                    ,
                        payment_errors = ex.Message

                    ,
                        info = ex.InnerException?.ToString()
                    ,
                        trace = ex.StackTrace
                    }
                });
            }

        }

        [HttpPost("sendmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Администратор")]
        public async Task<IActionResult> SendMail()
        {
            try
            {

                var json = await HttpContext.Request.Body.GetBody();
                _logger.LogWarning(json.ToString());

                var res = await _dbContext.Database.ExecuteSqlRawAsync(@"exec [msdb].[dbo].[sp_send_dbmail] @profile_name={0},@recipients={1},@copy_recipients={2},@blind_copy_recipients={3},@subject={4},@body={5},@body_format={6}",
new[]
{
    "Your Job",
    Convert.ToString(json["recipients"]),
    Convert.ToString(json["copy_recipients"])??Convert.DBNull,
    "admin@qlabfirst.ru",
    Convert.ToString(json["subject"]),
    Convert.ToString(json["body"]),
    "HTML"
});

                return new JsonResult(new {ok=true});

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    task = new
                    {
                        error = ex.Message
                    ,
                        info = ex.InnerException?.ToString()
                    ,
                        trace = ex.StackTrace
                    }
                });
            }
        }

        //на оплату
        [Route("tasks/{id}/{mode}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Администратор")]
        public async Task<IActionResult> Tasks(string id, string mode)
        {

            Contract contr = null;
            try
            {
                await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0} {1} {2}\r\n=================\r\n", HttpContext.Request.Method, string.IsNullOrEmpty(id) ? "ID" : id, string.IsNullOrEmpty(mode) ? "MODE" : mode));

                var json = await HttpContext.Request.Body.GetBody();
                contr = await _dbContext.Contracts.SingleAsync(f => f.RowId.ToString() == id);
                contr.Fee = Convert.ToDecimal(json["fee"]);
                var user = contr.Customer;

                if (contr.Status == ContractStatus.ToSign)
                {
                    var res = new SqlParameter("@res", SqlDbType.VarChar, 2000);
                    res.Direction = System.Data.ParameterDirection.Output;
                    await _dbContext.Database.ExecuteSqlRawAsync("EXECUTE [dbo].[SendWhatsapp] @tel,@body,@res OUTPUT",
                        new SqlParameter("@tel", user.PhoneNumber)
                        , new SqlParameter("@body", @$"Подпишмте договор № {contr.RowId}, 
https://qlabfirst.ru/User/MyContracts")
                        , res
                        );

                    throw new Exception("Договор не подписан ЦП СЗ.");
                }
                if (DateTime.Now-contr.DocDate< new TimeSpan(3,0,0,0))
                    throw new Exception("Оплата производитсься не ранее 3 дней после начала работ.");


                if (mode == "complete")
                {
                    contr.Status = ContractStatus.Finished;
                }
                else if (mode == "pay")
                {
                    contr.Status = ContractStatus.ToPay;
                }
                else
                {
                    await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", $"{json} {mode} {mode == "complete"}");
                }



                await _dbContext.SaveChangesAsync();

                return await tasks(id);

            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    task = new
                    {
                        id = id
                    ,
                        status = "draft"
                    //                      ,
                    //                        selfEmployedStatus = recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus
                    ,
                        payment_errors = ex.Message

                    ,
                        info = ex.InnerException?.ToString()
                    ,
                        trace = ex.StackTrace
                    }
                });
            }

        }


        private async Task<JsonResult> tasks(string id)
        {
            try
            {

                var contr = await _dbContext.Contracts.SingleAsync(f => f.RowId.ToString() == id);
                var recipt = await _dbContext.Recipients.SingleOrDefaultAsync(f => f.AppUserId == contr.CustomerId);
                if (recipt == default(Recipient))
                    throw new Exception("Нет персональных данных.");

                string ret = "";

                switch (contr.Status)
                {
                    case ContractStatus.ToSign:
                        ret = "draft"; 
                        break;
                    case ContractStatus.Active:
                        ret = "inwork";
                        break;
                    case ContractStatus.Finished:
                        ret = "completed";
                        break;
                    case ContractStatus.ToPay:
                        ret = "paying";
                        break;

                    case ContractStatus.Paid:

                        ret = "paid";
                        return new JsonResult(new { task = new { id = id, status = ret, selfEmployedStatus = recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus, receipt_uri = contr.FNS_uri, date = contr.PaidDate?.ToString("yyyy-MM-dd") } });


                    //согласование
                    //   case ContractStatus.Finished:
                    //      ret = "completed";
                    //      break;
                    case ContractStatus.Closed:
                        ret = "canceled";
                        break;
                    case ContractStatus.Archived:
                        ret = "failed";
                        break;
                }

                return new JsonResult(new { task = new { id = id, status = ret, selfEmployedStatus = recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus } });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
                throw;
            }

        }


        //Запрос статуса
        [Route("tasks/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Администратор")]
        public async Task<IActionResult> Tasks(string id)
        {
            try
            {
                await System.IO.File.AppendAllTextAsync("/home/temp/tasks.txt", string.Format("{0} {1}\r\n=================\r\n",HttpContext.Request.Method , string.IsNullOrEmpty(id)?"ID":id
                    ));


                return await tasks(id);


                /*                
                 *                                var contr = await _dbContext.Contracts.SingleAsync(f => f.RowId.ToString() == id);
                                var recipt = await _dbContext.Recipients.SingleAsync(f => f.AppUserId == contr.CustomerId);

                                string ret = "";

                                switch(contr.Status)
                                {
                                    case ContractStatus.ToSign:
                                        ret = "draft";
                                        break;
                                    case ContractStatus.Active:
                                        ret = "inwork";
                                        break;
                                    case ContractStatus.Finished:
                                        ret = "completed";
                                        break;
                                    case ContractStatus.ToPay:
                                        ret = "paying";
                                        break;

                                    case ContractStatus.Paid:

                                        ret = "paid";
                                        return new JsonResult(new { task = new { id = id, status = ret, selfEmployedStatus= recipt.accountStatus=="ACTIVE"? recipt.selfEmployedStatus: recipt.accountStatus, receipt_uri = contr.FNS_uri , date = contr.PaidDate.ToString("yyyy-MM-dd")} });


                //согласование
                                 //   case ContractStatus.Finished:
                                  //      ret = "completed";
                                  //      break;
                                    case ContractStatus.Closed:
                                        ret = "canceled";
                                        break;
                                    case ContractStatus.Archived:
                                        ret = "failed";
                                        break;
                                }

                                return new JsonResult(new { task = new { id = id, status=ret, selfEmployedStatus = recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus } });
                 */
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    task = new 
                    {
                        id = id
    ,
                        status = "draft"
    //                      ,
    //                        selfEmployedStatus = recipt.accountStatus == "ACTIVE" ? recipt.selfEmployedStatus : recipt.accountStatus
    ,
                        payment_errors = ex.Message

    ,
                        info = ex.InnerException?.ToString()
    ,
                        trace = ex.StackTrace
                    }
                });

            }
        }

        [HttpGet]
        [Route("kontract/{kontract_id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles = "Администратор")]
        public async Task<IActionResult> MakeKontract(int? kontract_id)
        {


            return new FileStreamResult( new System.IO.MemoryStream(await _pdfUtil.ContractMake(kontract_id.Value)), "application/pdf")
            {
                FileDownloadName = $"{kontract_id.Value}.pdf"
            };
        }



        [Route("who")]
        [AllowAnonymous]
        public async Task<IActionResult> Who()
        {
            var callbackData = await new System.IO.StreamReader(_accessor.HttpContext.Request.Body).ReadToEndAsync();

            JObject jsone = (JObject)JsonConvert.DeserializeObject(callbackData);
            _logger.LogError(callbackData);

            var docname = jsone["document_url"].ToString();
            var title = jsone["title"].ToString();

            int? row_id=null;
            try
            { 
                //int row_id = int.Parse(docname.Split('/').Last().Split('.').First().Replace("doc",""));
                row_id = int.Parse(title.Split(' ').Last());//.Length - 2, 2);); ;
            }
            catch(Exception ex)
            {
                try
                {
                    row_id = int.Parse(docname.Split('/').Last().Split('.').First().Replace("doc", ""));
                }
                catch(Exception ex1)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("==============");
                    Console.WriteLine(ex1);
                }
            }

            var contract = _dbContext.Contracts.Single(f => f.RowId == row_id.Value);
            contract.Status = ContractStatus.Active;
            contract.SignDate = DateTime.Now;
            contract.DocUri = docname;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("internal/cs{row_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> SignDocument(int? row_id)
        {
            var callbackData = await new System.IO.StreamReader(_accessor.HttpContext.Request.Body).ReadToEndAsync();

            JObject jsone = (JObject) JsonConvert.DeserializeObject(callbackData);
            _logger.LogError(callbackData);

            var docname = jsone["document_url"].ToString();
            //var postback = jsone["postback_url"].ToString();

            //int row_id = int.Parse(docname.Split('/').Last().Split('.').First().Replace("doc",""));

            var contract = _dbContext.Contracts.Single(f => f.RowId == row_id);
            contract.Status = ContractStatus.Active; 
            contract.SignDate = DateTime.Now;
            contract.DocUri = docname;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }



        [HttpPost]
        [Route("confirmphone")]
        [Authorize]
        public async Task<IActionResult> ConfirmPhone()
        {

            AgregatorUser user = await _userManager.GetUserAsync(User);
            var body = (JObject)await HttpContext.Request.Body.GetBody();
            var phone = body["phone"].ToString();
            try
            {
                var verification = await VerificationCheckResource.CreateAsync(
                    to: phone,
                    code: body["code"].ToString(),
                    pathServiceSid: _twiloSettings.VerificationServiceSID
                );
              
                if (verification.Status == "approved")
                {
                    await _userManager.SetPhoneNumberAsync(user, phone);

                    user.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (updateResult.Succeeded)
                    {
                        return Ok();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return new StatusCodeResult(500);
        }

        [HttpPost]
        [Route("verifyphone")]
        [Authorize]
        public async Task<IActionResult> VerifiPhone()
        {

            //            AgregatorUser user = await _userManager.GetUserAsync(User);
            //    Console.WriteLine("n{0} e{1} p{2}", user.Name, user.Email, user.PhoneNumber);
            var body = (JObject)await HttpContext.Request.Body.GetBody();

            try
            {
                var verification = await VerificationResource.CreateAsync(to: body["phone"].ToString()
                    , channel: "sms"
                    , pathServiceSid: _twiloSettings.VerificationServiceSID
                    , locale: "ru");

                if (verification.Status == "pending")
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return new StatusCodeResult(500);
        }



        [HttpPost]
        [Route("setphoto")]
        [Authorize]
        public async Task<IActionResult> OnSetPhoto()
        {
            AgregatorUser user = await _userManager.GetUserAsync(User);
            using var r = new System.IO.StreamReader(HttpContext.Request.Body);
            dynamic jsonRes = JsonConvert.DeserializeObject(await r.ReadToEndAsync());
            string data = jsonRes.data;
            user.Photo = Convert.FromBase64String(data.Split(',')[1]);
            await _userManager.UpdateAsync(user);
            return new JsonResult(jsonRes);

        }


        [HttpGet]
        [Route("test")]
        [Authorize]
        public void ToServerLog(string msg) => _logger.LogWarning(msg);


        [HttpPost]
        [Route("onfideback")]
        [Authorize]
        public async Task<ActionResult> OnFideback()
        {
            try
            {
                AgregatorUser user = await _userManager.GetUserAsync(User);

                using var r = new System.IO.StreamReader(HttpContext.Request.Body);

                dynamic jsonRes = JsonConvert.DeserializeObject(await r.ReadToEndAsync());
  

                _dbContext.Feedbacks.Add(new Feedback
                {
                    UserId = user.Id
                     ,
                    Vacancy = jsonRes.Vacancy
                     ,
                    Status = 1
                });

                await _dbContext.SaveChangesAsync();
                return new OkResult();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new StatusCodeResult(500);

            }
        }


        [HttpPut]
        [Route("uploadchanges")]
        [AllowAnonymous]
        public async Task<ActionResult> ApplyAllChanges()
        {
            try
            {


                using (Process proc = new Process())
                {
                    proc.StartInfo = new ProcessStartInfo("mysql");
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();
                    proc.StandardInput.AutoFlush = true;

                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        await HttpContext.Request.Body.CopyToAsync(ms);
                        ms.Position = 0;

                        using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true))
                        {
                            //                        await HttpContext.Request.Body.CopyToAsync(proc.StandardInput.BaseStream);
                            await zip.CopyToAsync(proc.StandardInput.BaseStream);
                        }
                    }
                    proc.StandardInput.Close();

                    var error = await proc.StandardError.ReadToEndAsync();
                    if (error.Length > 0)
                    {
                        _logger.LogError("Error: {0}\nreturn StatusCode=500", error);
                        return new StatusCodeResult(500);
                    }

                    //new Thread(async () => await CkeckIdentity()).Start();
                    await CkeckIdentity();
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: {0}\nreturn StatusCode=500", ex.ToString());
                return new StatusCodeResult(500);
            }
        }

        async Task CkeckIdentity()
        {
            await Task.FromResult(0);
#if OLD
            using (MySqlCommand cmd = new MySqlCommand())// (MySqlCommand)gprContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = @"select distinct u.id from Feedbacks f join agregator.AspNetUsers u on u.id=f.aspNetUser
where f.status=2 and u.IdentityReady=0";

                var connState = cmd.Connection.State;
                if (connState != System.Data.ConnectionState.Open)
                    await cmd.Connection.OpenAsync();
                logger.LogWarning($"From Task connection={connState}");
                //                        var tran = await cmd.Connection.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted);
                //                        cmd.Transaction = tran;

                IList<string> uids = new List<string>();

                using (var r = await cmd.ExecuteReaderAsync())
                {
                    while (await r.ReadAsync())
                    {
                        uids.Add(r.GetString(0));
                    }
                }
                if (connState != System.Data.ConnectionState.Open)
                    await cmd.Connection.CloseAsync();

                foreach (var uid in uids)
                {

                    var user = await userManager.FindByIdAsync(uid);
                    user.IdentityReady = true;
                    var res = await userManager.UpdateAsync(user);
                    logger.LogWarning($"Update user={res}");


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
                    birthPlace = user.PlaceOfBirth,
                    photo = user.Photo == null ? string.Empty : Convert.ToBase64String(user.Photo),
                    male = user.Male.Value ? 0 : 1,
                    citizenship = user.Citizenship,
                    phoneNumber = user.PhoneNumberConfirmed ? user.PhoneNumber : "",
                    email = user.Email
                })))
                    {
                        //var resp = "";//await httpClient.PostAsync("/localapi/Register", httpContext);
                        //logger.LogWarning($"status={resp.StatusCode}");
                    }
                }
            }
       
#endif
        }
    }
}
