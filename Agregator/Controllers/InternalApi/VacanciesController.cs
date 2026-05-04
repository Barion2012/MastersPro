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

namespace Agregator.Controllers
{
    [Route("api/manager/[controller]/[action]")]
    public class VacanciesController : Controller
    {


#if DESIGN
        private ApplicationDbContext _context;
        public VacanciesController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public VacanciesController(
            ApplicationStoreContext context
#endif
		)
	{
            _context = context;
        }

        [HttpGet]
        public async Task<object> Get(DataSourceLoadOptions loadOptions) {
            var vacancies = _context.Vacancies.Include(f=>f.VacancyBuildObjectCts).Select(i => new {
                i.RowId,
                i.Position,
                i.BillingTypeIdValue,
                i.BillingTypeIdView,
                i.AreaIdValue,
                i.AreaIdView,
                i.Code,
      //          i.Name,
      //          i.Description,
                i.SalaryCurrencyValue,
                i.SalaryCurrencyView,
                i.SpecializationsIdValue,
                i.SpecializationsIdView,
                i.TypeIdValue,
                i.TypeIdView,
                i.KeySkillsNameValue,
                i.KeySkillsNameView,
                i.SalaryFrom,
                i.SalaryTo,
                i.AddressShowMetroOnly,
                i.DescriptionTab1,
                i.DescriptionTab2,
                i.DescriptionTab3,
                objects=i.VacancyBuildObjectCts.Select(f=>f.Object).ToArray()
            }).AsSingleQuery();

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(vacancies, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> GetObjects( DataSourceLoadOptions loadOptions)
        {
            var vacancie2obj = _context.BuildObjects.Select(i => new
            {
                Value=i.RowId,
                Text=i.Name
            });
            
            return Json(await DataSourceLoader.LoadAsync(vacancie2obj, loadOptions));
        }

    [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Vacancy();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Vacancies.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Vacancies.FirstOrDefaultAsync(item => item.RowId == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (model.objects != null)
            {
                foreach (var x in model.objects)
                {
                    if (!await _context.VacancyBuildObjectCts.AnyAsync(f => f.Vacancy == key && f.Object == x))
                        await _context.VacancyBuildObjectCts.AddAsync(new VacancyBuildObjectCt { Object = x, Vacancy = key });
                }

                foreach (var ent in await _context.VacancyBuildObjectCts.Where(f => f.Vacancy == key)/*.Select(f=>f.Object)*/.ToListAsync())
                {

                    if (!model.objects.Any(f => f == ent.Object))
                        _context.VacancyBuildObjectCts.Remove(ent);
                }
            }

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.Vacancies.FirstOrDefaultAsync(item => item.RowId == key);

            _context.Vacancies.Remove(model);
            await _context.SaveChangesAsync();
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

        private void PopulateModel(Vacancy model, IDictionary values) {
            string ROW_ID = nameof(Vacancy.RowId);
            string POSITION = nameof(Vacancy.Position);
            string BILLING_TYPE_ID_VALUE = nameof(Vacancy.BillingTypeIdValue);
            string BILLING_TYPE_ID_VIEW = nameof(Vacancy.BillingTypeIdView);
            string AREA_ID_VALUE = nameof(Vacancy.AreaIdValue);
            string AREA_ID_VIEW = nameof(Vacancy.AreaIdView);
            string CODE = nameof(Vacancy.Code);
       //     string NAME = nameof(Vacancy.Name);
       //     string DESCRIPTION = nameof(Vacancy.Description);
            string SALARY_CURRENCY_VALUE = nameof(Vacancy.SalaryCurrencyValue);
            string SALARY_CURRENCY_VIEW = nameof(Vacancy.SalaryCurrencyView);
            string SPECIALIZATIONS_ID_VALUE = nameof(Vacancy.SpecializationsIdValue);
            string SPECIALIZATIONS_ID_VIEW = nameof(Vacancy.SpecializationsIdView);
            string TYPE_ID_VALUE = nameof(Vacancy.TypeIdValue);
            string TYPE_ID_VIEW = nameof(Vacancy.TypeIdView);
            string KEY_SKILLS_NAME_VALUE = nameof(Vacancy.KeySkillsNameValue);
            string KEY_SKILLS_NAME_VIEW = nameof(Vacancy.KeySkillsNameView);
            string SALARY_FROM = nameof(Vacancy.SalaryFrom);
            string SALARY_TO = nameof(Vacancy.SalaryTo);
            string ADDRESS_SHOW_METRO_ONLY = nameof(Vacancy.AddressShowMetroOnly);
            string DESCRIPTION1 = nameof(Vacancy.DescriptionTab1);
            string DESCRIPTION2 = nameof(Vacancy.DescriptionTab2);
            string DESCRIPTION3 = nameof(Vacancy.DescriptionTab3);
            string OBJECTS = nameof(Vacancy.objects);

            if (values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(POSITION)) {
                model.Position = Convert.ToInt32(values[POSITION]);
            }

            if(values.Contains(BILLING_TYPE_ID_VALUE)) {
                model.BillingTypeIdValue = Convert.ToString(values[BILLING_TYPE_ID_VALUE]);
            }

            if(values.Contains(BILLING_TYPE_ID_VIEW)) {
                model.BillingTypeIdView = Convert.ToString(values[BILLING_TYPE_ID_VIEW]);
            }

            if(values.Contains(AREA_ID_VALUE)) {
                model.AreaIdValue = Convert.ToString(values[AREA_ID_VALUE]);
            }

            if(values.Contains(AREA_ID_VIEW)) {
                model.AreaIdView = Convert.ToString(values[AREA_ID_VIEW]);
            }

            if(values.Contains(CODE)) {
                model.Code = Convert.ToString(values[CODE]);
            }
            /*
            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }
            
            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }
            */
            if (values.Contains(DESCRIPTION1))
            {
                model.DescriptionTab1 = Convert.ToString(values[DESCRIPTION1]);
            }
            if (values.Contains(DESCRIPTION2))
            {
                model.DescriptionTab2 = Convert.ToString(values[DESCRIPTION2]);
            }
            if (values.Contains(DESCRIPTION3))
            {
                model.DescriptionTab3 = Convert.ToString(values[DESCRIPTION3]);
            }

            if (values.Contains(SALARY_CURRENCY_VALUE)) {
                model.SalaryCurrencyValue = Convert.ToString(values[SALARY_CURRENCY_VALUE]);
            }

            if(values.Contains(SALARY_CURRENCY_VIEW)) {
                model.SalaryCurrencyView = Convert.ToString(values[SALARY_CURRENCY_VIEW]);
            }

            if(values.Contains(SPECIALIZATIONS_ID_VALUE)) {
                model.SpecializationsIdValue = Convert.ToString(values[SPECIALIZATIONS_ID_VALUE]);
            }

            if(values.Contains(SPECIALIZATIONS_ID_VIEW)) {
                model.SpecializationsIdView = Convert.ToString(values[SPECIALIZATIONS_ID_VIEW]);
            }

            if(values.Contains(TYPE_ID_VALUE)) {
                model.TypeIdValue = Convert.ToString(values[TYPE_ID_VALUE]);
            }

            if(values.Contains(TYPE_ID_VIEW)) {
                model.TypeIdView = Convert.ToString(values[TYPE_ID_VIEW]);
            }

            if(values.Contains(KEY_SKILLS_NAME_VALUE)) {
                model.KeySkillsNameValue = Convert.ToString(values[KEY_SKILLS_NAME_VALUE]);
            }

            if(values.Contains(KEY_SKILLS_NAME_VIEW)) {
                model.KeySkillsNameView = Convert.ToString(values[KEY_SKILLS_NAME_VIEW]);
            }

            if(values.Contains(SALARY_FROM)) {
                model.SalaryFrom = values[SALARY_FROM] != null ? Convert.ToDecimal(values[SALARY_FROM], CultureInfo.InvariantCulture) : (decimal?)null;
            }

            if(values.Contains(SALARY_TO)) {
                model.SalaryTo = values[SALARY_TO] != null ? Convert.ToDecimal(values[SALARY_TO], CultureInfo.InvariantCulture) : (decimal?)null;
            }

            if(values.Contains(ADDRESS_SHOW_METRO_ONLY)) {
                model.AddressShowMetroOnly = values[ADDRESS_SHOW_METRO_ONLY] != null ? Convert.ToBoolean(values[ADDRESS_SHOW_METRO_ONLY]) : (bool?)null;
            }

            if (values.Contains(OBJECTS))
            {
                model.objects = values[OBJECTS]!= null ?  JsonConvert.DeserializeObject<int[]>(values[OBJECTS].ToString()) : new int[] { };
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