using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class EmailTemplateViewModel
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public bool IsAvailable { get; set; }
    }
}
