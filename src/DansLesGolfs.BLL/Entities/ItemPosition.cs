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
    
    public partial class ItemPosition
    {
        public long ItemId { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
    
        public virtual Item Item { get; set; }
    }
}