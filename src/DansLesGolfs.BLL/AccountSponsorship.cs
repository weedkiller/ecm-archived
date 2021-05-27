using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class AccountSponsorship
    {
        public string SponsorEmailFrom { get; set; }

        public string SponsorContent { get; set; }

        public string EmailTo { get; set; }

        public string SubjectEmail { get; set; }

        public string SponsorFullName { get; set; }

        public List<GodChildren> GodChildren { get; set; }
    }
    public class GodChildren 
    {
        public string SponsorEmail { get; set; }
        public string SponsorDate { get; set; }
    }
}
