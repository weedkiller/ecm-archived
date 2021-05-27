using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class CustomerTypeViewModel
    {
        public long CustomerTypeId { get; set; }
        public long? ParentId { get; set; }
        public long SiteId { get; set; }
        public string CustomerTypeName { get; set; }
        public string ParentCustomerTypeName { get; set; }
    }
}
