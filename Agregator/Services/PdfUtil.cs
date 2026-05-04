using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
/*
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.Layout;
using iText.Layout.Element;
using System.Globalization;
using System.Text.Json;
using System.Text;
using iText.Kernel.Geom;
*/
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Agregator.Services
{

    using static Properties.Resources;
    using Data;

    public class PdfUtil
    {
        private readonly IFormatProvider _appFormats;
//        private readonly PdfFont _pdfFont = PdfFontFactory.CreateFont(TIMES, PdfEncodings.IDENTITY_H);
//        private readonly PdfFont _pdfFontBd = PdfFontFactory.CreateFont(TIMESBD, PdfEncodings.IDENTITY_H);

#if DESIGN
        private readonly ApplicationDbContext _dbContext;
        public PdfUtil(
        ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public PdfUtil(
            ApplicationStoreContext dbContext
#endif
        ,IFormatProvider appFormats)
        {
            _appFormats = appFormats;
            _dbContext = dbContext;
        }



        public static async Task<byte[]> MakePrevImg(byte[] pict, float pk = 10)
        {


            using (MemoryStream ret = new MemoryStream())
            using (MemoryStream pm = new MemoryStream(pict, 0, pict.Length))
            {
                pm.Seek(0, SeekOrigin.Begin);
                using (var img = System.Drawing.Image.FromStream(pm))
                using (var bmp = new System.Drawing.Bitmap(Convert.ToInt32(img.Width / pk), Convert.ToInt32(img.Height / pk)))
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                    bmp.Save(ret, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return await Task.FromResult( ret.ToArray());
                }
            }
        }
        public async Task<byte[]> ContractMake(int kontract_id)
        {
            try
            {

                var appUserId = _dbContext.Contracts.Single(f => f.RowId == kontract_id);
                await _dbContext.CheckRecipient(appUserId.CustomerId.Value);


                var templ = await _dbContext.ContractTemplates.FirstOrDefaultAsync();
                 
                var res = new SqlParameter("@res",SqlDbType.VarChar,int.MaxValue);
                res.Direction = ParameterDirection.Output;
                await _dbContext.Database.ExecuteSqlRawAsync("EXEC [dbo].[GetUserKontractAsJson] @kontract_id, @res OUTPUT"
                        , new SqlParameter("@kontract_id",kontract_id)
                        , res
                    );
              //  Console.WriteLine("res={0};ln1={1};ln2={2}",res.Value,templ.Core.Length,templ.Templ.Length);
                return await (Task<byte[]>)Assembly.Load(templ.Core)
                    .GetType("PdfUtilCore.PdfUtil")
                    .GetMethod("ContractMake", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, new object[]
                    {
                        templ.Templ
                        , _appFormats
                        , res.Value.ToString()
                    });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return Properties.Resources.wait;

                throw;
            }

        }

    }
}
