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
    
    public partial class EmailUnsubscription
    {
        public long EmailUnsubscriptionId { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> UnsubscribeReasonId { get; set; }
        public Nullable<long> EmailTrackingId { get; set; }
        public string OtherReason { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
