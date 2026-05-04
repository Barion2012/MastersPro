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
using Microsoft.AspNetCore.Hosting;

namespace Agregator.Controllers
{
    using Data;

    [Authorize]
    public class DocumentsController : Controller
    {

        private readonly UserManager<AgregatorUser> _userManager;
        private readonly IFormatProvider _format;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

#if DESIGN
        private ApplicationDbContext _dbContext;
        public DocumentsController(
            ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public DocumentsController(
            ApplicationStoreContext dbContext
#endif
            , UserManager<AgregatorUser> userManager
            ,IFormatProvider format
            , ILogger<DocumentsController> logger
            , IWebHostEnvironment webHostEnvironment
            )
        {
            _userManager = userManager;
            _format = format; 
            _dbContext = dbContext;
            _logger = logger;
            _webHostEnvironment=webHostEnvironment;
        }

        public async Task<object> GetBanks(DataSourceLoadOptions loadOptions)
            => await DataSourceLoader.LoadAsync(_dbContext.Banks, loadOptions)
            ;

        public async Task<IActionResult> Sept()
        {
            string webRoot = _webHostEnvironment.WebRootPath;
            return File(System.IO.File.ReadAllBytes(System.IO.Path.Combine(webRoot,"pdf/202109.rar")), "application/x-rar-compressed", "YourJob-202109.rar");
        }




#if OLD
        public async Task<IActionResult> SignTest()
            => View(await _userManager.GetUserAsync(User))
            ;

        bool TryValidateModel()
        {
            //ModelState.AddModelError("", "Нет комплекта");
            //return false;
            //return base.TryValidateModel(model);
            return true;
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            if (!TryValidateModel())
                return BadRequest(ModelState.First().Value);

            dynamic req = JsonConvert.DeserializeObject(await new System.IO.StreamReader(Request.Body).ReadToEndAsync());
           // Console.WriteLine(Convert.ToString(req));
            string user_id = req.Id, text = req.Text;
            var user = await _userManager.FindByIdAsync(user_id);
            if (string.IsNullOrEmpty(user.INN))
                return BadRequest("ИНН не указан");
            if (string.IsNullOrEmpty(user.Account))
                return BadRequest("Счет не указан");

            var contract = new Contract()
            {
                IssuerId = (await _userManager.GetUserAsync(User)).Id
                ,
                CustomerId = Guid.Parse(user_id)
                ,
                Remark = text
                ,
                Status =  ContractStatus.Init
                ,
                Type = 1
                ,
                Created = DateTime.Now
            };

            _dbContext.Add(contract);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> SignTest(AgregatorUser userpass)
        {
            var user = await _userManager.GetUserAsync(User);
            user.INN = userpass.INN;
            user.Account = userpass.Account;
            user.BankId = userpass.BankId;
/*
            user.DocName = userpass.DocName;
            user.DocSerie = userpass.DocSerie; 
            user.DocNum = userpass.DocNum;
            user.DocDate = userpass.DocDate;
            user.DocIssuerFullName = userpass.DocIssuerFullName;
            user.DocIssuerCode = userpass.DocIssuerCode;
            user.Address = userpass.Address; 
            user.Zip = userpass.Zip;
*/
            await _userManager.UpdateAsync(user);
            return Redirect(await XML2pdf(await XML2DSOC(user)));
        }


        async Task<string> XML2pdf(string pdf)
        {
            var url = "https://content.dropboxapi.com/2/files/upload";
            JObject res;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer bsmfnrwgTscAAAAAAAAAAXA2wCOb6U6mafgfLpXC53YHBpk1Ukl8-QWV04UdaAKW");
                httpClient.DefaultRequestHeaders.Add("Dropbox-API-Arg", $"{{\"path\":\"/{System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetTempFileName())+".pdf"}\"}}");
                using (HttpContent cont = new  ByteArrayContent(await System.IO.File.ReadAllBytesAsync(pdf)))
                {
                    cont.Headers.Add("Content-Type", "application/octet-stream");
                    res =  (JObject)JsonConvert.DeserializeObject( await(await httpClient.PostAsync(url, cont)).Content.ReadAsStringAsync());
                }
            }

            url = "https://api.dropboxapi.com/2/sharing/create_shared_link_with_settings";

            using (HttpClient httpClient = new HttpClient())
            {
                string data= @$"{{""path"": ""{res["id"]}"",""settings"": {{}}}}";
                using (HttpContent cont = new StringContent(data,System.Text.Encoding.UTF8, "application/json"))
                {
                    var ret = await (await httpClient.PostAsync(url, cont)).Content.ReadAsStringAsync();
                    _logger.LogWarning(ret);

                    res = (JObject)JsonConvert.DeserializeObject(ret);
                }
            }

            url = "https://api.legium.io/v1/cases/venderpro";

            using (HttpClient httpClient = new HttpClient())
            {

                string message = @$"{{
""document_url"": ""{Convert.ToString(res["url"]).Split('?')[0]}"",
""title"": ""Договор подряда"",
""description"": ""Договор подрядя с ООО Техстрой"",
""price"": ""25000 руб."",
""customer"": {{
    ""phone"": ""+79255070748"",
    ""inn"": ""9706008544"",
    ""legal_form"": ""organization"",
    ""organization_name"": ""ООО КЬЮЛАБ ФЁСТ"",
    ""last_name"": ""Микула"",
    ""first_name"": ""Юрий"",
    ""patronymic_name"": ""Иваныч""
  }},
  ""executor"": {{
    ""phone"": ""+79152635032"",
    ""inn"": ""503129557206"",
    ""legal_form"": ""individual"",
    ""last_name"": ""Самойлов"",
    ""first_name"": ""Борис"",
    ""patronymic_name"": ""Викторович""
  }},
  ""postback_url"": ""https://agregator1.qlabfirst.ru:48443/api/who""
}}";

                _logger.LogInformation(message);

                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                using (HttpContent cont = new StringContent(message, System.Text.Encoding.UTF8, "application/json"))
                {
                    var resp = await httpClient.PostAsync(url, cont);
                    var ret = await resp.Content.ReadAsStringAsync();
                    _logger.LogWarning($"{resp.StatusCode},{ret}");

                    return Convert.ToString(JsonConvert.DeserializeObject(ret));
                }
            }
        }

        async Task<string> OLD_XML2DSOC(AgregatorUser user)
        {
            string tempPath = System.IO.Path.GetTempFileName();
            try
            {
                PdfFont pdfFont = PdfFontFactory.CreateFont(Properties.Resources.TIMES, PdfEncodings.IDENTITY_H);
                PdfFont pdfFontBd = PdfFontFactory.CreateFont(Properties.Resources.TIMESBD, PdfEncodings.IDENTITY_H);

                using System.IO.MemoryStream source = new System.IO.MemoryStream(Properties.Resources.dog1_);
                source.Position = 0;

                using PdfDocument pdf = new PdfDocument(new PdfReader(source), new PdfWriter(tempPath));

                var form = PdfAcroForm.GetAcroForm(pdf, true);
                form.SetGenerateAppearance(true);
                var fld = form.GetFormFields();

                const string numDog = "45123";
                
                fld["numDog"].SetValue(numDog, pdfFontBd, 11f);
                fld["dateDog"].SetValue(DateTime.Today.ToString("dd MMMM yyyy г.", _format), pdfFontBd, 11f);
                fld["FIO"].SetValue($"{user.Surname} {user.Name} {user.Middlename}", pdfFontBd, 11f);
                fld["footer"].SetValue($"Договор подряда № {numDog} от {DateTime.Today.ToString("dd MMMM yyyy", _format)} г.", pdfFont, 11f);
               // fld["docName"].SetValue(user.DocName, pdfFontBd, 11f);
           
                fld["INN"].SetValue(user.INN, pdfFont, 11f);
                fld["EMail"].SetValue(user.Email, pdfFont, 11f);
                fld["FIO2"].SetValue(user.Name?.Substring(0,1)+"."+user.Middlename?.Substring(0,1)+". "+user.Surname, pdfFontBd, 11f);
                fld["Account"].SetValue(user.Account, pdfFont, 11f);
           /*
                fld["Address"].SetValue(user.Address, pdfFont, 11f);
                fld["Zip"].SetValue(user.Zip, pdfFont, 11f);
            //    fld["Phone"].SetValue(user.PhoneNumber, pdfFont, 11f);
                fld["docName"].SetValue($"{user.DocSerie} {user.DocNum} выдан {user.DocDate?.ToString("dd.mm.yyyy")} {user.DocIssuerFullName}, код подразделения: {user.DocIssuerCode}", pdfFont, 11f);
           */

                //                foreach (var f in fld)
                //                    Console.WriteLine("{0}",f.Key);

                form.FlattenFields();

                pdf.SetCloseReader(true);
                pdf.SetCloseWriter(true);
                pdf.Close();

                return await Task.FromResult(tempPath);

                /*
                using (var docMS = System.IO.File.Create(tempPath))
                {

                    using (System.IO.MemoryStream tdocMS = new System.IO.MemoryStream(Agregator.Properties.Resources.xd))
                    {
                        await tdocMS.CopyToAsync(docMS);
                    }

                    using WordprocessingDocument wordDoc = WordprocessingDocument.Open(docMS, true);

                    string docText = null;
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    // Console.WriteLine(docText);
                    Regex regexText = new Regex("#docNum");
                    docText = regexText.Replace(docText, "1");
                    docText = Regex.Replace(docText, "#dd", DateTime.Now.ToString("dd"));
                    docText = Regex.Replace(docText, "#MM", DateTime.Now.ToString("MM"));
                    docText = Regex.Replace(docText, "#yy", DateTime.Now.ToString("yy"));
                    docText = Regex.Replace(docText, "#fio", user.Surname+" "+user.Name+" "+user.Middlename);
                    docText = Regex.Replace(docText, " #docName", user.DocName);
                    docText = Regex.Replace(docText, " #inn", user.INN);
                    docText = Regex.Replace(docText, " #email", user.Email);

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(wordDoc.MainDocumentPart.GetStream(System.IO.FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }

                var pdfPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(tempPath)
                    , System.IO.Path.GetFileNameWithoutExtension(tempPath) + ".pdf"
                    );

                Aspose.Words.Document document = new Aspose.Words.Document(tempPath);
                document.Save(pdfPath);


                return pdfPath;
                */


                //            await wordDoc.SaveAsync(   , SaveOptions.DisableFormatting, new System.Threading.CancellationToken(false));

                //await wordDoc.SaveAsync()



                //return File(Agregator.Properties.Resources.PDF, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");


                /*

                Body body = wordprocessingDocument.MainDocumentPart.Document.Body;
          //      body.Where(f => f.InnerText == "@td").First().InnerXml = DateTime.Now.ToString("dd");

                wordprocessingDocument.SaveAs(@"D:\EXIT\TECHSTROY\data\d1.doc");
                wordprocessingDocument.Close();


                var xdoc = XDocument.Load(@"D:\EXIT\TECHSTROY\data\d1.xml", LoadOptions.None);
                foreach(var d in xdoc.Root.Descendants().Where(f=>f.Value.Contains("@")|| f.Value.Contains("#")))
                {
                    Console.WriteLine("{0}:{1}", d.Name, d.NodeType);

                    if (d.Value=="@td") d.SetValue(DateTime.Now.ToString("dd"));
                    else if (d.Value=="@tm") d.SetValue(DateTime.Now.ToString("MMMM"));
                    else if (d.Value=="@ty") d.Value = DateTime.Now.ToString("yy");
                    else if (d.Value=="#NAME") d.Value = "Самойлов Б.В.";


                }
                */

                //using Stream s = File.OpenWrite(@"D:\EXIT\TECHSTROY\data\d1.doc");

                //await xdoc.SaveAsync(s,SaveOptions.DisableFormatting,new System.Threading.CancellationToken(false));

                //   await Task.FromResult(0);

            }
            finally
            {
                try
                {
              //      System.IO.File.Delete(tempPath);
                }
                catch
                {
                }
            }
        }
#endif
        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _dbContext.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }

            return View(countries);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Country,Alfa2,Alfa3,FullName,IsEaes,UseProfile")] Country countries)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(countries);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(countries);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _dbContext.Countries.FindAsync(id);
            if (countries == null)
            {
                return NotFound();
            }
            return View(countries);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Country,Alfa2,Alfa3,FullName,IsEaes,UseProfile")] Country countries)
        {
            if (id != countries.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(countries);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountriesExists(countries.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(countries);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) 
            {
                return NotFound();
            }

            var countries = await _dbContext.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }

            return View(countries);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var countries = await _dbContext.Countries.FindAsync(id);
            _dbContext.Countries.Remove(countries);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountriesExists(int id)
        {
            return _dbContext.Countries.Any(e => e.Id == id);
        }
    }
}
