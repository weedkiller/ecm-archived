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
    
    public partial class ModifierLang
    {
        public int ModifierId { get; set; }
        public int LangId { get; set; }
        public string ModifierName { get; set; }
        public string ModifierDesc { get; set; }
    
        public virtual Language Language { get; set; }
        public virtual Modifier Modifier { get; set; }
    }
}
