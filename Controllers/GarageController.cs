using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AUTO_ARCHIVE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Serialization;

namespace AUTO_ARCHIVE.Controllers
{
    public class GarageController : Controller
    {
        private readonly SCARFContext _context;

        private readonly MemoryCache _cache;

        public GarageController(SCARFContext context)
        {
            _context = context;

            _cache = MemoryCache.Default;
        }

        // GET: Garage
        public IActionResult Index(string searchYear, string searchModel, string searchCyl,
                                    string searchStatus, string searchDate, string minBid, string maxBid,
                                    string minMile, string maxMile, string isPending,
                                    string hasDuplicate, string hasImages)
        {
            ViewBag.filtCount = getUserGarage().AUTO.Count();

            ViewBag.cartCount = getUserGarage().AUTO.Count() + getUserGarage().Compare.Count();

            ViewBag.compCount = getUserGarage().Compare.Count();

            if (String.IsNullOrEmpty(hasImages) &&
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
                if (!_cache.Contains(User.Claims.FirstOrDefault().Value.ToString()))
                {
                    var cacheItemPolicy = new CacheItemPolicy()
                    {
                        //Set your Cache expiration.
                        AbsoluteExpiration = DateTime.Now.AddDays(1)
                    };

                    _cache.Add(User.Claims.FirstOrDefault().Value.ToString(), getUserGarage(), cacheItemPolicy);
                }

                return View(_cache.Get(User.Claims.FirstOrDefault().Value.ToString()) as AutoGarage);
            }
            else
            {
                var _key = getKey(searchYear, searchModel, searchModel, searchStatus, searchDate, minBid, maxBid, minMile, maxMile, isPending, hasDuplicate, hasImages);

                if (_cache.Contains(_key))
                {
                    return View(_cache.Get(_key) as AutoGarage);
                }
                else
                {
                    var query = getUserGarage().AUTO.AsQueryable();

                    _key = "Garage";

                    if (!String.IsNullOrEmpty(searchYear))
                    {
                        query = query.Where(a => a.Year.Equals(searchYear));
                        _key += searchYear;
                    }

                    if (!String.IsNullOrEmpty(searchModel))
                    {
                        query = query.Where(a => a.Make.Equals(searchModel));
                        _key += searchModel;
                    }

                    if (!String.IsNullOrEmpty(searchCyl) && searchCyl != "None")
                    {
                        query = query.Where(a => a.Cyl.Contains(searchCyl));
                        _key += searchCyl;
                    }

                    if (searchCyl == "None")
                    {
                        query = query.Where(a => a.Cyl == null);
                        _key += searchCyl;
                    }

                    if (!String.IsNullOrEmpty(searchStatus))
                    {
                        query = query.Where(a => a.Status.Equals(searchStatus));
                        _key += searchStatus;
                    }

                    if (!String.IsNullOrEmpty(searchDate))
                    {
                        query = query.Where(a => a.AuctionDate.Equals(searchDate));
                        _key += searchDate;
                    }

                    if (!String.IsNullOrEmpty(minBid))
                    {
                        int bid = Convert.ToInt32(minBid);
                        query = query.Where(a => a.BidAmountC >= bid);
                        _key += minBid;
                    }
                    if (!String.IsNullOrEmpty(maxBid))
                    {
                        int bid = Convert.ToInt32(maxBid);
                        query = query.Where(a => a.BidAmountC <= bid);
                        _key += maxBid;
                    }
                    if (!String.IsNullOrEmpty(minMile))
                    {
                        int mile = Convert.ToInt32(minMile);
                        query = query.Where(a => a.MileageKm >= mile);
                        _key += minMile;
                    }
                    if (!String.IsNullOrEmpty(maxMile))
                    {
                        int mile = Convert.ToInt32(maxMile);
                        query = query.Where(a => a.MileageKm <= mile);
                        _key += maxMile;
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
                        _key += isPending;
                    }

                    if (!String.IsNullOrEmpty(hasImages))
                    {
                        query = query.Where(a => a.FirstImage != null || a.SecondImage != null);
                        _key += hasImages;
                    }

                    if (!String.IsNullOrEmpty(hasDuplicate))
                    {
                        query = query.Where(a => a.Duplicate == true);
                        _key += hasDuplicate;
                    }

                    AutoGarage UserGarage = new AutoGarage { AUTO = query.ToList(), Compare = getUserGarage().Compare };

                    var cacheItemPolicy = new CacheItemPolicy()
                    {
                        //Set your Cache expiration.
                        AbsoluteExpiration = DateTime.Now.AddDays(1)
                    };

                    _cache.Add(_key, UserGarage, cacheItemPolicy);

                    return View(UserGarage);
                }
            }
        }

        public IActionResult addToGarage(int id, string type)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            if (alreadyExist(id, type) == -1)
            {
                AutoGarage userGarage = getUserGarage();

                if (type == "garage")
                {
                    userGarage.AUTO.Add(_context.Auto.Find(id));
                }

                if (type == "compare")
                {
                    userGarage.Compare.Add(_context.Auto.Find(id));
                }

                string xmlData = ConvertObjectToXMLString(userGarage);

                var modGarage = new Garage { UserOid = User.Claims.FirstOrDefault().Value.ToString(), UserGarage = xmlData };

                _context.Update(modGarage);

                _context.SaveChanges();

                if (_cache.Contains(User.Claims.FirstOrDefault().Value.ToString()))
                {
                    var cacheItemPolicy = new CacheItemPolicy()
                    {
                        AbsoluteExpiration = DateTime.Now.AddDays(1)
                    };

                    _cache.Set(User.Claims.FirstOrDefault().Value.ToString(), userGarage, cacheItemPolicy);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private int alreadyExist(int id, string type)
        {
            List<Auto> userGarage = null;

            if (type == "compare")
            {
                userGarage = getUserGarage().Compare;
            }

            if (type == "garage")
            {
                userGarage = getUserGarage().AUTO;
            }

            for (int i = 0; i < userGarage.Count; i++)
            {
                if (userGarage[i].Id == id) return i;
            }

            return -1;
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
            using (MemoryStream memoryStream = new MemoryStream())
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

        public IActionResult Delete(int id, string type)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            AutoGarage userGarage = getUserGarage();

            int check = alreadyExist(id, type);

            if (type == "garage")
            {
                userGarage.AUTO.RemoveAt(check);
            }

            if (type == "compare")
            {
                userGarage.Compare.RemoveAt(check);
            }

            string xmlData = ConvertObjectToXMLString(userGarage);

            var modGarage = new Garage { UserOid = User.Claims.FirstOrDefault().Value.ToString(), UserGarage = xmlData };

            _context.Update(modGarage);

            _context.SaveChanges();

            var cacheItemPolicy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            };

            _cache.Set(User.Claims.FirstOrDefault().Value.ToString(), userGarage, cacheItemPolicy);

            return RedirectToAction(nameof(Index));
        }

        public string getKey(string searchYear, string searchModel, string searchCyl,
                            string searchStatus, string searchDate, string minBid, string maxBid,
                            string minMile, string maxMile, string isPending,
                            string hasDuplicate, string hasImages)
        {
            string _key = "Garage";

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
