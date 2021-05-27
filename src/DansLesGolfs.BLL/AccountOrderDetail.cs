using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class AccountOrderDetail
    {
        public string CategoryName { get; set; }

        public string ItemName { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
