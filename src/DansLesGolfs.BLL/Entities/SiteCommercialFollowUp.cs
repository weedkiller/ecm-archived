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
    
    public partial class SiteCommercialFollowUp
    {
        public long SiteId { get; set; }
        public System.DateTime VisitDate { get; set; }
        public string ContractualTerms { get; set; }
        public string NeedsInCommunicationTools { get; set; }
        public string CardSeller { get; set; }
        public string HotelPartnerSeeking { get; set; }
        public string AdsSpotSelling { get; set; }
        public string SellGFOnline { get; set; }
        public string CentralPurchasingPoint { get; set; }
        public string FormationNeeds { get; set; }
        public string ConsultingNeeds { get; set; }
        public string FidelityShop { get; set; }
        public string RegionalProspectingPoint { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
