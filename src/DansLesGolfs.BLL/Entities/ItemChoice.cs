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
    
    public partial class ItemChoice
    {
        public long ItemId { get; set; }
        public int ModifierId { get; set; }
        public int ChoiceId { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual Modifier Modifier { get; set; }
        public virtual ModifierChoice ModifierChoice { get; set; }
    }
}
