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
    
    public partial class Modifier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Modifier()
        {
            this.ItemChoices = new HashSet<ItemChoice>();
            this.ItemModifiers = new HashSet<ItemModifier>();
            this.ModifierChoices = new HashSet<ModifierChoice>();
            this.ModifierLangs = new HashSet<ModifierLang>();
        }
    
        public int ModifierId { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<short> ControlType { get; set; }
        public Nullable<bool> Active { get; set; }
        public long UserId { get; set; }
        public int ItemTypeId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemChoice> ItemChoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemModifier> ItemModifiers { get; set; }
        public virtual ItemType ItemType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModifierChoice> ModifierChoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModifierLang> ModifierLangs { get; set; }
        public virtual User User { get; set; }
    }
}
