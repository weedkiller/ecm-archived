using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class EmailingViewModel
    {
        public long EmailId { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public string EmailName { get; set; }
        public string Subject { get; set; }
        public DateTime? InsertDate { get; set; }
    }
}
