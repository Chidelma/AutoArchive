using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AUTO_ARCHIVE.Models;
using MoreLinq;
using System.Diagnostics;
using System.Runtime.Caching;

namespace AUTO_ARCHIVE.Controllers
{
    public class HomeController : Controller
    {
        private readonly SCARFContext _context;

        private static string _key = "weeklyAuctions";

        private static MemoryCache _cache;

        public HomeController(SCARFContext context)
        {
            _context = context;
            _cache = MemoryCache.Default;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Results(string searchYear, string searchModel, string searchCyl,
                                               string searchStatus, string searchDate, string minBid, string maxBid,
                                               string minMile, string maxMile, string ultSearch, string isPending,
                                               string hasDuplicate, string hasImages)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.filtCount = getUserGarage().AUTO.Count();

                ViewBag.cartCount = getUserGarage().AUTO.Count() + getUserGarage().Compare.Count();

                ViewBag.compCount = getUserGarage().Compare.Count();
            }

            if (String.IsNullOrEmpty(ultSearch) &&
                String.IsNullOrEmpty(hasImages) &&
                String.IsNullOrEmpty(isPending) &&
                String.IsNullOrEmpty(maxMile) &&
                String.IsNullOrEmpty(minMile) &&
                String.IsNullOrEmpty(maxBid) &&
                String.IsNullOrEmpty(minBid) &&
                String.IsNullOrEmpty(searchDate) &&
                String.IsNullOrEmpty(searchStatus) &&
                String.IsNullOrEmpty(searchCyl) &&
                String.IsNullOrEmpty(searchModel) &&
                String.IsNullOrEmpty(searchYear) &&
                String.IsNullOrEmpty(hasDuplicate))
            {
                if (!_cache.Contains(_key))
                    await getWeekAuctions();
                
                return View(_cache.Get(_key) as List<Auto>);
            }
            else
            {
                var _queryKey = getKey(ultSearch, searchYear, searchModel, searchCyl, searchStatus, searchDate, minBid, maxBid, minMile, maxMile, isPending, hasDuplicate, hasImages);

                if (_cache.Contains(_queryKey))
                {
                    var autoResults = _cache.Get(_queryKey) as List<Auto>;

                    ViewBag.TotalRows = autoResults.Count();

                    var orderedResults = autoResults.OrderByDescending(a => a.AuctionDate);

                    return View(orderedResults);
                }
                else
                {
                    var query = _context.Auto.AsQueryable();

                    _queryKey = "Results";

                    if (!String.IsNullOrEmpty(ultSearch))
                    {
                        string[] sArr = ultSearch.ToUpper().Split(' ');
                        foreach (string word in sArr)
                        {
                            query = query.Where(a => a.Year.Contains(word) ||
                                                        a.Model.Contains(word) ||
                                                        a.Vin.Contains(word) ||
                                                        a.Status.Contains(word) ||
                                                        a.Cyl.Contains(word));
                        }

                        _queryKey += ultSearch;
                    }

                    if (!String.IsNullOrEmpty(searchYear))
                    {
                        query = query.Where(a => a.Year.Equals(searchYear));
                        _queryKey += searchYear;
                    }

                    if (!String.IsNullOrEmpty(searchModel))
                    {
                        query = query.Where(a => a.Make.Equals(searchModel));
                        _queryKey += searchModel;
                    }

                    if (!String.IsNullOrEmpty(searchCyl) && searchCyl != "None")
                    {
                        query = query.Where(a => a.Cyl.Contains(searchCyl));
                        _queryKey += searchCyl;
                    }

                    if (searchCyl == "None")
                    {
                        query = query.Where(a => a.Cyl.Equals(searchCyl));
                        _queryKey += searchCyl;
                    }

                    if (!String.IsNullOrEmpty(searchStatus))
                    {
                        query = query.Where(a => a.Status.Equals(searchStatus));
                        _queryKey += searchStatus;
                    }

                    if (!String.IsNullOrEmpty(searchDate))
                    {
                        query = query.Where(a => a.AuctionDate.Equals(searchDate));
                        _queryKey += searchDate;
                    }

                    if (!String.IsNullOrEmpty(minBid))
                    {
                        int bid = Convert.ToInt32(minBid);
                        query = query.Where(a => a.BidAmountC >= bid);
                        _queryKey += minBid;
                    }
                    if (!String.IsNullOrEmpty(maxBid))
                    {
                        int bid = Convert.ToInt32(maxBid);
                        query = query.Where(a => a.BidAmountC <= bid);
                        _queryKey += maxBid;
                    }
                    if (!String.IsNullOrEmpty(minMile))
                    {
                        int mile = Convert.ToInt32(minMile);
                        query = query.Where(a => a.MileageKm >= mile);
                        _queryKey += minMile;
                    }
                    if (!String.IsNullOrEmpty(maxMile))
                    {
                        int mile = Convert.ToInt32(maxMile);
                        query = query.Where(a => a.MileageKm <= mile);
                        _queryKey += maxMile;
                    }
                    if (!String.IsNullOrEmpty(isPending))
                    {
                        if (isPending == "unavailable")
                        {
                            query = query.Where(a => a.BidAmountC == null);
                        }
                        else
                        {
                            query = query.Where(a => a.BidAmountC != null);
                        }
                        _queryKey += isPending;
                    }

                    if (!String.IsNullOrEmpty(hasImages))
                    {
                        query = query.Where(a => a.FirstImage != null || a.SecondImage != null || a.ThirdImage != null || a.FourthImage != null || a.FifthImage != null);
                        _queryKey += hasImages;
                    }

                    if (!String.IsNullOrEmpty(hasDuplicate))
                    {
                        query = query.Where(a => a.Duplicate == true);
                        _queryKey += hasDuplicate;
                    }

                    var cacheItemPolicy = new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTime.Now.AddDays(1)
                    };

                    _cache.Add(_queryKey, await query.ToListAsync(), cacheItemPolicy);

                    ViewBag.TotalRows = query.Count();

                    query = query.OrderByDescending(a => a.AuctionDate);

                    return View(await query.ToListAsync());
                }
            }
        }

        public AutoGarage getUserGarage()
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            if (_context.Garage.Find(User.Claims.FirstOrDefault().Value.ToString()) == null)
            {
                AutoGarage newGarage = new AutoGarage { AUTO = new List<Auto>(), Compare = new List<Auto>() };

                string xmlData = ConvertObjectToXMLString(newGarage);

                var userGarage = new Garage { UserOid = User.Claims.FirstOrDefault().Value.ToString(), UserGarage = xmlData };

                _context.Add(userGarage);

                _context.SaveChanges();
            }

            var garage = _context.Garage.AsNoTracking().SingleOrDefault(m => m.UserOid == User.Claims.FirstOrDefault().Value.ToString());

            return ConvertXmlStringtoObject<AutoGarage>(garage.UserGarage.ToString());
        }

        static string ConvertObjectToXMLString(object classObject)
        {
            string xmlString = null;
            XmlSerializer xmlSerializer = new XmlSerializer(classObject.GetType());
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, classObject);
                memoryStream.Position = 0;
                xmlString = new StreamReader(memoryStream).ReadToEnd();
            }
            return xmlString;
        }

        static T ConvertXmlStringtoObject<T>(string xmlString)
        {
            T classObject;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xmlString))
            {
                classObject = (T)xmlSerializer.Deserialize(stringReader);
            }
            return classObject;
        }

        public async Task getWeekAuctions()
        { 
            var lastMonth = _context.Auto.DistinctBy(a => a.AuctionDate).OrderByDescending(a => a.AuctionDate).Select(b => b.AuctionDate).Take(3).ToList();

            var currDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

            if (currDate - Convert.ToInt32(lastMonth[1].Replace("-", "")) > 2)
            {
                var auctionDate = lastMonth[1];

                var topTenAuto = _context.Auto.Where(a => a.AuctionDate.Equals(auctionDate)).GroupBy(b => b.Model).Select(b => new { Model = b.Key, Freq = b.Count() }).OrderByDescending(c => c.Freq).Take(6).ToList();

                List<Auto> popAutos = new List<Auto>();

                foreach (var auto in topTenAuto)
                {
                    var firstAttr = _context.Auto.Where(a => a.Model.Contains(auto.Model) && a.AuctionDate.Equals(auctionDate)).GroupBy(b => new { b.BidAmountC, b.Status, b.Cyl }).Select(c => new { Bid = c.Key.BidAmountC, status = c.Key.Status, cyl = c.Key.Cyl }).ToList();

                    var firstAuto = _context.Auto.Where(a => a.Model.Contains(auto.Model) && a.BidAmountC.Equals(firstAttr[0].Bid) && a.Status.Equals(firstAttr[0].status) && a.Cyl.Equals(firstAttr[0].cyl) && a.AuctionDate.Equals(auctionDate)).FirstOrDefault();

                    popAutos.Add(firstAuto);
                }

                var cacheItemPolicy = new CacheItemPolicy()
                {
                    //Set your Cache expiration.
                    AbsoluteExpiration = DateTime.Now.AddDays(1)
                };

                _cache.Add(_key, popAutos, cacheItemPolicy);
            }
            else
            {
                var auctionDate = lastMonth[2];
                
                var topTenAuto = _context.Auto.Where(a => a.AuctionDate.Equals(auctionDate)).GroupBy(b => b.Model).Select(b => new { Model = b.Key, Freq = b.Count() }).OrderByDescending(c => c.Freq).Take(6).ToList();
                
                List<Auto> popAutos = new List<Auto>();

                foreach(var auto in topTenAuto)
                {
                    var firstAttr = _context.Auto.Where(a => a.Model.Contains(auto.Model) && a.AuctionDate.Equals(auctionDate)).GroupBy(b => new { b.BidAmountC, b.Status, b.Cyl }).Select(c => new { Bid = c.Key.BidAmountC, status = c.Key.Status, cyl = c.Key.Cyl }).ToList();

                    var firstAuto = _context.Auto.Where(a => a.Model.Contains(auto.Model) && a.BidAmountC.Equals(firstAttr[0].Bid) && a.Status.Equals(firstAttr[0].status) && a.Cyl.Equals(firstAttr[0].cyl) && a.AuctionDate.Equals(auctionDate)).FirstOrDefault();

                    popAutos.Add(firstAuto);
                }

                var cacheItemPolicy = new CacheItemPolicy()
                {
                    //Set your Cache expiration.
                    AbsoluteExpiration = DateTime.Now.AddDays(1)
                };

                _cache.Add(_key, popAutos, cacheItemPolicy);
            }
            
        }

        public string getKey(string ultSearch, string searchYear, string searchModel, string searchCyl,
                            string searchStatus, string searchDate, string minBid, string maxBid,
                            string minMile, string maxMile, string isPending,
                            string hasDuplicate, string hasImages)
        {
            string _key = "Results";

            if (!String.IsNullOrEmpty(ultSearch))
            {
                _key += ultSearch;
            }

            if (!String.IsNullOrEmpty(searchYear))
            {
                _key += searchYear;
            }

            if (!String.IsNullOrEmpty(searchModel))
            {
                _key += searchModel;
            }

            if (!String.IsNullOrEmpty(searchCyl) && searchCyl != "None")
            {
                _key += searchCyl;
            }

            if (searchCyl == "None")
            {
                _key += searchCyl;
            }

            if (!String.IsNullOrEmpty(searchStatus))
            {
                _key += searchStatus;
            }

            if (!String.IsNullOrEmpty(searchDate))
            {
                _key += searchDate;
            }

            if (!String.IsNullOrEmpty(minBid))
            {
                _key += minBid;
            }
            if (!String.IsNullOrEmpty(maxBid))
            {
                _key += maxBid;
            }
            if (!String.IsNullOrEmpty(minMile))
            {
                _key += minMile;
            }
            if (!String.IsNullOrEmpty(maxMile))
            {
                _key += maxMile;
            }
            if (!String.IsNullOrEmpty(isPending))
            {
                _key += isPending;
            }

            if (!String.IsNullOrEmpty(hasImages))
            {
                _key += hasImages;
            }

            if (!String.IsNullOrEmpty(hasDuplicate))
            {
                _key += hasDuplicate;
            }

            return _key;
        }
    }
}
