using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;

namespace DansLesGolfs.ECM.Models
{
    public class PickerModel
    {
        public string PrimaryKeyName { get; set; }
        public Dictionary<string, string> Columns { get; set; }
        public List<object> Data { get; set; }

        public PickerModel()
        {
            PrimaryKeyName = string.Empty;
            Columns = new Dictionary<string, string>();
            Data = new List<object>();
        }
    }
}