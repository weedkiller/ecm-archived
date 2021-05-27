using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class SalesReportModel
    {
        public List<SalesReport> SalesReportList { get; set; }

        public List<int> ListOrderId { get; set; } 
    }
}
