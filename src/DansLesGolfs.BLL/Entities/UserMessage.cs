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
    
    public partial class UserMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserMessage()
        {
            this.UserMessageAttacheds = new HashSet<UserMessageAttached>();
        }
    
        public long MessageId { get; set; }
        public Nullable<int> MessageTypeId { get; set; }
        public string Subject { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public string Body { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public Nullable<System.DateTime> ReadDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> HasAttachedFile { get; set; }
        public Nullable<bool> IsFlag { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMessageAttached> UserMessageAttacheds { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
