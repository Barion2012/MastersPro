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
using Microsoft.AspNetCore.Authorization;

namespace Agregator.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class ApplicantsController : Controller
    {

#if DESIGN
        private ApplicationDbContext _context;
        public ApplicantsController(
            ApplicationDbContext context
#else

        private ApplicationStoreContext _context;
        public ApplicantsController(
            ApplicationStoreContext context
#endif
            ) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var applicants = _context.Applicants.Select(i => new {
                i.RowId,
                i.FirstName,
                i.LastName,
                i.MiddleName,
                i.BirthDate,
                i.BirthPlace,
                i.Male,
                i.CitizenshipId,
                i.ActualPlace,
                i.Education,
                i.MarriedStatus,
                i.DeviceNumber,
                i.ProfessionId,
                i.Skils,
                i.Tools,
                i.Hostel,
                i.Agreement,
                i.Сonfirmation,
                i.Created,
                i.Creator
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(applicants, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Applicant();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Applicants.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Applicants.FirstOrDefaultAsync(item => item.RowId == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.Applicants.FirstOrDefaultAsync(item => item.RowId == key);

            _context.Applicants.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CountriesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Countries
                         orderby i.CountryName
                         select new {
                             Value = i.Id,
                             Text = i.CountryName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> PositionsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Positions
                         orderby i.Name
                         select new {
                             Value = i.RowId,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Applicant model, IDictionary values) {
            string ROW_ID = nameof(Applicant.RowId);
            string FIRST_NAME = nameof(Applicant.FirstName);
            string LAST_NAME = nameof(Applicant.LastName);
            string MIDDLE_NAME = nameof(Applicant.MiddleName);
            string BIRTH_DATE = nameof(Applicant.BirthDate);
            string BIRTH_PLACE = nameof(Applicant.BirthPlace);
            string MALE = nameof(Applicant.Male);
            string CITIZENSHIP_ID = nameof(Applicant.CitizenshipId);
            string ACTUAL_PLACE = nameof(Applicant.ActualPlace);
            string EDUCATION = nameof(Applicant.Education);
            string MARRIED_STATUS = nameof(Applicant.MarriedStatus);
            string DEVICE_NUMBER = nameof(Applicant.DeviceNumber);
            string PROFESSION_ID = nameof(Applicant.ProfessionId);
            string SKILS = nameof(Applicant.Skils);
            string TOOLS = nameof(Applicant.Tools);
            string HOSTEL = nameof(Applicant.Hostel);
            string AGREEMENT = nameof(Applicant.Agreement);
            string СONFIRMATION = nameof(Applicant.Сonfirmation);
            string CREATED = nameof(Applicant.Created);
            string CREATOR = nameof(Applicant.Creator);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(FIRST_NAME)) {
                model.FirstName = Convert.ToString(values[FIRST_NAME]);
            }

            if(values.Contains(LAST_NAME)) {
                model.LastName = Convert.ToString(values[LAST_NAME]);
            }

            if(values.Contains(MIDDLE_NAME)) {
                model.MiddleName = Convert.ToString(values[MIDDLE_NAME]);
            }

            if(values.Contains(BIRTH_DATE)) {
                model.BirthDate = values[BIRTH_DATE] != null ? Convert.ToDateTime(values[BIRTH_DATE]) : (DateTime?)null;
            }

            if(values.Contains(BIRTH_PLACE)) {
                model.BirthPlace = Convert.ToString(values[BIRTH_PLACE]);
            }

            if(values.Contains(MALE)) {
                model.Male = Convert.ToBoolean(values[MALE]);
            }

            if(values.Contains(CITIZENSHIP_ID)) {
                model.CitizenshipId = values[CITIZENSHIP_ID] != null ? Convert.ToInt32(values[CITIZENSHIP_ID]) : (int?)null;
            }

            if(values.Contains(ACTUAL_PLACE)) {
                model.ActualPlace = Convert.ToString(values[ACTUAL_PLACE]);
            }

            if(values.Contains(EDUCATION)) {
                model.Education = Convert.ToString(values[EDUCATION]);
            }

            if(values.Contains(MARRIED_STATUS)) {
                model.MarriedStatus = Convert.ToString(values[MARRIED_STATUS]);
            }

            if(values.Contains(DEVICE_NUMBER)) {
                model.DeviceNumber = Convert.ToString(values[DEVICE_NUMBER]);
            }

            if(values.Contains(PROFESSION_ID)) {
                model.ProfessionId = values[PROFESSION_ID] != null ? Convert.ToInt32(values[PROFESSION_ID]) : (int?)null;
            }

            if(values.Contains(SKILS)) {
                model.Skils = Convert.ToString(values[SKILS]);
            }

            if(values.Contains(TOOLS)) {
                model.Tools = Convert.ToString(values[TOOLS]);
            }

            if(values.Contains(HOSTEL)) {
                model.Hostel = Convert.ToBoolean(values[HOSTEL]);
            }

            if(values.Contains(AGREEMENT)) {
                model.Agreement = Convert.ToBoolean(values[AGREEMENT]);
            }

            if(values.Contains(СONFIRMATION)) {
                model.Сonfirmation = Convert.ToString(values[СONFIRMATION]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }

            if(values.Contains(CREATOR)) {
                model.Creator = Convert.ToString(values[CREATOR]);
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