using System;
using System.Collections.Generic;

namespace AUTO_ARCHIVE.Models
{
    public partial class Auto
    {
        public string Year { get; set; }
        public string Model { get; set; }
        public string Vin { get; set; }
        public string Cyl { get; set; }
        public string Status { get; set; }
        public int? MileageKm { get; set; }
        public int? BidAmountC { get; set; }
        public string AuctionDate { get; set; }
        public byte[] FirstImage { get; set; }
        public byte[] SecondImage { get; set; }
        public string Make { get; set; }
        public int Id { get; set; }
        public bool? Duplicate { get; set; }
        public string Comments { get; set; }
        public byte[] ThirdImage { get; set; }
        public byte[] FourthImage { get; set; }
        public byte[] FifthImage { get; set; }
    }
}
