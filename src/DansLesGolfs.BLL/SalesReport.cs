using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class SalesReport
    {

        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string CustomerName { get; set; }

        public int? UserId { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public float UnitPrice { get; set; }

        public float Quantity { get; set; }

        public float ExtendedPrice { get; set; }

        public string PaymentType { get; set; }

        public bool Active { get; set; }

        public string PaymentStatus { get; set; }
    }
}
