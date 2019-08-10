using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Caching;
using AUTO_ARCHIVE.Models;

namespace AUTO_ARCHIVE.Controllers
{
    public class AnalyticController : Controller
    {
        private readonly SCARFContext _context;

        private static MemoryCache _cache;

        public AnalyticController(SCARFContext context)
        {
            _context = context;
            _cache = MemoryCache.Default;
        }

        public async Task<IActionResult> Index(string vin, string model, int year)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.filtCount = getUserGarage().AUTO.Count();

                ViewBag.cartCount = getUserGarage().AUTO.Count() + getUserGarage().Compare.Count();
            }

            ViewBag.VinAnalytic = vin;

            ViewBag.ModelAnalytic = model;

            ViewBag.YearAnalytic = year;

            var _key = vin + model + year;

            if (!_cache.Contains(_key))
                await getAnalytics(vin, model, year);

            return View(_cache.Get(_key) as AutoDetails);
        }

        public async Task getAnalytics(string vin, string model, int year)
        {
            var avgBid = "Unavailable";

            List<Auto> newGarage = new List<Auto>();

            var getAuto = _context.Auto.Where(a => a.Vin.Equals(vin)).OrderBy(a => a.AuctionDate).ToList();

            var autoCount = getAuto.Count();

            string minBid = null; string maxBid = null;

            if (getAuto.Min(a => a.BidAmountC) != null && getAuto.Max(a => a.BidAmountC) != null)
            {
                minBid = "C$" + getAuto.Min(a => a.BidAmountC);

                maxBid = "C$" + getAuto.Max(a => a.BidAmountC);
            }
            else
            {
                minBid = "Unavailable";

                maxBid = "Unavailable";
            }

            if (getAuto.Average(a => a.BidAmountC) != null)
            {
                avgBid = "C$" + Convert.ToString(Convert.ToInt32(getAuto.Average(a => a.BidAmountC)));
            }

            var list = new List<AutoChart>();

            foreach (var item in getAuto)
            {
                newGarage.Add(item);

                if (item.BidAmountC != null)
                {
                    list.Add(new AutoChart { AuctDateX = item.AuctionDate, BidPriceY = Convert.ToInt32(item.BidAmountC) });
                }
                else
                {
                    list.Add(new AutoChart { AuctDateX = item.AuctionDate, BidPriceY = 0 });
                }
            }

            var avgSale = PredictedSale(getAuto);

            string kijijiPrice;

            int timeSpan = Convert.ToInt32(DateTime.Now.ToString("yyyy")) - year;

            if (model.ToLower().Contains("trailer") || timeSpan > 25 || model.ToLower().Contains("motorcycle") || model.ToLower().Contains("moped"))
            {
                kijijiPrice = "Unavailable";
            }
            else
            {
                if (avgSale != "Unavailable")
                {
                    string avgK = getKijijiData(model, Convert.ToString(year));

                    int avgDiff = Convert.ToInt32(avgK) - Convert.ToInt32(avgSale.Replace("C$", ""));

                    kijijiPrice = "C$" + Convert.ToString(avgDiff);
                }
                else
                {
                    kijijiPrice = "C$" + getKijijiData(model, Convert.ToString(year));
                }
            }

            if (avgSale != "Unavailable")
            {
                avgSale = "C$" + avgSale;
            }

            AutoDetails avgDetails = new AutoDetails { AvgSalePrice = avgSale, AvgBidPrice = avgBid, ListGarage = newGarage, ListChart = list, ListComment = getAutoComments(vin), KijijiData = kijijiPrice, count = autoCount, maxBid = maxBid, minBid = minBid };

            var cacheItemPolicy = new CacheItemPolicy()
            {
                //Set your Cache expiration.
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            };

            var _key = vin + model + year;

            _cache.Add(_key, avgDetails, cacheItemPolicy);
        }

        public IActionResult PostComment(string comment = "", string vin = "")
        {
            ViewBag.Comment = comment;

            Comment newPost = new Comment { UserOID = User.Claims.FirstOrDefault().Value.ToString(), DisplayName = User.Claims.FirstOrDefault(c => c.Type.Equals("name")).Value.Split(" ")[0], UserComment = comment, timeStamp = DateTime.Now.ToString("d MMM, yyy") + " at " + DateTime.Now.ToString("HH:mm tt") };

            if (getAutoComments(vin) == null)
            {
                List<Comment> AutoComments = new List<Comment>();

                AutoComments.Add(newPost);

                string xmlData = ConvertObjectToXMLString(AutoComments);

                var getAuto = _context.Auto.Where(a => a.Vin.Equals(vin)).FirstOrDefault();

                getAuto.Comments = xmlData;

                _context.SaveChanges();
            }
            else
            {
                List<Comment> AutoComments = getAutoComments(vin);

                AutoComments.Add(newPost);

                string xmlData = ConvertObjectToXMLString(AutoComments);

                var getAuto = _context.Auto.Where(a => a.Vin.Equals(vin)).FirstOrDefault();

                getAuto.Comments = xmlData;

                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public string PredictedSale(List<Auto> getAuto)
        {
            var avgSale = "Unavailable";

            IQueryable<Auto> queryGetAuto;

            if (getAuto.Any(a => a.BidAmountC == null))
            {
                queryGetAuto = PredictionAlgorithm(getAuto);
            }
            else
            {
                queryGetAuto = GetAverageSalePrice(getAuto);
            }

            if (queryGetAuto.Average(a => a.BidAmountC) != null)
            {
                avgSale = Convert.ToString(Convert.ToInt32(queryGetAuto.Average(a => a.BidAmountC)));
            }

            return avgSale;
        }

        public IQueryable<Auto> PredictionAlgorithm(List<Auto> getAuto)
        {
            Auto Unsold = null; IQueryable<Auto> queryGetAuto; string splitModel = null;

            if (getAuto.Count == 1)
            {
                Unsold = getAuto[0];
            }
            else
            {
                Unsold = getAuto[getAuto.Count - 1];
            }

            var AutoModel = Convert.ToString(Unsold.Model).Split();

            if (AutoModel.Count() <= 3)
            {
                splitModel = AutoModel[0] + ' ' + AutoModel[1];
            }
            else
            {
                splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
            }

            queryGetAuto = _context.Auto.Where(a => a.Year.Equals(Unsold.Year) &&
                                                       a.Model.Contains(splitModel) &&
                                                       a.Cyl.Equals(Unsold.Cyl) &&
                                                       a.Status.Equals(Unsold.Status) &&
                                                       a.BidAmountC != null);

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                queryGetAuto = _context.Auto.Where(a => a.Year.Equals(Unsold.Year) &&
                                                       a.Model.Contains(splitModel) &&
                                                       a.Status.Equals(Unsold.Status) &&
                                                       a.BidAmountC != null);
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                queryGetAuto = _context.Auto.Where(a => a.Year.Equals(Unsold.Year) &&
                                                       a.Model.Contains(splitModel) &&
                                                       a.BidAmountC != null);
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                queryGetAuto = _context.Auto.Where(a => a.Model.Contains(splitModel) &&
                                                       a.Status.Equals(Unsold.Status) &&
                                                       a.BidAmountC != null);
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                queryGetAuto = _context.Auto.Where(a => a.Model.Contains(splitModel) &&
                                                       a.BidAmountC != null);
            }

            return queryGetAuto;
        }

        public IQueryable<Auto> GetAverageSalePrice(List<Auto> getAuto)
        {
            string splitModel = null; IQueryable<Auto> queryGetAuto = null; int avgBid = 0;

            if (getAuto.Average(a => a.BidAmountC) != null)
            {
                avgBid = Convert.ToInt32(getAuto.Average(a => a.BidAmountC));
            }

            foreach (var item in getAuto)
            {
                var AutoModel = Convert.ToString(item.Model).Split();

                if (AutoModel.Count() < 3)
                {
                    splitModel = AutoModel[0] + ' ' + AutoModel[1];
                }
                else
                {
                    splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
                }

                queryGetAuto = _context.Auto.Where(a => a.Year.Equals(item.Year) &&
                                                       a.Model.Contains(splitModel) &&
                                                       a.Cyl.Equals(item.Cyl) &&
                                                       a.Status.Equals(item.Status) &&
                                                       a.Vin != item.Vin &&
                                                       a.BidAmountC != null);
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                foreach (var item in getAuto)
                {
                    var AutoModel = Convert.ToString(item.Model).Split();

                    if (AutoModel.Count() < 3)
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1];
                    }
                    else
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
                    }

                    queryGetAuto = _context.Auto.Where(a => a.Year.Equals(item.Year) &&
                                                           a.Model.Contains(splitModel) &&
                                                           a.Status.Equals(item.Status) &&
                                                           a.Vin != item.Vin &&
                                                           a.BidAmountC != null);
                }
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                foreach (var item in getAuto)
                {
                    var AutoModel = Convert.ToString(item.Model).Split();

                    if (AutoModel.Count() < 3)
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1];
                    }
                    else
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
                    }

                    queryGetAuto = _context.Auto.Where(a => a.Year.Equals(item.Year) &&
                                                           a.Model.Contains(splitModel) &&
                                                           a.Vin != item.Vin &&
                                                           a.BidAmountC != null);
                }
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                foreach (var item in getAuto)
                {
                    var AutoModel = Convert.ToString(item.Model).Split();

                    if (AutoModel.Count() < 3)
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1];
                    }
                    else
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
                    }

                    queryGetAuto = _context.Auto.Where(a => a.Model.Contains(splitModel) &&
                                                           a.Status.Equals(item.Status) &&
                                                           a.Vin != item.Vin &&
                                                           a.BidAmountC != null);
                }
            }

            if (queryGetAuto.Average(a => a.BidAmountC) == null)
            {
                foreach (var item in getAuto)
                {
                    var AutoModel = Convert.ToString(item.Model).Split();

                    if (AutoModel.Count() < 3)
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1];
                    }
                    else
                    {
                        splitModel = AutoModel[0] + ' ' + AutoModel[1] + ' ' + AutoModel[2];
                    }

                    queryGetAuto = _context.Auto.Where(a => a.Model.Contains(splitModel) &&
                                                            a.Vin != item.Vin &&
                                                            a.BidAmountC != null);
                }
            }

            return queryGetAuto;
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

        public List<Comment> getAutoComments(string vin)
        {
            //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var comment = _context.Auto.Where(m => m.Vin.Equals(vin)).FirstOrDefault();

            if (comment.Comments != null)
            {
                return ConvertXmlStringtoObject<List<Comment>>(comment.Comments.ToString());
            }

            return null;
        }

        public string getKijijiData(string model, string year)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();

                var modelSplit = model.Split();

                string weburl = null;

                if(modelSplit.Count() == 3 && (modelSplit[2].Length == 2 || modelSplit[2].Length == 3))
                {
                    weburl = "https://www.kijiji.ca/b-cars-vehicles/canada/" + year + "-" + modelSplit[0] + "-" + modelSplit[1] + "-" + modelSplit[2] +"/k0c27l0?ad=offering&price=1000__100000";
                }
                else
                {
                    weburl = "https://www.kijiji.ca/b-cars-vehicles/canada/" + year + "-" + modelSplit[0] + "-" + modelSplit[1] + "/k0c27l0?ad=offering&price=1000__100000";
                }
                
                HtmlDocument doc = web.Load(weburl.ToLower());

                var priceList = doc.DocumentNode.SelectNodes("//div[@class='price']").ToList();

                List<int> prices = new List<int>();

                string priceAvg;

                foreach (var price in priceList)
                {
                    int i;

                    if (int.TryParse(price.InnerText.Replace("$", "").Replace(",", "").Replace(".00", ""), out i))
                    {
                        int intPrice = Convert.ToInt32(price.InnerText.Replace("$", "").Replace(",", "").Replace(".00", ""));

                        prices.Add(intPrice);
                    }
                }

                priceAvg = Convert.ToString(Convert.ToInt32(prices.Average()));

                return priceAvg;
            }
            catch(Exception)
            {
                try
                {
                    HtmlWeb web = new HtmlWeb();

                    var modelSplit = model.Split();

                    string weburl = null;

                    if (modelSplit.Count() == 3 && (modelSplit[2].Length == 2 || modelSplit[2].Length == 3))
                    {
                        weburl = "https://www.autotrader.ca/cars/" + modelSplit[0] + "/" + modelSplit[1] + "%20" + modelSplit[2] + "/" + year + "/?rcp=100&sts=Used";
                    }
                    else
                    {
                        weburl = "https://www.autotrader.ca/cars/" + modelSplit[0] + "/" + modelSplit[1] + "/" + year + "/?rcp=100&sts=Used";
                    }

                    HtmlDocument doc = web.Load(weburl.ToLower());

                    var priceList = doc.DocumentNode.SelectNodes("//span[@class='price-amount']").ToList();

                    List<int> prices = new List<int>();

                    string priceAvg;

                    foreach (var price in priceList)
                    {
                        int i;

                        if (int.TryParse(price.InnerText.Replace("$", "").Replace(",", ""), out i))
                        {
                            int intPrice = Convert.ToInt32(price.InnerText.Replace("$", "").Replace(",", ""));

                            prices.Add(intPrice);
                        }
                    }

                    if(Convert.ToInt32(prices.Average()) > 30000)
                    {
                        web = new HtmlWeb();

                        modelSplit = model.Split();

                        weburl = "https://www.autotrader.ca/cars/" + modelSplit[0] + "/" + modelSplit[1] + "/" + year + "/?rcp=100&sts=Used";
                        
                        doc = web.Load(weburl.ToLower());

                        priceList = doc.DocumentNode.SelectNodes("//span[@class='price-amount']").ToList();

                        prices = new List<int>();

                        foreach (var price in priceList)
                        {
                            int i;

                            if (int.TryParse(price.InnerText.Replace("$", "").Replace(",", ""), out i))
                            {
                                int intPrice = Convert.ToInt32(price.InnerText.Replace("$", "").Replace(",", ""));

                                prices.Add(intPrice);
                            }
                        }

                        priceAvg = Convert.ToString(Convert.ToInt32(prices.Average()));

                        return priceAvg;
                    }
                    else
                    {
                        priceAvg = Convert.ToString(Convert.ToInt32(prices.Average()));

                        return priceAvg;
                    }
                }
                catch(Exception)
                {
                    HtmlWeb web = new HtmlWeb();

                    var modelSplit = model.Split();

                    string weburl = null;

                    if (modelSplit.Count() == 3 && (modelSplit[2].Length == 2 || modelSplit[2].Length == 3))
                    {
                        weburl = "https://www.kijijiautos.ca/cars/" + modelSplit[0] + "/" + modelSplit[1] + "-" + modelSplit[2] + "/" + year + "/";
                    }
                    else
                    {
                        weburl = "https://www.kijijiautos.ca/cars/" + modelSplit[0] + "/" + modelSplit[1] + "/" + year + "/";
                    }

                    HtmlDocument doc = web.Load(weburl.ToLower());

                    var priceList = doc.DocumentNode.SelectNodes("//span[@class='_257rsiOLZRw2SUP41j3TiB _2EdRVi2tLqR7VPkJ2yR6Zx QVJux59ueO-M7o0DZZ6Vx _14Nqyg9Gv-h-nJ_lZalHW_']").ToList();

                    List<int> prices = new List<int>();

                    string priceAvg;

                    foreach (var price in priceList)
                    {
                        int i;

                        if (int.TryParse(price.InnerText.Replace("$", "").Replace(",", ""), out i))
                        {
                            int intPrice = Convert.ToInt32(price.InnerText.Replace("$", "").Replace(",", ""));

                            prices.Add(intPrice);
                        }
                    }

                    priceAvg = Convert.ToString(Convert.ToInt32(prices.Average()));

                    return priceAvg;
                }
            }
        }
    }
}