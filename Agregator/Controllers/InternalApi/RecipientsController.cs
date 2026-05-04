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
using System.Transactions;

namespace Agregator.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class RecipientsController : Controller
    {
        private ApplicationStoreContext _context;

        public RecipientsController(ApplicationStoreContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {


            var recipientsview = _context.RecipientsView.Select(i => new {
                i.number,
                i.appUserId,
                i.Male,
                i.INN,
                i.Account,
                i.BankId,
                i.Email,
                i.firstName,
                i.lastName,
                i.middleName,
                i.birthDate,
                i.birthPlace,
                i.citizenship,
                i.latinFirstName,
                i.latinLastName,
                i.recipientId,
                i.PhoneNumber,
                i.docSerial,
                i.docNumber,
                i.docDate,
                i.organization,
                i.division,
                i.mgMumber,
                i.mgSerial,
                i.expireDate,
                i.mgDate,
                i.mgExpireDate,
                i.mgOrganization,
                i.mgDivision,
                i.postalCode,
                i.state,
                i.city,
                i.district,
                i.settlement,
                i.street,
                i.house,
                i.building,
                i.construction,
                i.apartment,
                i.countryId,
                i.regPostalCode,
                i.regState,
                i.regCity,
                i.regDistrict,
                i.regSettlement,
                i.regStreet,
                i.regHouse,
                i.regBuilding,
                i.regConstruption,
                i.regApartment,
                i.regCountryId
                
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "number" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(recipientsview, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipient(Guid user_id)
        {
            /*
            try
            {

                if (!await _context.Recipients.AnyAsync(item => item.AppUserId == user_id))
                {

                    var user = await _context.AppUsers.Include(f=>f.CountryCitizenship).SingleOrDefaultAsync(f => f.Id == user_id);

                    await _context.Recipients.AddAsync(new Recipient 
                    { 
                        AppUserId = user_id 
                        , firstName = user.Name
                        ,lastName = user.Surname
                        ,middleName = user.Middlename
                        , citizenship = user.CountryCitizenshipId
                    
                    
                    });


                    await _context.PersonePhones.AddAsync(new PersonePhone
                    {
                        AppUserId = user_id
                       ,
                        typeName = PersonePhoneType.Mobile
                       ,
                        number = user.PhoneNumber
                    });

                    await _context.SaveChangesAsync();
                }

                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            */
            /*                                   
                 var recipientsview = _context.RecipientsView.Select(i => new {
                  i.number,
                  i.appUserId,
                  i.Male,
                  i.INN,
                  i.Account,
                  i.BankId,
                  i.Email,
                  i.firstName,
                  i.lastName,
                  i.middleName,
                  i.birthDate,
                  i.birthPlace,
                  i.citizenship,
                  i.latinFirstName,
                  i.latinLastName,
                  i.recipientId,
                  i.PhoneNumber,
                  i.docSerial,
                  i.docNumber,
                  i.docDate,
                  i.organization,
                  i.division,
                  i.mgMumber,
                  i.mgSerial,
                  i.expireDate,
                  i.mgDate,
                  i.mgExpireDate,
                  i.mgOrganization,
                  i.mgDivision,
                  i.postalCode,
                  i.state,
                  i.city,
                  i.district,
                  i.settlement,
                  i.street,
                  i.house,
                  i.building,
                  i.construction,
                  i.apartment,
                  i.countryId,
                  i.regPostalCode,
                  i.regState,
                  i.regCity,
                  i.regDistrict,
                  i.regSettlement,
                  i.regStreet,
                  i.regHouse,
                  i.regBuilding,
                  i.regConstruption,
                  i.regApartment,
                  i.regCountryId
              });


              return Json(await recipientsview.SingleAsync(f => f.appUserId == user_id));
            
            await _context.Recipients.FirstAsync();
            await _context.PersoneAddresses.FirstAsync(); 
            await _context.PersoneDocuments.FirstAsync(); 
            await _context.PersonePhones.FirstOrDefaultAsync();
            */

           


            return Json(await _context.RecipientsView.FirstOrDefaultAsync(f=>f.appUserId==user_id));

        }
/*
        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Frecipient();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.RecipientsView.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.number });
        }
*/

        [HttpPut]
        public async Task<IActionResult> Put(Guid key, string values) 
        {

            using (var tx = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions()
                {
                    IsolationLevel = IsolationLevel.Serializable
                    ,
                    Timeout = TimeSpan.FromSeconds(10)
                }, TransactionScopeAsyncFlowOption.Enabled))
            {


                /*
                Console.WriteLine("{0} => {1}", User.Identity.Name, User.Claims.Count());
                foreach (var x in User.Claims)
                    Console.WriteLine("{0} => {1}",x.Type,x.Value);
                */
                if (key == Guid.Empty)
                    key = Guid.Parse(User.Claims.Single(f => f.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
          //      Console.WriteLine("{0} => {1}", key, values);


                var r = await _context.Recipients.FirstOrDefaultAsync(item => item.AppUserId == key);

                if (r == default(Recipient))
                {
                    var u = await _context.Users.SingleAsync(f => f.Id == key);
                    await _context.Recipients.AddAsync(r = new Recipient
                    {
                        AppUserId = key
                        ,
                        firstName = u.Name
                        ,
                        lastName = u.Surname
                        ,
                        middleName = u.Surname
                        ,
                        citizenship = u.CountryCitizenshipId
                        ,
                        birthDate = u.Birthday
                    });
                    await _context.PersoneAddresses.AddAsync(new PersoneAddress 
                    { 
                        AppUserId = key
                        , typeName = PersoneAddressType.Residence 
                        , countryId = (int) Countries.RUS
                    });
                    await _context.PersoneAddresses.AddAsync(new PersoneAddress 
                    { 
                        AppUserId = key
                        , typeName = PersoneAddressType.Registration
                        , countryId = (int) Countries.RUS
                    
                    });
                    await _context.PersonePhones.AddAsync(new PersonePhone
                    {
                        AppUserId = key
                        ,
                        typeName = PersonePhoneType.Mobile
                        ,
                        number = u.PhoneNumber
                    });

                    await _context.SaveChangesAsync();

                }

                var a1 = await _context.PersoneAddresses.FirstOrDefaultAsync(item => item.AppUserId == key && item.typeName == PersoneAddressType.Residence);
                var a2 = await _context.PersoneAddresses.FirstOrDefaultAsync(item => item.AppUserId == key && item.typeName == PersoneAddressType.Registration);
                var t1 = await _context.PersonePhones.FirstOrDefaultAsync(item => item.AppUserId == key && item.typeName == PersonePhoneType.Mobile);


                var docType = (r.citizenship == (int)Countries.RUS ? PersoneDocumentType.Passport : PersoneDocumentType.InternationalPassport);

                var d1 = await _context.PersoneDocuments.FirstOrDefaultAsync(item => item.AppUserId == key && item.typeName == docType);

                if (d1 == default(PersoneDocument))
                {
                    await _context.PersoneDocuments.AddAsync(d1 = new PersoneDocument { AppUserId = key, typeName = docType });
                }

                var d2 = await _context.PersoneDocuments.FirstOrDefaultAsync(item => item.AppUserId == key && item.typeName == PersoneDocumentType.MigrationCard);

                if (d2 == default(PersoneDocument) && d1.typeName == PersoneDocumentType.InternationalPassport)
                {
                    await _context.PersoneDocuments.AddAsync(d2 = new PersoneDocument { AppUserId = key, typeName = PersoneDocumentType.MigrationCard });
                }

                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(r, d1, d2, a1, a2, t1, valuesDict);

             //   if (!TryValidateModel(r))
             //       return BadRequest(GetFullErrorMessage(ModelState));

                await _context.SaveChangesAsync();

                tx.Complete();
                

                return Ok();

            }

        }


        /*        
                [HttpDelete]
                public async Task Delete(int key) {
                    var model = await _context.RecipientsView.FirstOrDefaultAsync(item => item.number == key);

                    _context.RecipientsView.Remove(model);
                    await _context.SaveChangesAsync();
                }
                */

        private void PopulateModel(
            Recipient model
            , PersoneDocument d1
            , PersoneDocument d2
            , PersoneAddress a1
            , PersoneAddress a2
            , PersonePhone t1
            , IDictionary values)
        {
            //    string NUMBER = nameof(Frecipient.number);
            //    string APP_USER_ID = nameof(Frecipient.appUserId);
            string MALE = nameof(Frecipient.Male);
            string INN = nameof(Frecipient.INN);
            string ACCOUNT = nameof(Frecipient.Account);
            string BANK_ID = nameof(Frecipient.BankId);
            string EMAIL = nameof(Frecipient.Email);
            string FIRST_NAME = nameof(Frecipient.firstName);
            string LAST_NAME = nameof(Frecipient.lastName);
            string MIDDLE_NAME = nameof(Frecipient.middleName);
            string BIRTH_DATE = nameof(Frecipient.birthDate);

            string BIRTH_PLACE = nameof(Frecipient.birthPlace);
            string CITIZENSHIP = nameof(Frecipient.citizenship);
            string LATIN_FIRST_NAME = nameof(Frecipient.latinFirstName);
            string LATIN_LAST_NAME = nameof(Frecipient.latinLastName);
            string RECIPIENT_ID = nameof(Frecipient.recipientId);
            string PHONE_NUMBER = nameof(Frecipient.PhoneNumber);
            string DOC_SERIAL = nameof(Frecipient.docSerial);
            string DOC_NUMBER = nameof(Frecipient.docNumber);
            string DOC_DATE = nameof(Frecipient.docDate);
            string ORGANIZATION = nameof(Frecipient.organization);
            string DIVISION = nameof(Frecipient.division);
            string MG_MUMBER = nameof(Frecipient.mgMumber);
            string MG_SERIAL = nameof(Frecipient.mgSerial);
            string EXPIRE_DATE = nameof(Frecipient.expireDate);

            string MG_DATE = nameof(Frecipient.mgDate);
            string MG_EXPIRE_DATE = nameof(Frecipient.mgExpireDate);
            string MG_ORGANIZATION = nameof(Frecipient.mgOrganization);
            string MG_DIVISION = nameof(Frecipient.mgDivision);

            /*
            string POSTAL_CODE = nameof(Frecipient.postalCode);
            string STATE = nameof(Frecipient.state);
            string CITY = nameof(Frecipient.city);
            string DISTRICT = nameof(Frecipient.district);
            string SETTLEMENT = nameof(Frecipient.settlement);
            string STREET = nameof(Frecipient.street);
            string HOUSE = nameof(Frecipient.house);
            string BUILDING = nameof(Frecipient.building);
            string CONSTRUCTION = nameof(Frecipient.construction);
            string APARTMENT = nameof(Frecipient.apartment);
            string COUNTRY_ID = nameof(Frecipient.countryId);
            */
            
            string REG_POSTAL_CODE = nameof(Frecipient.regPostalCode);
            string REG_STATE = nameof(Frecipient.regState);
            string REG_CITY = nameof(Frecipient.regCity);
            string REG_DISTRICT = nameof(Frecipient.regDistrict);
            string REG_SETTLEMENT = nameof(Frecipient.regSettlement);
            string REG_STREET = nameof(Frecipient.regStreet);
            string REG_HOUSE = nameof(Frecipient.regHouse);
            string REG_BUILDING = nameof(Frecipient.regBuilding);
            string REG_CONSTRUPTION = nameof(Frecipient.regConstruption);
            string REG_APARTMENT = nameof(Frecipient.regApartment);
            string REG_COUNTRY_ID = nameof(Frecipient.regCountryId);
            
            /*
            if(values.Contains(NUMBER)) {
                model.number = Convert.ToInt32(values[NUMBER]);
            }
            
            if(values.Contains(APP_USER_ID)) {
                model.AppUserId = ConvertTo<System.Guid>(values[APP_USER_ID]);
            }
            */

            if (values.Contains(MALE))
            {
                model.AppUser.Male = values[MALE] != null ? Convert.ToBoolean(values[MALE]) : (bool?)null;
            }

            if (values.Contains(INN))
            {
                model.AppUser.INN = Convert.ToString(values[INN]);
            }

            if (values.Contains(ACCOUNT))
            {
                model.AppUser.Account = Convert.ToString(values[ACCOUNT]);
            }

            if (values.Contains(BANK_ID))
            {
                model.AppUser.BankId = values[BANK_ID] != null ? ConvertTo<System.Guid>(values[BANK_ID]) : (Guid?)null;
            }

            if (values.Contains(EMAIL))
            {
                model.AppUser.Email = Convert.ToString(values[EMAIL]);
            }

            if (values.Contains(FIRST_NAME))
            {
                model.firstName = Convert.ToString(values[FIRST_NAME]);
            }

            if (values.Contains(LAST_NAME))
            {
                model.lastName = Convert.ToString(values[LAST_NAME]);
            }

            if (values.Contains(MIDDLE_NAME))
            {
                model.middleName = Convert.ToString(values[MIDDLE_NAME]);
            }

            if (values.Contains(BIRTH_DATE))
            {
                model.birthDate = values[BIRTH_DATE] == null ?
                    (DateTime?)null
                    :
                DateTime.ParseExact(Convert.ToString(values[BIRTH_DATE]).Substring(0, 33), "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
            }

            if (values.Contains(BIRTH_PLACE))
            {
                model.birthPlace = Convert.ToString(values[BIRTH_PLACE]);
            }

            if (values.Contains(CITIZENSHIP))
            {
                model.citizenship = values[CITIZENSHIP]==null? (int?)null: Convert.ToInt32(values[CITIZENSHIP]);
            }

            if (values.Contains(LATIN_FIRST_NAME))
            {
                model.latinFirstName = Convert.ToString(values[LATIN_FIRST_NAME]);
            }

            if (values.Contains(LATIN_LAST_NAME))
            {
                model.latinLastName = Convert.ToString(values[LATIN_LAST_NAME]);
            }

            if (values.Contains(RECIPIENT_ID))
            {
                model.recipientId = values[RECIPIENT_ID] != null ?  Convert.ToInt32(values[RECIPIENT_ID]) : (int?) null;
            }
            
            if(values.Contains(PHONE_NUMBER)) {
                t1.number = Convert.ToString(values[PHONE_NUMBER]);
            }
            
            if (values.Contains(DOC_SERIAL))
            {
                d1.serial = Convert.ToString(values[DOC_SERIAL]);
            }

            if (values.Contains(DOC_NUMBER))
            {
                d1.number = Convert.ToString(values[DOC_NUMBER]);
            }

            if (values.Contains(DOC_DATE))
            {
                d1.date = values[DOC_DATE] == null ? (DateTime?)null
                    :
                    DateTime.ParseExact(Convert.ToString(values[DOC_DATE]).Substring(0, 33), "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
            }

            if (values.Contains(ORGANIZATION))
            {
                d1.organization = Convert.ToString(values[ORGANIZATION]);
            }

            if (values.Contains(DIVISION))
            {
                d1.division = Convert.ToString(values[DIVISION]);
            }

            if (values.Contains(EXPIRE_DATE))
            {
                d1.expireDate = values[EXPIRE_DATE] == null ? (DateTime?)null :
                    DateTime.ParseExact(Convert.ToString(values[EXPIRE_DATE]).Substring(0, 33), "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
                d1.organization = _context.Countries.Single(f => f.Id == model.citizenship.Value).CountryName;

            }



            if (values.Contains(MG_MUMBER))
            {
                d2.number = Convert.ToString(values[MG_MUMBER]);
            }

            if (values.Contains(MG_SERIAL))
            {
                d2.serial = Convert.ToString(values[MG_SERIAL]);
            }


            if (values.Contains(MG_DATE))
            {
                d2.date = values[MG_DATE] == null ?  (DateTime?)null
                    :
                DateTime.ParseExact(Convert.ToString(values[MG_DATE]).Substring(0, 33), "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture)
                ;

                d2.expireDate = d2.date + new TimeSpan(5*365,0,0,0);
                d2.organization = d1.organization??_context.Countries.Single(f => f.Id == model.citizenship.Value).CountryName;
            }

            if (values.Contains(MG_EXPIRE_DATE))
            {
                d2.expireDate = values[MG_EXPIRE_DATE] == null ?  (DateTime?)null
                    :
                DateTime.ParseExact(Convert.ToString(values[MG_EXPIRE_DATE]).Substring(0, 33), "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture);
            }

            if (values.Contains(MG_ORGANIZATION))
            {
                d2.organization = Convert.ToString(values[MG_ORGANIZATION]);
            }

            if (values.Contains(MG_DIVISION))
            {
                d2.division = Convert.ToString(values[MG_DIVISION]);
            }



            if (values.Contains(REG_POSTAL_CODE))
            {
                a1.postalCode = Convert.ToString(values[REG_POSTAL_CODE]);
                a2.postalCode = Convert.ToString(values[REG_POSTAL_CODE]);
            }

            if (values.Contains(REG_STATE))
            {
                a1.state = Convert.ToString(values[REG_STATE]);
                a2.state = Convert.ToString(values[REG_STATE]);

            }

            if (values.Contains(REG_CITY))
            {
                a1.city = Convert.ToString(values[REG_CITY]);
                a2.city = Convert.ToString(values[REG_CITY]);
            }

            if (values.Contains(REG_DISTRICT))
            {
                a1.district = Convert.ToString(values[REG_DISTRICT]);
                a2.district = Convert.ToString(values[REG_DISTRICT]);
            }

            if (values.Contains(REG_SETTLEMENT))
            {
                a1.settlement = Convert.ToString(values[REG_SETTLEMENT]);
                a2.settlement = Convert.ToString(values[REG_SETTLEMENT]);
            }

            if (values.Contains(REG_STREET))
            {
                a1.street = Convert.ToString(values[REG_STREET]);
                a2.street = Convert.ToString(values[REG_STREET]);
            }

            if (values.Contains(REG_HOUSE))
            {
                a1.house = Convert.ToString(values[REG_HOUSE]);
                a2.house = Convert.ToString(values[REG_HOUSE]);
            }

            if (values.Contains(REG_BUILDING))
            {
                a1.building = Convert.ToString(values[REG_BUILDING]);
                a2.building = Convert.ToString(values[REG_BUILDING]);
            }

            if (values.Contains(REG_CONSTRUPTION))
            {
                a1.construction = Convert.ToString(values[REG_CONSTRUPTION]);
                a2.construction = Convert.ToString(values[REG_CONSTRUPTION]);

            }

            if (values.Contains(REG_APARTMENT))
            {
                a1.apartment = Convert.ToString(values[REG_APARTMENT]);
                a2.apartment = Convert.ToString(values[REG_APARTMENT]);
            }

            if (values.Contains(REG_COUNTRY_ID))
            {
                a1.countryId = values[REG_COUNTRY_ID]==null? (int?)null :Convert.ToInt32(values[REG_COUNTRY_ID]);
                a2.countryId = values[REG_COUNTRY_ID] == null ? (int?)null : Convert.ToInt32(values[REG_COUNTRY_ID]);
            }

            /*
            if(values.Contains(REG_POSTAL_CODE)) {
                model.regPostalCode = Convert.ToString(values[REG_POSTAL_CODE]);
            }

            if(values.Contains(REG_STATE)) {
                model.regState = Convert.ToString(values[REG_STATE]);
            }

            if(values.Contains(REG_CITY)) {
                model.regCity = Convert.ToString(values[REG_CITY]);
            }

            if(values.Contains(REG_DISTRICT)) {
                model.regDistrict = Convert.ToString(values[REG_DISTRICT]);
            }

            if(values.Contains(REG_SETTLEMENT)) {
                model.regSettlement = Convert.ToString(values[REG_SETTLEMENT]);
            }

            if(values.Contains(REG_STREET)) {
                model.regStreet = Convert.ToString(values[REG_STREET]);
            }

            if(values.Contains(REG_HOUSE)) {
                model.regHouse = Convert.ToString(values[REG_HOUSE]);
            }

            if(values.Contains(REG_BUILDING)) {
                model.regBuilding = Convert.ToString(values[REG_BUILDING]);
            }

            if(values.Contains(REG_CONSTRUPTION)) {
                model.regConstruption = Convert.ToString(values[REG_CONSTRUPTION]);
            }

            if(values.Contains(REG_APARTMENT)) {
                model.regApartment = Convert.ToString(values[REG_APARTMENT]);
            }

            if(values.Contains(REG_COUNTRY_ID)) {
                model.regCountryId = Convert.ToInt32(values[REG_COUNTRY_ID]);
            }
            */
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