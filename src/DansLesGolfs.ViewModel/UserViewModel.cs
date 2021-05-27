using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class UserViewModel
    {
        public long UserId { get; set; }
        public int UserTypeId { get; set; }
        public long SiteId { get; set; }
        public long CustomerTypeId { get; set; }
        public long SubCustomerTypeId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerTypeName { get; set; }
        public string SubCustomerTypeName { get; set; }
        public string SiteName { get; set; }
        public int Gender { get; set; }
        public string GenderName { get; set; }

        public DateTime BirthDate { get; set; }

        public int Age
        {
            get
            {
                return new DateTime((DateTime.Now - BirthDate).Ticks).Year;
            }
        }

        public DateTime? InsertDate { get; set; }
    }
}
