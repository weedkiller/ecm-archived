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
    
    public partial class SiteFinancial
    {
        public long SiteId { get; set; }
        public int FiscalYear { get; set; }
        public string MemberAmount { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string ClassicCard { get; set; }
        public string ClassicCardTurnover { get; set; }
        public string GolfCardTurnover { get; set; }
        public string ClassicCorpoTurnover { get; set; }
        public string RFAPractice { get; set; }
        public string RFAProshop { get; set; }
        public string RFAField { get; set; }
        public string RFAServices { get; set; }
        public string RFACarts { get; set; }
        public string RFARestauration { get; set; }
        public string RFATotal { get; set; }
        public string AdsPackVisibility { get; set; }
        public string AdsAdditionalServices { get; set; }
        public string AdsShow { get; set; }
        public string CommisionGFOnline { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
