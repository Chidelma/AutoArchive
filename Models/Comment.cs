using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AUTO_ARCHIVE.Models
{
    public class Comment
    {
        public String DisplayName { get; set; }

        [Key]
        public String UserOID { get; set; }

        public String UserComment { get; set; }

        public String timeStamp { get; set; }
    }
}
