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
    public class ContractAttachmentsController : Controller
    {

#if DESIGN
        private ApplicationDbContext _context;
        public ContractAttachmentsController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public ContractAttachmentsController(
            ApplicationStoreContext context
#endif
            ) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id, DataSourceLoadOptions loadOptions) {
            var contractattachments = _context.ContractAttachments.Where(f => f.ContractId == id).Select(i => new {
                i.RowId,
                i.ContractId,
                i.RowNum,
                i.WorkId,
                i.Quant,
                i.Price,
                i.Created,
                i.Updated,
                i.UserId
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(contractattachments, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new ContractAttachment();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            valuesDict.Remove(nameof(ContractAttachment.RowId));
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ContractAttachments.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.ContractAttachments.FirstOrDefaultAsync(item => item.RowId == key);
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
            var model = await _context.ContractAttachments.FirstOrDefaultAsync(item => item.RowId == key);

            _context.ContractAttachments.Remove(model);
            await _context.SaveChangesAsync();
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

        [HttpGet]
        public async Task<IActionResult> WorksLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Works
                         orderby i.Name
                         select new {
                             Value = i.RowId,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> UsersLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.AppUsers
                         orderby i.UserName
                         select new
                         {
                             Value = i.Id,
                             Text = i.Name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(ContractAttachment model, IDictionary values) {
            string ROW_ID = nameof(ContractAttachment.RowId);
            string CONTRACT_ID = nameof(ContractAttachment.ContractId);
            string ROW_NUM = nameof(ContractAttachment.RowNum);
            string WORK_ID = nameof(ContractAttachment.WorkId);
            string QUANT = nameof(ContractAttachment.Quant);
            string PRICE = nameof(ContractAttachment.Price);
            string CREATED = nameof(ContractAttachment.Created);
            string UPDATED = nameof(ContractAttachment.Updated);
            string USER_ID = nameof(ContractAttachment.UserId);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(CONTRACT_ID)) {
                model.ContractId = Convert.ToInt32(values[CONTRACT_ID]);
            }

            if(values.Contains(ROW_NUM)) {
                model.RowNum = Convert.ToInt16(values[ROW_NUM]);
            }

            if(values.Contains(WORK_ID)) {
                model.WorkId = Convert.ToInt32(values[WORK_ID]);
            }

            if(values.Contains(QUANT)) {
                model.Quant = values[QUANT] != null ? Convert.ToInt32(values[QUANT]) : (int?)null;
            }

            if(values.Contains(PRICE)) {
                model.Price = values[PRICE] != null ? Convert.ToDecimal(values[PRICE], CultureInfo.InvariantCulture) : (decimal?)null;
            }

            if(values.Contains(CREATED)) {
                model.Created = values[CREATED] != null ? Convert.ToDateTime(values[CREATED]) : (DateTime?)null;
            }

            if(values.Contains(UPDATED)) {
                model.Updated = values[UPDATED] != null ? Convert.ToDateTime(values[UPDATED]) : (DateTime?)null;
            }

            if(values.Contains(USER_ID)) {
                model.UserId = values[USER_ID] != null ? ConvertTo<System.Guid>(values[USER_ID]) : (Guid?)null;
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