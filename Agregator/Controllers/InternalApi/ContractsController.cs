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
using Microsoft.AspNetCore.Authorization;

namespace Agregator.Controllers
{

    using Data;
    [Route("api/manager/[controller]/[action]")]
    public class ContractsController : Controller
    {
        private readonly UserManager<AgregatorUser> _userManager;
#if DESIGN
        private ApplicationDbContext _context;
        public ContractsController(
            ApplicationDbContext context
#else
        private ApplicationStoreContext _context;
        public ContractsController(
            ApplicationStoreContext context
#endif
            ,UserManager<AgregatorUser> userManager
            ) 
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var contracts = _context.Contracts.Include(f=>f.Customer).Select(i => new Contract()
            {
                RowId = i.RowId,
                DocNum = i.DocNum,
                DocDate = i.DocDate,
                Status = i.Status,
                Type = i.Type,
                Created = i.Created,
                SignDate = i.SignDate,
                PaidDate = i.PaidDate,
                Customer = new AgregatorUser { Id = i.Customer.Id, Name = i.Customer.Name, Surname = i.Customer.Surname, Middlename = i.Customer.Middlename },
                CustomerId = i.CustomerId,
                Remark = i.Remark
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "RowId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(contracts, loadOptions));
        }

        [HttpGet]

        public async Task<IActionResult> GetMy(DataSourceLoadOptions loadOptions)
        {
            var me = await _userManager.GetUserAsync(User);

            var contracts = _context.Contracts.Include(f=>f.Customer)
                .Where(f => f.Customer.Id == me.Id)
                                .Select(i => new Contract(){ 
                RowId=i.RowId,
                    DocNum=i.DocNum,
                    DocDate=i.DocDate,
                    Status=i.Status,
                    Type=i.Type,
                    Created=i.Created,
                    SignDate=i.SignDate,
                    PaidDate=i.PaidDate,
                                    Customer = new AgregatorUser {Id=i.Customer.Id,Name=i.Customer.Name, Surname = i.Customer.Surname, Middlename = i.Customer.Middlename},
                    CustomerId=i.CustomerId,
                    Remark=i.Remark
            });

            return Json(await DataSourceLoader.LoadAsync(contracts, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> ContractStatusLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Enums
                         where i.typeId==4
                        orderby i.RowId
                         select new
                         {
                             Value =(ContractStatus) i.RowId,
                             Text = i.name
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> SelfEmployedStatusLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Recipients
                         orderby i.lastName
                         select new
                         {
                             Value = i.AppUserId,
                             Text = (i.accountStatus=="ACTIVE"? i.selfEmployedStatus:i.accountStatus)
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> UsersLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.RecipientsView
                         orderby i.lastName
                         select new
                         {
                             Value = i.appUserId,
                             Text = i.lastName + " " + i.fioName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Contract();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Contracts.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.RowId });
        }


        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Contracts.FirstOrDefaultAsync(item => item.RowId == key);
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
            var model = await _context.Contracts.FirstOrDefaultAsync(item => item.RowId == key);

            _context.Contracts.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Contract model, IDictionary values) {
            string ROW_ID = nameof(Contract.RowId);
            string DOCNUM = nameof(Contract.DocNum);
            string DOCDATE = nameof(Contract.DocDate);
            string STATUS = nameof(Contract.Status);
            string TYPE = nameof(Contract.Type);
            string DOC_URI = nameof(Contract.DocUri);
            string SIGN_URI = nameof(Contract.SignUri);
            string CREATED = nameof(Contract.Created);
            string CUSTOMER_ID = nameof(Contract.CustomerId);
            string REMARK = nameof(Contract.Remark);

            if(values.Contains(ROW_ID)) {
                model.RowId = Convert.ToInt32(values[ROW_ID]);
            }

            if(values.Contains(DOCNUM)) {
                model.DocNum = Convert.ToString(values[DOCNUM]);
            }

            if(values.Contains(DOCDATE)) {
                model.DocDate = Convert.ToDateTime(values[DOCDATE]);
            }

            if(values.Contains(STATUS)) {
                model.Status = (ContractStatus) Convert.ToInt32(values[STATUS]);
            }

            if(values.Contains(TYPE)) {
                model.Type = Convert.ToInt32(values[TYPE]);
            }

            if(values.Contains(DOC_URI)) {
                model.DocUri = Convert.ToString(values[DOC_URI]);
            }

            if(values.Contains(SIGN_URI)) {
                model.SignUri = Convert.ToString(values[SIGN_URI]);
            }

            if(values.Contains(CREATED)) {
                model.Created = Convert.ToDateTime(values[CREATED]);
            }

            if(values.Contains(CUSTOMER_ID)) {
                model.IssuerId = values[CUSTOMER_ID] != null ? new Guid(Convert.ToString(values[CUSTOMER_ID])) : (Guid?)null;
            }

            if(values.Contains(REMARK)) {
                model.Remark = Convert.ToString(values[REMARK]);
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