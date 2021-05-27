using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;
using DansLesGolfs.BLL;

namespace DansLesGolfs.Models
{
    public class TeeSheetModel
    {
        public long id { get; set; }
        public string date { get; set; }
        public short day { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
        public decimal percent { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public int prebooking { get; set; }
    }

    public class TeeSheetList
    {
        public List<TeeSheetModel> teeSheets { get; set; }
    }
}