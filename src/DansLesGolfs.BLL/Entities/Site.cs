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
    
    public partial class Site
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Site()
        {
            this.Categories = new HashSet<Category>();
            this.Impressums = new HashSet<Impressum>();
            this.Items = new HashSet<Item>();
            this.ItemCategories = new HashSet<ItemCategory>();
            this.MailingLists = new HashSet<MailingList>();
            this.OrderItems = new HashSet<OrderItem>();
            this.SiteCentralLineSEOs = new HashSet<SiteCentralLineSEO>();
            this.SiteCommercialFollowUps = new HashSet<SiteCommercialFollowUp>();
            this.SiteCommunications = new HashSet<SiteCommunication>();
            this.SiteEvents = new HashSet<SiteEvent>();
            this.SiteFinancials = new HashSet<SiteFinancial>();
            this.SiteImages = new HashSet<SiteImage>();
            this.SiteLangs = new HashSet<SiteLang>();
            this.SiteRestaurantProductCategories = new HashSet<SiteRestaurantProductCategory>();
            this.SiteRestaurantSuppliers = new HashSet<SiteRestaurantSupplier>();
            this.SiteReviews = new HashSet<SiteReview>();
            this.Users = new HashSet<User>();
            this.Courses = new HashSet<Course>();
            this.CustomerGroups = new HashSet<CustomerGroup>();
            this.CustomerTypes = new HashSet<CustomerType>();
            this.Emailings = new HashSet<Emailing>();
        }
    
        public long SiteId { get; set; }
        public string SiteSlug { get; set; }
        public int GolfBrandId { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int StateId { get; set; }
        public int RegionId { get; set; }
        public int CountryId { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string FB { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> Active { get; set; }
        public long UserId { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<int> AlbatrosCourseId { get; set; }
        public string AlbatrosUrl { get; set; }
        public string AlbatrosUsername { get; set; }
        public string AlbatrosPassword { get; set; }
        public Nullable<bool> Visible { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public Nullable<int> SMTPPort { get; set; }
        public Nullable<bool> SMTPUseSSL { get; set; }
        public Nullable<bool> IsTrackingOpenMail { get; set; }
        public Nullable<bool> IsTrackingLinkClick { get; set; }
        public int ReservationAPI { get; set; }
        public Nullable<bool> IsUseGlobalSMTPSettings { get; set; }
        public string PrimaAPIKey { get; set; }
        public string PrimaClubKey { get; set; }
        public Nullable<bool> IsCommercial { get; set; }
        public Nullable<bool> IsAssociative { get; set; }
        public Nullable<bool> IsRegie { get; set; }
        public string ManageRestaurantFlag { get; set; }
        public string ManageProshopFlag { get; set; }
        public string ManageFieldFlag { get; set; }
        public string ManageGolfFlag { get; set; }
        public string ManagerPhone { get; set; }
        public string RespReceptionPhone { get; set; }
        public string GreenKeeperPhone { get; set; }
        public string RespProshopPhone { get; set; }
        public string RestaurateurPhone { get; set; }
        public string AssociationPresidentPhone { get; set; }
        public string ManagerEmail { get; set; }
        public string RespReceptionEmail { get; set; }
        public string GreenKeeperEmail { get; set; }
        public string RespProshopEmail { get; set; }
        public string RestaurateurEmail { get; set; }
        public string AssociationPresidentEmail { get; set; }
        public string CommercialNBSubscriber { get; set; }
        public string CommercialNBGF { get; set; }
        public string CommercialTurnover { get; set; }
        public string CommercialReservationSystem { get; set; }
        public string DefaultSenderName { get; set; }
        public string DefaultSenderEmail { get; set; }
        public string LydiaVendorToken { get; set; }
        public string LydiaVendorId { get; set; }
        public Nullable<bool> SMTPUseVERP { get; set; }
        public string BouncedReturnEmail { get; set; }
        public Nullable<bool> HotelOnSite { get; set; }
        public Nullable<bool> HotelPartner { get; set; }
        public Nullable<bool> IsUseGlobalNetmessageSettings { get; set; }
        public string NetmessageFTPUsername { get; set; }
        public string NetmessageFTPPassword { get; set; }
        public string NetmessageAccountName { get; set; }
        public Nullable<long> AlbatrosClubId { get; set; }
        public string ChronogolfClientId { get; set; }
        public string ChronogolfClientSecret { get; set; }
        public string ChronogolfRefreshToken { get; set; }
        public Nullable<int> ChronogolfClubId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }
        public virtual Country Country { get; set; }
        public virtual GolfBrand GolfBrand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Impressum> Impressums { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemCategory> ItemCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MailingList> MailingLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Region Region { get; set; }
        public virtual ReservationAPI ReservationAPI1 { get; set; }
        public virtual State State { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteCentralLineSEO> SiteCentralLineSEOs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteCommercialFollowUp> SiteCommercialFollowUps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteCommunication> SiteCommunications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteEvent> SiteEvents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteFinancial> SiteFinancials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteImage> SiteImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteLang> SiteLangs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteRestaurantProductCategory> SiteRestaurantProductCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteRestaurantSupplier> SiteRestaurantSuppliers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteReview> SiteReviews { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerGroup> CustomerGroups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerType> CustomerTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emailing> Emailings { get; set; }
    }
}
