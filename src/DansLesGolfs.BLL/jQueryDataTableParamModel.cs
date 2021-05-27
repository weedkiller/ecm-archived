using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class jQueryDataTableParamModel
    {    
        public string draw { get; set; }

        public string search { get; set; }

        public string moreConditions { get; set; }

        public int length { get; set; }

        public int start { get; set; }

        public int recordsTotal { get; set; }

        public int recordsFiltered { get; set; }

        public string error { get; set; }

        public int iSortingCols { get; set; }

        public int DlgCardId { get; set; }

        public string order { get; set; }
    }

}
