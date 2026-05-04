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
    [Route("api/[controller]/[action]")]
    public class FeedbacksController : Controller
    {
        private ApplicationStoreContext _context;

        public FeedbacksController(ApplicationStoreContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var feedbacks = _context.Feedbacks.Include(f => f.VacancyNavigation).ThenInclude(f => f.PositionNavigation).Join(_context.RecipientsView, p => p.UserId, f => f.appUserId, (p, f) => new
            {
                //                f.appUserId,


                p.RowId,
                p.UserId,
                //                i.User.Email,
                p.Vacancy,

                p.Status,
                p.Created,
                p.Updated
                           ,
                User = new { f.Email }
                           ,
                VacancyNavigation = new { PositionNavigation = new { Name = p.VacancyNavigation.PositionNavigation.Name } }
                           ,
                Recipient = new
                {
                    f.citizenship
                           ,
                    f.lastName
                }


            });
            return Json(await DataSourceLoader.LoadAsync(feedbacks, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Feedback();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Feedbacks.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.RowId == key);
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
            var model = await _context.Feedbacks.FirstOrDefaultAsync(item => item.RowId == key);

            _context.Feedbacks.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> AppUsersLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.AppUsers
                         orderby i.Name
                         select new {
                             Value = i.Id,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> FeedBackStatusLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.FeedBackStatus
                         orderby i.Name
                         select new {
                             Value = i.RowId,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> VacanciesLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Vacancies
                         orderby i.BillingTypeIdValue
                         select new {
                             Value = i.RowId,
                             Text = i.BillingTypeIdValue
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Feedback model, IDictionary values) {
            string ROW_ID = nameof(Feedback.RowId);
            string USER_ID = nameof(Feedback.UserId);
            string VACANCY = nameof(Feedback.Vacancy);
            string STATUS = nameof(Feedback.Status);
            string CREATED = nameof(Feedback.Created);
            string UPDATED = nameof(Feedback.Updated);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(USER_ID)) {
                model.UserId = ConvertTo<System.Guid>(values[USER_ID]);
            }

            if(values.Contains(VACANCY)) {
                model.Vacancy = Convert.ToInt32(values[VACANCY]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToInt32(values[STATUS]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }

            if(values.Contains(UPDATED)) {
                model.Updated = values[UPDATED] != null ? Convert.ToDateTime(values[UPDATED]) : (DateTime?)null;
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