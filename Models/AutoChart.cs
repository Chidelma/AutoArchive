using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AUTO_ARCHIVE.Models
{
    public class AutoChart
    {
        [Key]
        public int Id { get; set; }

        public String AuctDateX { get; set; }

        public int BidPriceY { get; set; }
    }
}
