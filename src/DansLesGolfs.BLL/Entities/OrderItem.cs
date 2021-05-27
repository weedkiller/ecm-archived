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
    
    public partial class OrderItem
    {
        public long OrderItemId { get; set; }
        public string ItemName { get; set; }
        public long OrderId { get; set; }
        public long ItemId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public long SiteId { get; set; }
        public Nullable<System.DateTime> ReserveDate { get; set; }
        public int VatId { get; set; }
        public long ItemCouponId { get; set; }
        public Nullable<double> VatRate { get; set; }
        public Nullable<decimal> ReductionRate { get; set; }
        public Nullable<int> ReductionType { get; set; }
        public string RefCode { get; set; }
        public Nullable<long> PlayerId { get; set; }
    
        public virtual Coupon Coupon { get; set; }
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
        public virtual Site Site { get; set; }
        public virtual Tax Tax { get; set; }
    }
}
