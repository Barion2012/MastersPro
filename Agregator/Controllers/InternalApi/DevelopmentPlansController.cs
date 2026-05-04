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


namespace Agregator.Controllers
{
    using Data;

    [Route("api/internal/[controller]/[action]")]
    public class DevelopmentPlansController : Controller
    {

#if DESIGN
        private ApplicationDbContext _context;
        public DevelopmentPlansController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public DevelopmentPlansController(
            ApplicationStoreContext context
#endif
            ) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var developmentplan = _context.DevelopmentPlan.Select(i => new {
                i.RowId,
                i.ParentId,
                i.Title,
                i.Description,
                i.PlanDate,
                i.FactDate,
                i.WhoResponsible,
                i.Created
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(developmentplan, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new DevelopmentPlan();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            valuesDict.Remove("RowId");
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.DevelopmentPlan.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.DevelopmentPlan.FirstOrDefaultAsync(item => item.RowId == key);
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
            var model = await _context.DevelopmentPlan.FirstOrDefaultAsync(item => item.RowId == key);

            _context.DevelopmentPlan.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> AspNetUsersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.AppUsers
                         orderby i.UserName
                         select new {
                             Value = i.Id,
                             Text = i.UserName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(DevelopmentPlan model, IDictionary values) {
            string ROW_ID = nameof(DevelopmentPlan.RowId);
            string PARENT_ID = nameof(DevelopmentPlan.ParentId);
            string TITLE = nameof(DevelopmentPlan.Title);
            string DESCRIPTION = nameof(DevelopmentPlan.Description);
            string PLAN_DATE = nameof(DevelopmentPlan.PlanDate);
            string FACT_DATE = nameof(DevelopmentPlan.FactDate);
            string WHO_RESPONSIBLE = nameof(DevelopmentPlan.WhoResponsible);
            string CREATED = nameof(DevelopmentPlan.Created);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(PARENT_ID)) {
                model.ParentId = values[PARENT_ID] != null ? Convert.ToInt32(values[PARENT_ID]) : (int?)null;
            }

            if(values.Contains(TITLE)) {
                model.Title = Convert.ToString(values[TITLE]);
            }

            if(values.Contains(DESCRIPTION)) {
                model.Description = Convert.ToString(values[DESCRIPTION]);
            }

            if(values.Contains(PLAN_DATE)) {
                model.PlanDate = values[PLAN_DATE] != null ? Convert.ToDateTime(values[PLAN_DATE]) : (DateTime?)null;
            }

            if(values.Contains(FACT_DATE)) {
                model.FactDate = values[FACT_DATE] != null ? Convert.ToDateTime(values[FACT_DATE]) : (DateTime?)null;
            }

            if(values.Contains(WHO_RESPONSIBLE)) {
                model.WhoResponsible = ConvertTo<System.Guid>(values[WHO_RESPONSIBLE]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }
        }

        private T ConvertTo<T>(object value) {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if(converter != null) {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            } else {
                // If necessary, implement a type conversion here
                throw new NotImplementedException();
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