using System;
using System.Collections.Generic;

namespace AUTO_ARCHIVE.Models
{
    public partial class TempAuto
    {
        public int Year { get; set; }
        public string Model { get; set; }
        public string Vin { get; set; }
        public string Cyl { get; set; }
        public string Status { get; set; }
        public int? MileageKm { get; set; }
        public int? BidAmountC { get; set; }
        public string AuctionDate { get; set; }
    }
}
