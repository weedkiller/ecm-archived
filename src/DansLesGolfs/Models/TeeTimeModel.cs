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
    public class TeeTimeModel
    {
        public int TeeId { get; set; }
        public string TeeName { get; set; }
        public string SiteName { get; set; }

        public TeeTimeModel()
        {

        }

        public TeeTimeModel(int id, string name, string site)
        {
            TeeId = id;
            TeeName = name;
            SiteName = site;
        }
    }
}