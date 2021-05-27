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
    public class PinModel
    {
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string SiteUrl { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool HasGreenFees { get; set; }
        public bool HasStayPackages { get; set; }
        public bool HasGolfLessons { get; set; }
        public bool HasDrivingRanges { get; set; }
        public bool HasResellerProducts { get; set; }
        public string PinIcon { get; set; }
    }
}