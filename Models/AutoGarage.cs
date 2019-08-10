using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace AUTO_ARCHIVE.Models
{
    public class AutoGarage
    {
        public List<Auto> AUTO { get; set; }

        public List<Auto> Compare { get; set; }
    }
}
