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
    
    public partial class EmailTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmailTemplate()
        {
            this.EmailTemplateAttachments = new HashSet<EmailTemplateAttachment>();
            this.EmailTemplateLangs = new HashSet<EmailTemplateLang>();
            this.Emailings = new HashSet<Emailing>();
        }
    
        public int TemplateId { get; set; }
        public string TemplateKey { get; set; }
        public int CategoryId { get; set; }
        public long UserId { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string FileName1 { get; set; }
        public string FileName2 { get; set; }
        public string FileName3 { get; set; }
        public string FileName4 { get; set; }
        public string FileName5 { get; set; }
        public string FileDescription1 { get; set; }
        public string FileDescription2 { get; set; }
        public string FileDescription3 { get; set; }
        public string FileDescription4 { get; set; }
        public string FileDescription5 { get; set; }
        public string FileUrl1 { get; set; }
        public string FileUrl2 { get; set; }
        public string FileUrl3 { get; set; }
        public string FileUrl4 { get; set; }
        public string FileUrl5 { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailTemplateAttachment> EmailTemplateAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailTemplateLang> EmailTemplateLangs { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emailing> Emailings { get; set; }
    }
}
