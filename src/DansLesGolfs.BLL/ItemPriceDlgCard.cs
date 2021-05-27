using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DansLesGolfs.BLL
{
    public class ItemPriceDlgCard
    {

        public int ItemPriceId { get; set; }

        public int ItemId { get; set; }

        public long SiteId { get; set; }

        public int CustomerTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double Price { get; set; }

        public int UserId { get; set; }
    }
}
