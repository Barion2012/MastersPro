using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Agregator.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace Agregator.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AgregatorUsersController : Controller
    {
        private readonly UserManager<AgregatorUser> _userManager;

#if DESIGN
        private ApplicationDbContext _context;
        public AgregatorUsersController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public AgregatorUsersController(
            ApplicationStoreContext context
#endif
            , UserManager<AgregatorUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) 
        {
        
            var appusers = _context.AppUsers.Select(i => new 
            {
                i.Id,
                i.Name,
                i.Surname,
                i.Middlename,
                i.Birthday,
//                i.PlaceOfBirth,
                i.ProfileReady,
                i.IdentityReady,
                i.Male,
                i.CountryCitizenshipId,
                i.INN,
                /*
                i.DocName,
                i.DocSerie,
                i.DocNum,
                i.DocDate,
                i.DocIssuerFullName,
                i.DocIssuerCode,
                i.Address,
                */
                i.Account,
                //i.Zip,
                i.BankId,

                i.PhoneNumber,
                i.PhoneNumberConfirmed,
                i.Email
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(appusers, loadOptions));
        }


        [HttpGet]
        public async Task<IActionResult> GetAppUser(Guid user_id)
        {
            var appusers = _context.AppUsers.Select(i => new {
                i.Id,
                i.Name,
                i.Surname,
                i.Middlename,
                i.Birthday,
          //      i.PlaceOfBirth,
                i.ProfileReady,
                i.IdentityReady,
                i.Male,
                i.CountryCitizenshipId,
                i.INN,
                /*
                i.DocName,
                i.DocSerie,
                i.DocNum,
                i.DocDate,
                i.DocIssuerFullName,
                i.DocIssuerCode,
                i.Address,
                */
                i.Account,
//                i.Zip,
                i.BankId,
                i.PhoneNumber,
                i.PhoneNumberConfirmed,
                i.Email
            });


            return Json(await appusers.SingleAsync(f=>f.Id==user_id));
        }



        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new AgregatorUser();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.AppUsers.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) {
            //var model = await _userManager.FindByIdAsync(key.ToString());
            var model = await _context.Users.SingleOrDefaultAsync(f => f.Id == key);
            if(model == null)
                return StatusCode(409, "AppUser not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            //await _userManager.UpdateAsync(model);
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(Guid key) {
            var model = await _context.AppUsers.FirstOrDefaultAsync(item => item.Id == key);

            _context.AppUsers.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CountriesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Countries
                         orderby i.CountryName
                         where i.IsEaes ==1
                         select new {
                             Value = i.Id,
                             Text = i.CountryName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> BanksLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Banks
                         orderby i.Name
                         select new {
                             Value = i.Id,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> ContractsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Contracts
                         orderby i.DocNum
                         select new {
                             Value = i.RowId,
                             Text = i.DocNum
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(AgregatorUser model, IDictionary values) {
            string ID = nameof(AgregatorUser.Id);
            string NAME = nameof(AgregatorUser.Name);
            string SURNAME = nameof(AgregatorUser.Surname);
            string MIDDLENAME = nameof(AgregatorUser.Middlename);
            string BIRTHDAY = nameof(AgregatorUser.Birthday);
          //  string PLACE_OF_BIRTH = nameof(AgregatorUser.PlaceOfBirth);
            string PROFILE_READY = nameof(AgregatorUser.ProfileReady);
            string IDENTITY_READY = nameof(AgregatorUser.IdentityReady);
            string MALE = nameof(AgregatorUser.Male);
            string CITIZENSHIP = nameof(AgregatorUser.CountryCitizenshipId);
            string INN = nameof(AgregatorUser.INN);
            //string DOC_NAME = nameof(AgregatorUser.DocName);
            //string DOC_SERIE = nameof(AgregatorUser.DocSerie);
            //string DOC_NUM = nameof(AgregatorUser.DocNum);
            //string DOC_DATE = nameof(AgregatorUser.DocDate);
            //string DOC_ISSUER_FULL_NAME = nameof(AgregatorUser.DocIssuerFullName);
            //string DOC_ISSUER_CODE = nameof(AgregatorUser.DocIssuerCode);
            //string ADDRESS = nameof(AgregatorUser.Address);
            string ACCOUNT = nameof(AgregatorUser.Account);
            //string ZIP = nameof(AgregatorUser.Zip);
            string BANK_ID = nameof(AgregatorUser.BankId);

            if(values.Contains(ID)) {
                model.Id = new Guid(Convert.ToString(values[ID]));
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(SURNAME)) {
                model.Surname = Convert.ToString(values[SURNAME]);
            }

            if(values.Contains(MIDDLENAME)) {
                model.Middlename = Convert.ToString(values[MIDDLENAME]);
            }

            if(values.Contains(BIRTHDAY)) {
                model.Birthday = values[BIRTHDAY] != null ? Convert.ToDateTime(values[BIRTHDAY]) : (DateTime?)null;
            }
            /*
            if(values.Contains(PLACE_OF_BIRTH)) {
                model.PlaceOfBirth = Convert.ToString(values[PLACE_OF_BIRTH]);
            }
            */
            if(values.Contains(PROFILE_READY)) {
                model.ProfileReady = Convert.ToBoolean(values[PROFILE_READY]);
            }

            if(values.Contains(IDENTITY_READY)) {
                model.IdentityReady = Convert.ToBoolean(values[IDENTITY_READY]);
            }

            if(values.Contains(MALE)) {
                model.Male = values[MALE] != null ? Convert.ToBoolean(values[MALE]) : (bool?)null;
            }

            if(values.Contains(CITIZENSHIP)) {
                model.CountryCitizenshipId = values[CITIZENSHIP]==null? (int?)null: Convert.ToInt32(values[CITIZENSHIP]);
            }

            if(values.Contains(INN)) {
                model.INN = Convert.ToString(values[INN]);
            }
            /*
            if(values.Contains(DOC_NAME)) {
                model.DocName = Convert.ToString(values[DOC_NAME]);
            }

            if(values.Contains(DOC_SERIE)) {
                model.DocSerie = Convert.ToString(values[DOC_SERIE]);
            }

            if(values.Contains(DOC_NUM)) {
                model.DocNum = Convert.ToString(values[DOC_NUM]);
            }

            if(values.Contains(DOC_DATE)) {
                model.DocDate = values[DOC_DATE] != null ? Convert.ToDateTime(values[DOC_DATE]) : (DateTime?)null;
            }

            if(values.Contains(DOC_ISSUER_FULL_NAME)) {
                model.DocIssuerFullName = Convert.ToString(values[DOC_ISSUER_FULL_NAME]);
            }

            if(values.Contains(DOC_ISSUER_CODE)) {
                model.DocIssuerCode = Convert.ToString(values[DOC_ISSUER_CODE]);
            }

            if(values.Contains(ADDRESS)) {
                model.Address = Convert.ToString(values[ADDRESS]);
            }
            */
            if(values.Contains(ACCOUNT)) {
                model.Account = Convert.ToString(values[ACCOUNT]);
            }
            /*
            if(values.Contains(ZIP)) {
                model.Zip = Convert.ToString(values[ZIP]);
            }
            */
            if(values.Contains(BANK_ID)) {
                model.BankId = values[BANK_ID] != null ? new Guid(Convert.ToString(values[BANK_ID])) : (Guid?)null;
            }

        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}