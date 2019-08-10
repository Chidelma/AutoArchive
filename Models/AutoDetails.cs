using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AUTO_ARCHIVE.Models
{
    public class AutoDetails
    {
        [Key]
        public int Id { get; set; }

        public int count { get; set; }

        public string minBid { get; set; }

        public string maxBid { get; set; }

        public string AvgSalePrice { get; set; }

        public string AvgBidPrice { get; set; }

        public List<Auto> ListGarage { get; set; }

        public List<AutoChart> ListChart { get; set; }

        public List<Comment> ListComment { get; set; }

        public string KijijiData { get; set; }
    }
}
