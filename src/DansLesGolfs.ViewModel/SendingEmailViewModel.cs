using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class SendingEmailViewModel
    {
        public long EmailId { get; set; }
        public long EmailQueId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Status { get; set; }
        public bool? IsOpened { get; set; }
        public bool? IsClicked { get; set; }
        public bool? IsUnsubscribed { get; set; }
        public bool? IsBounced { get; set; }
    }
}
