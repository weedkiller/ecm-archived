using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    #region Ad
    public partial class Ad
    {
        public string AdsetName { get; set; }
    }
    #endregion

    #region Address
    public partial class Address
    {
        public string CityName { get; set; }
        public string Digicode { get; set; }
    }
    #endregion

    #region Brand
    public partial class Brand
    {
        public string BrandImage { get; set; }
    }
    #endregion

    #region Coupon
    public partial class Coupon
    {
        private bool isAvailable = true;
        public bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }
    }
    #endregion

    #region CouponGroup
    public partial class CouponGroup
    {
        public List<int> ItemCategoryIds { get; set; }
        public List<int> ItemTypeIds { get; set; }
    }
    #endregion

    #region Site
    public partial class Site
    {
        public string SiteName { get; set; }
        public string Description { get; set; }
        public string PracticalInfo { get; set; }
        public string Accommodation { get; set; }
        public string Restaurant { get; set; }
        public float AverageRating { get; set; }
        public int ReviewNumber { get; set; }
        public int LangId { get; set; }
    }
    #endregion

    #region SiteReview
    public partial class SiteReview
    {
        public string SiteName { get; set; }
        public string SiteSlug { get; set; }
    }
    #endregion

    #region User
    public partial class User
    {
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public string PasswordEncrypted { get; set; }
        public string CustomerTypeName { get; set; }
        public string SubCustomerTypeName { get; set; }
        public string CountryName { get; set; }
        public string ShippingCountryName { get; set; }
        public string SiteName { get; set; }
    }
    #endregion

    #region Item
    public partial class Item
    {
        public string CategoryName { get; set; }
        public string SiteName { get; set; }
        public string DexterityName { get; set; }
        public string GenreName { get; set; }
        public string ShaftName { get; set; }
        public string ShapeName { get; set; }

        public string ShippingMinDate
        {
            get { return DateTime.Today.AddDays((double)ShippingTimeMin).ToString("dddd d MMMM"); }
        }

        public string ShippingMaxDate
        {
            get { return DateTime.Today.AddDays((double)ShippingTimeMax).ToString("dddd d MMMM"); }
        }

        private string itemName;
        public string ItemName
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    if (this.ItemLangs != null && ItemLangs.Any())
                    {
                        itemName = ItemLangs.First().ItemName;
                    }
                }
                return itemName;
            }
            set
            {
                itemName = value;
            }
        }

        private string invoiceName;
        public string InvoiceName
        {
            get
            {
                if (String.IsNullOrEmpty(invoiceName))
                {
                    if (this.ItemLangs != null && ItemLangs.Any())
                    {
                        invoiceName = ItemLangs.First().InvoiceName;
                    }
                }
                return invoiceName;
            }
            set
            {
                invoiceName = value;
            }
        }

        public bool HasSpecialPrice
        {
            get
            {
                return SpecialPrice > 0;
            }
        }

        public bool HasPeriodPrice
        {
            get { return PeriodPrice > 0; }
        }

        public decimal SpecialPrice { get; set; }

        public decimal PeriodPrice { get; set; }

        public decimal CheapestPeriodPrice { get; set; }

        public bool HasCheapestPeriodPrice
        {
            get { return CheapestPeriodPrice > 0; }
        }

        public decimal CheapestPrice
        {
            get
            {
                if (SpecialPrice > 0)
                {
                    return SpecialPrice;
                }
                else if (PeriodPrice > 0)
                {
                    return PeriodPrice;
                }
                else
                {
                    return Price;
                }
            }
        }

        public decimal? TeeSheetCheapestPrice { get; set; }

        public bool HasCheapestPrice
        {
            get { return CheapestPrice > 0 && CheapestPrice < Price; }
        }

        public DateTime? ItemMinDate { get; set; }

        public DateTime? ItemMaxDate { get; set; }

        public string BrandName { get; set; }

        public string ShippingTypeName { get; set; }

        public string ItemTypeName { get; set; }

        public string ProductType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DlgCardStyleId { get; set; }

        public string PersonalMessage { get; set; }

        public string MessageFrom { get; set; }

        public string Email { get; set; }

        public int itemPriceDlgCardId { get; set; }

        public string ImageCardName { get; set; }

        public float TaxRate { get; set; }

        public bool HasTeeSheet { get; set; }

        public float AverageRating { get; set; }

        public DateTime? SpecialPriceStartDate { get; set; }

        public DateTime? SpecialPriceEndDate { get; set; }

        public int AlbatrosCourseId { get; set; }

        public string StateName { get; set; }
        public string SiteSlug { get; set; }
        public int ReviewNumber { get; set; }

        public List<SiteLang> SiteLangs { get; set; }
    }
    #endregion

    #region ItemReview
    public partial class ItemReview
    {
        public string ItemSlug { get; set; }
    }
    #endregion

    #region Order
    public partial class Order
    {
        #region GetBaseTotal
        public decimal GetBaseTotal(long? siteId = null)
        {
            if (siteId.HasValue)
            {
                return OrderItems != null && OrderItems.Any() ? OrderItems.Where(it => it.SiteId == siteId.Value).Sum(it => it.TotalBasePrice) : 0;
            }
            else
            {
                return OrderItems != null && OrderItems.Any() ? OrderItems.Sum(it => it.TotalBasePrice) : 0;
            }
        }
        #endregion

        #region GetSubTotal
        public decimal GetSubTotal(long? siteId = null)
        {
            decimal total = 0;

            IEnumerable<OrderItem> theItems = null;
            if (siteId.HasValue)
            {
                theItems = OrderItems != null && OrderItems.Any() ? OrderItems.Where(it => it.SiteId == siteId.Value) : new List<OrderItem>();
            }
            else
            {
                theItems = OrderItems != null && OrderItems.Any() ? OrderItems : new List<OrderItem>();
            }

            if (theItems != null && theItems.Any())
            {
                total = theItems.Sum(it => it.SubTotal);
            }
            return total;
        }
        #endregion

        #region GetTotalShippingCost
        public decimal GetTotalShippingCost(long? siteId = null)
        {
            if (siteId.HasValue)
            {
                return OrderItems != null && OrderItems.Any() ? OrderItems.Where(it => it.SiteId == siteId.Value).Sum(it => it.TotalShippingCost) : 0;
            }
            else
            {
                return OrderItems != null && OrderItems.Any() ? OrderItems.Sum(it => it.TotalShippingCost) : 0;
            }
        }
        #endregion

        #region GetTotalVAT
        public decimal GetTotalVAT()
        {
            return OrderItems != null && OrderItems.Any() ? OrderItems.Sum(it => it.TotalVAT) : 0;
        }
        #endregion

        #region GetTotalPrice
        public decimal GetTotalPrice(long? siteId = null)
        {
            if (siteId.HasValue)
            {
                return GetSubTotal(siteId) + GetTotalShippingCost(siteId);
            }
            else
            {
                return GetSubTotal() + GetTotalShippingCost();
            }
        }
        #endregion

        #region GetDiscount
        public decimal GetDiscount(long? siteId = null)
        {
            decimal discount = 0;
            if (CouponId > 0)
            {
                IEnumerable<OrderItem> theItems = null;
                if (siteId.HasValue)
                {
                    theItems = OrderItems != null && OrderItems.Any() ? OrderItems.Where(it => it.SiteId == siteId.Value) : new List<OrderItem>();
                }
                else
                {
                    theItems = OrderItems != null && OrderItems.Any() ? OrderItems : new List<OrderItem>();
                }

                if (theItems != null && theItems.Any())
                {
                    discount = theItems.Sum(it => it.TotalDiscount);
                }
            }
            return discount;
        }
        #endregion
    }
    #endregion

    #region OrderItem
    public partial class OrderItem
    {
        public string SiteEmail { get; set; }

        public decimal TotalBasePrice
        {
            get
            {
                if (Quantity.HasValue && UnitPrice.HasValue)
                    return Quantity.Value * UnitPrice.Value;
                else
                    return 0;
            }
        }
        public decimal SubTotal
        {
            get
            {
                if (Quantity.HasValue)
                    return Quantity.Value * PriceAfterDiscount;
                else
                    return 0;

            }
        }
        public decimal TotalPrice
        {
            get { return SubTotal + TotalShippingCost; }
        }
        public decimal TotalVAT
        {
            get { return TotalPrice / (((decimal)VatRate + 100) / 100); }
        }
        public decimal TotalShippingCost
        {
            get
            {
                if (Quantity.HasValue && ShippingCost.HasValue)
                    return (Quantity.Value * ShippingCost.Value);
                else
                    return 0;

            }
        }
        public decimal TotalDiscount
        {
            get
            {
                if (Quantity.HasValue)
                    return Quantity.Value * Discount;
                else
                    return 0;
            }
        }
        public decimal Discount
        {
            get
            {
                if (UnitPrice.HasValue)
                {

                }
                else
                {
                    return 0;
                }
                decimal discount = 0;
                decimal unitPrice = UnitPrice ?? 0;
                decimal reductionRate = ReductionRate ?? 0;
                switch ((CouponType)ReductionType)
                {
                    case CouponType.Amount:
                        discount = reductionRate;
                        break;
                    case CouponType.Percent:
                        discount = unitPrice * reductionRate / 100;
                        break;
                }
                return discount > unitPrice ? unitPrice : discount;
            }
        }
        public decimal PriceAfterDiscount
        {
            get
            {
                if (UnitPrice.HasValue)
                    return UnitPrice.Value - Discount;
                else
                    return 0;
            }
        }
        public string CategoryName { get; set; }

        public string ConversionTrackingCode { get; set; }
    }
    #endregion

    #region DLGCard
    public partial class DLGCard
    {
        public long CardId { get; set; }
        public string ItemCode { get; set; }
        public string ImageName { get; set; }
        public string PersonalMessage { get; set; }
        public int ToltalItemCount { get; set; }
        public decimal Amount { get; set; }
        public int Qualtity { get; set; }
    }
    #endregion

    #region DLGCardStyle
    public partial class DLGCardStyle
    {
        public long CardId { get; set; }
        public string BaseName { get; set; }
        public string FileExtension { get; set; }
    }
    #endregion

    #region CustomerType
    public partial class CustomerType
    {
        public string CustomerTypeName { get; set; }
    }
    #endregion

    #region EmailTemplate
    public partial class EmailTemplate
    {
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public byte[] HtmlDetail
        {
            get
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    return new byte[0];
                }
                else
                {
                    return EmailTemplateLangs.First().HtmlDetail;
                }
            }
            set
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    EmailTemplateLangs = new List<EmailTemplateLang>();
                }
                EmailTemplateLangs.First().HtmlDetail = value;
            }
        }
        public string HtmlDetailString
        {
            get
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    return string.Empty;
                }
                else
                {
                    return EmailTemplateLangs.First().HtmlDetailString;
                }
            }
            set
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    EmailTemplateLangs = new List<EmailTemplateLang>();
                }
                EmailTemplateLangs.First().HtmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }
        public byte[] TextDetail
        {
            get
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    return new byte[0];
                }
                else
                {
                    if (EmailTemplateLangs != null && EmailTemplateLangs.Any() && EmailTemplateLangs.First().TextDetail != null)
                        return Encoding.ASCII.GetBytes(EmailTemplateLangs.FirstOrDefault().TextDetail);
                    else
                        return new byte[0];
                }
            }
            set
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    EmailTemplateLangs = new List<EmailTemplateLang>();
                }
                EmailTemplateLangs.First().TextDetail = Encoding.UTF8.GetString(value);
            }
        }
        public string TextDetailString
        {
            get
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    return string.Empty;
                }
                else
                {
                    return EmailTemplateLangs.First().TextDetail;
                }
            }
            set
            {
                if (EmailTemplateLangs == null || !EmailTemplateLangs.Any())
                {
                    EmailTemplateLangs = new List<EmailTemplateLang>();
                }
                EmailTemplateLangs.First().TextDetail = value;
            }
        }
    }
    #endregion

    #region EmailTemplateLang
    public partial class EmailTemplateLang
    {
        public string HtmlDetailString
        {
            get
            {
                if (HtmlDetail == null)
                {
                    HtmlDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(HtmlDetail);
                }
            }
            set
            {
                HtmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }
    }
    #endregion

    #region Emailing
    public partial class Emailing
    {
        public string TemplateName { get; set; }
        public bool IsSendToAllCustomer { get; set; }
        public string CustomerGroupIds { get; set; }
        public string MjmlDetailString
        {
            get
            {
                if (MjmlDetail == null)
                {
                    MjmlDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(MjmlDetail);
                }
            }
            set
            {
                MjmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }
        public string HtmlDetailString
        {
            get
            {
                if (HtmlDetail == null)
                {
                    HtmlDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(HtmlDetail);
                }
            }
            set
            {
                HtmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }
        public string TextDetailString
        {
            get
            {
                if (TextDetail == null)
                {
                    TextDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(TextDetail);
                }
            }
            set
            {
                TextDetail = Encoding.UTF8.GetBytes(value);
            }
        }
        public List<String> OtherEmails = new List<String>();
    }
    #endregion

    #region EmailQue
    public partial class EmailQue
    {
        public string HtmlDetailString
        {
            get
            {
                if (HtmlDetail == null)
                {
                    HtmlDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(HtmlDetail);
                }
            }
            set
            {
                HtmlDetail = Encoding.UTF8.GetBytes(value);
            }
        }
        public string TextDetailString
        {
            get
            {
                if (TextDetail == null)
                {
                    TextDetail = new byte[0];
                    return string.Empty;
                }
                else
                {
                    return Encoding.UTF8.GetString(TextDetail);
                }
            }
            set
            {
                TextDetail = Encoding.UTF8.GetBytes(value);
            }
        }
    }
    #endregion

    #region UserType
    public partial class UserType
    {
        public static class Type
        {
            public const int SuperAdmin = 0;
            public const int Admin = 1;
            public const int Customer = 2;
            public const int Reseller = 3;
            public const int SiteManager = 4;
            public const int Staff = 5;
            public const int Guest = -1;
        }
    }
    #endregion

    #region Menu
    public partial class MenuItem
    {
        public List<MenuItem> Children { get; set; }
    }
    #endregion

    #region SlideImage
    public partial class SlideImage
    {
        public string ImageUrl { get; set; }
    }
    #endregion

    #region ItemType
    public partial class ItemType
    {
        public enum Type
        {
            All = 0,
            Product = 1,
            GreenFee = 2,
            StayPackage = 3,
            GolfLesson = 4,
            DrivingRange = 5,
            DLGCard = 6
        }
    }
    #endregion

    #region CouponType
    public enum CouponType
    {
        Amount = 0,
        Percent = 1
    }
    #endregion

    #region CouponUsageType
    public enum CouponUsageType
    {
        PerOrder = 0,
        PerItems = 1,
        PerCategories = 2,
        PerItemType = 3
    }
    #endregion

    #region CouponUsagePeriodType
    public enum CouponUsagePeriodType
    {
        Total = 0,
        ByDay = 1,
        ByWeek = 2
    }
    #endregion

    #region ReservationAPIType
    public enum ReservationAPIType
    {
        DLG = 0,
        Albatros = 1,
        Prima = 2
    }
    #endregion

    #region SendMailUsing
    public enum SendMailUsing
    {
        None = 0,
        SMTP = 1,
        Netmessage = 2
    }
    #endregion
}
