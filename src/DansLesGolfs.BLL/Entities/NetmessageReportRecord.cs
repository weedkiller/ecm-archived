//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DansLesGolfs.BLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class NetmessageReportRecord
    {
        public long NetmessageReportRecordId { get; set; }
        public long NetmessageReportId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Description { get; set; }
        public string Profession { get; set; }
        public string Index { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> OpenTime { get; set; }
        public Nullable<System.DateTime> UnsubTime { get; set; }
        public Nullable<System.DateTime> SubTime { get; set; }
        public Nullable<System.DateTime> RadiateTime { get; set; }
        public Nullable<System.DateTime> ViewTime { get; set; }
        public Nullable<System.DateTime> ClickTime { get; set; }
        public Nullable<bool> IsOpen { get; set; }
        public Nullable<bool> IsUnsub { get; set; }
        public Nullable<bool> IsSub { get; set; }
        public Nullable<bool> IsRadiate { get; set; }
        public Nullable<bool> IsView { get; set; }
        public Nullable<bool> IsClick { get; set; }
        public Nullable<bool> IsSent { get; set; }
        public Nullable<System.DateTime> SentTime { get; set; }
    
        public virtual NetmessageReport NetmessageReport { get; set; }
    }
}