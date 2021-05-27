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
    
    public partial class Emailing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Emailing()
        {
            this.EmailingAttachments = new HashSet<EmailingAttachment>();
            this.EmailTrackings = new HashSet<EmailTracking>();
            this.EmailQues = new HashSet<EmailQue>();
        }
    
        public long EmailId { get; set; }
        public string EmailName { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public Nullable<int> EmailFormatId { get; set; }
        public int TemplateId { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public long UserId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public long SiteId { get; set; }
        public byte[] MjmlDetail { get; set; }
        public byte[] HtmlDetail { get; set; }
        public byte[] TextDetail { get; set; }
        public Nullable<int> SendMailUsing { get; set; }
        public Nullable<System.DateTime> ScheduleDateTime { get; set; }
    
        public virtual EmailTemplate EmailTemplate { get; set; }
        public virtual Site Site { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailingAttachment> EmailingAttachments { get; set; }
        public virtual EmailingList EmailingList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailTracking> EmailTrackings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailQue> EmailQues { get; set; }
    }
}
