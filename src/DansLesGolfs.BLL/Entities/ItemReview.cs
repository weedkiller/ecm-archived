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
    
    public partial class ItemReview
    {
        public long ItemReviewId { get; set; }
        public long ItemId { get; set; }
        public long UserId { get; set; }
        public Nullable<byte> Rating { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Subject { get; set; }
        public Nullable<bool> IsApproved { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual User User { get; set; }
    }
}