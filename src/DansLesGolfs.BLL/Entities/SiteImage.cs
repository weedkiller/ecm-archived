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
    
    public partial class SiteImage
    {
        public long SiteImageId { get; set; }
        public long SiteId { get; set; }
        public string ImageName { get; set; }
        public Nullable<int> ListNo { get; set; }
        public string BaseName { get; set; }
        public string FileExtension { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
