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
    [Route("api/internal/[controller]/[action]")]
    public class WorksController : Controller
    {

#if DESIGN
        private ApplicationDbContext _context;
        public WorksController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public WorksController(
            ApplicationStoreContext context
#endif
            )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var works = _context.Works.Select(i => new {
                i.RowId,
                i.Status,
                i.Name,
                i.Created,
                i.Updated,
                i.ParentId
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(works, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Work();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Works.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Works.FirstOrDefaultAsync(item => item.RowId == key);
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
            var model = await _context.Works.FirstOrDefaultAsync(item => item.RowId == key);

            _context.Works.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Work model, IDictionary values) {
            string ROW_ID = nameof(Work.RowId);
            string STATUS = nameof(Work.Status);
            string NAME = nameof(Work.Name);
            string CREATED = nameof(Work.Created);
            string UPDATED = nameof(Work.Updated);
            string PARENT_ID = nameof(Work.ParentId);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToSByte(values[STATUS]);
            }

            if(values.Contains(NAME)) {
                model.Name = Convert.ToString(values[NAME]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }

            if(values.Contains(UPDATED)) {
                model.Updated = values[UPDATED] != null ? Convert.ToDateTime(values[UPDATED]) : (DateTime?)null;
            }

            if(values.Contains(PARENT_ID)) {
                model.ParentId = values[PARENT_ID] != null ? Convert.ToInt32(values[PARENT_ID]) : (int?)null;
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