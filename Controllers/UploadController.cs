using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AUTO_ARCHIVE.Models;
using AUTO_ARCHIVE.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;

namespace AUTO_ARCHIVE.Controllers
{
    [Authorize(Policy = "Database")]
    public class UploadController : Controller
    {
        private const string importcsv = "knctdata/importcsv";

        private const string autoimages = "knctdata/autoimages";

        private readonly string csvContainer = "importcsv";

        private readonly string imgContainer = "autoimages";

        private readonly IS3Service _service;

        private readonly IBlobManager _serviceBlob;

        private readonly SCARFContext _context;

        public UploadController(IS3Service service, SCARFContext context, IConfiguration configuration, IBlobManager serviceBlob)
        {
            _serviceBlob = serviceBlob;
            _service = service;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UploadFiles(string selection, List<IFormFile> Files)
        {
            if(selection == "csv" && Files != null)
            {
                await AddCSV(Files);

                return Redirect(Request.Headers["Referer"].ToString());
            }
            else if(selection == "images" && Files != null)
            {
                await AddImages(Files);

                return Redirect(Request.Headers["Referer"].ToString());
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task AddCSV(List<IFormFile> csvFiles)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _context.Database.ExecuteSqlCommand("TRUNCATE table TEMP_AUTO");

            foreach (var file in csvFiles)
            {
                if (file.Length > 0)
                {
                    var reader = new StreamReader(file.OpenReadStream());
                    
                    var csv = new CsvReader(reader);

                    csv.Configuration.HasHeaderRecord = false;

                    csv.Configuration.Delimiter = ",";

                    csv.Configuration.MissingFieldFound = null;

                    while (csv.Read())
                    {
                        TempAuto record = csv.GetRecord<TempAuto>();

                        _context.Add(record);

                        _context.SaveChanges();
                    }
                    
                    _context.Database.ExecuteSqlCommand("LoadData");

                    await _service.UploadFileAsync(file, importcsv);

                    await _serviceBlob.UploadFile(file, csvContainer);
                }
            }
        }

        public async Task AddImages(List<IFormFile> imgFiles)
        {
            string fileName = null;

            foreach (var file in imgFiles)
            {
                if (file.Length > 0)
                {
                    var ms = new MemoryStream();
                        
                    file.CopyTo(ms);

                    var fileBytes = ms.ToArray();

                    fileName = file.FileName;

                    if (fileName.Contains("(1)"))
                    {
                        var Vin = fileName.Split("_")[0];

                        var Date = fileName.Split("_")[1].Replace("(1).jpg", "");

                        var param = new SqlParameter();

                        _context.Database.ExecuteSqlCommand("UPDATE [dbo].[AUTO] " +
                                                            "SET[First Image] = @p0 " +
                                                            "WHERE VIN = @p1 and [AUCTION DATE] = @p2",
                                                            parameters: new object[] { fileBytes, Vin, Date });
                    }

                    if (fileName.Contains("(2)"))
                    {
                        var Vin = fileName.Split("_")[0];

                        var Date = fileName.Split("_")[1].Replace("(2).jpg", "");

                        var param = new SqlParameter();

                        _context.Database.ExecuteSqlCommand("UPDATE [dbo].[AUTO] " +
                                                            "SET[Second Image] = @p0 " +
                                                            "WHERE VIN = @p1 and [AUCTION DATE] = @p2",
                                                            parameters: new object[] { fileBytes, Vin, Date });
                    }

                    if (fileName.Contains("(3)"))
                    {
                        var Vin = fileName.Split("_")[0];

                        var Date = fileName.Split("_")[1].Replace("(3).jpg", "");

                        var param = new SqlParameter();

                        _context.Database.ExecuteSqlCommand("UPDATE [dbo].[AUTO] " +
                                                            "SET[Third Image] = @p0 " +
                                                            "WHERE VIN = @p1 and [AUCTION DATE] = @p2",
                                                            parameters: new object[] { fileBytes, Vin, Date });
                    }

                    if (fileName.Contains("(4)"))
                    {
                        var Vin = fileName.Split("_")[0];

                        var Date = fileName.Split("_")[1].Replace("(4).jpg", "");

                        var param = new SqlParameter();

                        _context.Database.ExecuteSqlCommand("UPDATE [dbo].[AUTO] " +
                                                            "SET[Fourth Image] = @p0 " +
                                                            "WHERE VIN = @p1 and [AUCTION DATE] = @p2",
                                                            parameters: new object[] { fileBytes, Vin, Date });
                    }

                    if (fileName.Contains("(5)"))
                    {
                        var Vin = fileName.Split("_")[0];

                        var Date = fileName.Split("_")[1].Replace("(5).jpg", "");

                        var param = new SqlParameter();

                        _context.Database.ExecuteSqlCommand("UPDATE [dbo].[AUTO] " +
                                                            "SET[Fifth Image] = @p0 " +
                                                            "WHERE VIN = @p1 and [AUCTION DATE] = @p2",
                                                            parameters: new object[] { fileBytes, Vin, Date });
                    }

                    //await _service.UploadFileAsync(file, autoimages);

                    await _serviceBlob.UploadFile(file, imgContainer);
                }
            }
        }
    }
}
