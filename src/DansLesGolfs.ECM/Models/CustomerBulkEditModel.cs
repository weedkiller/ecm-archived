using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DansLesGolfs.BLL;

namespace DansLesGolfs.ECM.Models
{
    public class CustomerBulkEditModel
    {
        public List<CustomerBulkEditRecord> rows = new List<CustomerBulkEditRecord>();

        public CustomerBulkEditModel()
        {
            
        }

        public void Add(User customer)
        {
            rows.Add(new CustomerBulkEditRecord(customer));
        }
    }

    public class CustomerBulkEditRecord
    {
        public long id { get; set; }
        public List<string> data { get; set; }
        public CustomerBulkEditRecord()
        {
            data = new List<string>();
        }
        public CustomerBulkEditRecord(User user)
        {
            this.id = user.UserId;
            data = new List<string>();
            //data.Add("<input type=\"checkbox\" name=\"ids\" class=\"checkbox\" value=\"" + user.UserId + "\" />");
            data.Add(user.Email);
            data.Add(user.FirstName);
            data.Add(user.LastName);
            data.Add(user.Gender == 0 ? Resources.Resources.Male : Resources.Resources.Female);
            data.Add(user.Birthdate.HasValue ? user.Birthdate.Value.ToString("dd/MM/yyyy") : DateTime.Today.ToString("d/M/yyyy"));
            data.Add(user.LicenseNumber);
            data.Add(user.Career);
            data.Add(user.Index.ToString());
            data.Add(user.Remarks);
            data.Add(user.Address);
            data.Add(user.City);
            data.Add(user.CountryName);
            data.Add(user.Phone);
            data.Add(user.MobilePhone);
            data.Add(user.CustomField1);
            data.Add(user.CustomField2);
            data.Add(user.CustomField3);
        }
    }
}