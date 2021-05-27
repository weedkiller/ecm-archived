using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;

namespace DansLesGolfs.Models
{
    public class AlbatrosTeeSheetModel
    {
        public string SiteName { get; set; }
        public DateTime Date { get; set; }
        public string TeeName { get; set; }
        public int TeeTimeId { get; set; }
        public int Max { get; set; }
        public int Free { get; set; }
        public int Booked { get; set; }
        public int ConstraintCode { get; set; }
        public string Detail { get; set; }
        public decimal NormalFee { get; set; }
        public decimal ReductionFee { get; set; }
        public TeeTimeType Type { get; set; }
        public bool IsHoles9 { get; set; }
        public bool IsHoles18 { get; set; }
        public bool IsAllowed
        {
            get { return (ConstraintCode == 0) && (Free > 0) && (Date > DateTime.Now); }
        }
        public int ReductionPercent
        {
            get
            {
                return (int)Math.Round((NormalFee - ReductionFee) * 100 / NormalFee);
            }
        }
    }
}