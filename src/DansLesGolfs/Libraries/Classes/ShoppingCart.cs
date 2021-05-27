using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DansLesGolfs
{
    public class ShoppingCart
    {
        #region Members
        private Coupon coupon;
        private List<CartItem> items;
        public static ShoppingCart Instance
        {
            get
            {
                ShoppingCart cart = null;
                if (HttpContext.Current.Session["ASPShoppingCart"] == null || !(HttpContext.Current.Session["ASPShoppingCart"] is ShoppingCart))
                {
                    cart = new ShoppingCart();
                    cart.Items = new List<CartItem>();
                    //dlgcard
                    cart.addDlgCardCart = new List<ItemDlgCardCore>();
                    HttpContext.Current.Session["ASPShoppingCart"] = cart;
                }
                else
                {
                    cart = HttpContext.Current.Session["ASPShoppingCart"] as ShoppingCart;
                }
                return cart;
            }
        }
        #endregion

        #region Properties
        public Coupon Coupon
        {
            get { return coupon; }
            set
            {
                coupon = value;
                this.items.ForEach(it => { it.ClearCoupon(); }); // Clear exists coupon first.
                if (HasCoupon)
                {
                    if (coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerOrder)
                    {
                        //if (coupon.CouponGroup.MinimumAmount > 0 && this.GetTotalPrice() < coupon.CouponGroup.MinimumAmount)
                        //{
                        //    throw new Exception(Resources.Resources.CartMinimumAmountIsInvalidForThisCoupon);
                        //}
                        this.items.ForEach(it => it.Coupon = this.coupon);
                    }
                    else if (coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerItems)
                    {
                        Item tempItem = null;
                        foreach (CartItem item in this.Items)
                        {
                            tempItem = coupon.CouponGroup.Items.First(it => it.ItemId == item.ItemId);
                            if (tempItem != null)
                            {
                                item.Coupon = this.coupon;
                            }
                        }
                    }
                    else if (coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerCategories)
                    {
                        foreach (CartItem item in this.Items)
                        {
                            if (coupon.CouponGroup.ItemCategoryIds.Where(it => item.Item.CategoryId == it).Any())
                            {
                                item.Coupon = this.coupon;
                            }
                        }
                    }
                    else if (coupon.CouponGroup.CouponUsageType == (int)CouponUsageType.PerItemType)
                    {
                        foreach (CartItem item in this.Items)
                        {
                            if (coupon.CouponGroup.ItemTypeIds.Where(it => item.Item.ItemTypeId == it).Any())
                            {
                                item.Coupon = this.coupon;
                            }
                        }
                    }
                }
            }
        }
        public string CouponCode
        {
            get { return coupon == null ? string.Empty : coupon.CouponCode; }
        }
        public bool HasCoupon
        {
            get
            {
                return coupon != null && coupon.CouponGroup != null && (!String.IsNullOrEmpty(coupon.CouponCode) && !String.IsNullOrWhiteSpace(coupon.CouponCode));
            }
        }
        public List<CartItem> Items
        {
            get { return items; }
            set { items = value; }
        }
        public int Quantity
        {
            get { return items.Count; }
        }
        #endregion

        private ShoppingCart()
        {

        }

        public CartItem AddItem(Item item, int quantity = 1)
        {
            CartItem cartItem = new CartItem(item);
            pAddItem(cartItem, quantity, item.SiteName);
            //cartItem.Description = String.IsNullOrEmpty(customDescription) ? cartItem.Description : customDescription;

            //var result = Items.Where(it => it.ItemId == cartItem.ItemId && it.Description == cartItem.Description);
            //if(result != null && result.Any())
            //{
            //    result.First().Quantity += quantity;
            //}
            //else
            //{
            //    cartItem.Quantity = quantity;
            //    Items.Add(cartItem);
            //}
            return cartItem;
        }

        public void pAddItem(CartItem cartItem, int quantity = 1, string siteName = "")
        {
            var result = Items.Where(it => it.ItemId == cartItem.ItemId && it.Description == cartItem.Description && it.FirstName == cartItem.FirstName && it.LastName == cartItem.LastName && it.PersonalMessage == cartItem.PersonalMessage && it.MessageFrom == cartItem.MessageFrom && it.DlgCardStyleId == cartItem.DlgCardStyleId);
            if (result != null && result.Any())
            {
                result.First().Quantity += quantity;
            }
            else
            {
                cartItem.Quantity = quantity;
                Items.Add(cartItem);
            }
        }


        public void AddDLGItem(Item item, int itemPriceId, int quantity)
        {
            CartItem cartItem = new CartItem(item);
            cartItem.UseItemCodeAsDescription = true;

            var numbers = (from itemprice in item.ItemPrices
                           where itemprice.ItemPriceId == itemPriceId
                           select itemprice).ToArray();

            cartItem.UnitPrice = numbers[0].Price;

            pAddItem(cartItem, quantity, item.SiteName);
        }



        /// <summary>
        /// Get Item Quantity by ItemId
        /// </summary>
        /// <param name="itemId">ItemId</param>
        /// <returns>Quantity of specific Item.</returns>
        public int GetItemQuantity(long itemId)
        {
            var result = Items.Where(it => it.ItemId == itemId);
            if (result != null && result.Any())
            {
                return result.First().Quantity;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Set Item Quantity
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="quantity">Quantity</param>
        public bool SetItemQuantity(long itemId, int quantity)
        {
            if (quantity <= 0)
                RemoveItem(itemId);

            var result = Items.Where(it => it.ItemId == itemId);
            if (result != null && result.Any())
            {
                result.First().Quantity = quantity;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove item by ItemId
        /// </summary>
        /// <param name="itemId">ItemId that you want to remove.</param>
        public void RemoveItem(long itemId)
        {
            var result = Items.Where(it => it.ItemId == itemId);
            if (result != null && result.Any())
            {
                Items.Remove(result.First());
            }
        }

        public decimal GetBaseTotal()
        {
            return Items != null && Items.Any() ? Items.Sum(it => it.TotalBasePrice) : 0;
        }
        public decimal GetTotalShippingCost()
        {
            return Items != null && Items.Any() ? Items.Sum(it => it.TotalShippingCost) : 0;
        }

        /// <summary>
        /// Get Sub Total.
        /// </summary>
        /// <returns>Sub Total</returns>
        public decimal GetSubTotal()
        {
            return Items != null && Items.Any() ? Items.Sum(it => it.SubTotal) : 0;
        }

        public decimal GetTotalPrice()
        {
            return GetSubTotal() + GetTotalShippingCost();
        }

        public decimal GetDiscount()
        {
            return Items != null && Items.Any() ? Items.Sum(it => it.TotalDiscount) : 0;
        }

        public decimal GetDiscountMinimumAmount()
        {
            return HasCoupon ? Coupon.CouponGroup.MinimumAmount : 0;
        }

        public List<CartItem> GetItemsByItemId(long itemId)
        {
            List<CartItem> items = null;
            var result = this.Items.Where(it => it.ItemId == itemId);
            if (result != null && result.Any())
            {
                items = result.ToList();
            }
            else
            {
                items = new List<CartItem>();
            }
            return items;
        }

        public List<ItemDlgCardCore> addDlgCardCart { get; set; }
    }

    public class CartItem : IEqualityComparer<CartItem>
    {
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }

        public Item Item { get; set; }

        public string RefCode { get; set; }

        public string ItemImageUrl { get; set; }

        private decimal unitPrice;

        private decimal discountPrice;

        private decimal shippingCost;

        private bool useItemCodeAsDescription = false;

        private Coupon coupon;

        private Dictionary<string, object> specialData = new Dictionary<string, object>();

        public long ItemId
        {
            get
            {
                return Item.ItemId;
            }
        }

        public string Description
        {
            get
            {
                if (this.Item != null)
                {
                    string description = string.Empty;
                    if (UseItemCodeAsDescription)
                    {
                        description = String.IsNullOrEmpty(Item.InvoiceName) || String.IsNullOrWhiteSpace(Item.InvoiceName) ? Item.ItemCode : Item.InvoiceName;
                    }
                    else
                    {
                        description = String.IsNullOrEmpty(Item.InvoiceName) || String.IsNullOrWhiteSpace(Item.InvoiceName) ? Item.ItemName : Item.InvoiceName;
                    }
                    return description + (!string.IsNullOrEmpty(CustomDescription) && !string.IsNullOrWhiteSpace(CustomDescription) ? " " + CustomDescription.Trim() : string.Empty)
                        + (!string.IsNullOrEmpty(RefCode) && !string.IsNullOrWhiteSpace(RefCode) ? " (Ref: " + RefCode.Trim() + ")" : string.Empty);
                }
                else
                {
                    return Resources.Resources.Untitled + (!string.IsNullOrEmpty(CustomDescription) && !string.IsNullOrWhiteSpace(CustomDescription) ? " " + CustomDescription.Trim() : string.Empty);
                }
            }
        }

        public string SiteName
        {
            get
            {
                if (this.Item != null)
                {
                    return Item.SiteName;
                }
                else
                {
                    return Resources.Resources.Untitled;
                }
            }
        }

        public string CustomDescription { get; set; }

        public decimal UnitPrice
        {
            get
            {
                return unitPrice;
            }
            set
            {
                unitPrice = value;
            }
        }

        public decimal TotalBasePrice
        {
            get { return Quantity * unitPrice; }
        }

        public decimal ShippingCost
        {
            get
            {
                return shippingCost;
            }
            set
            {
                shippingCost = value;
            }
        }
        public decimal TotalShippingCost
        {
            get { return (Quantity * shippingCost); }
        }
        public decimal Discount
        {
            get
            {
                decimal discount = 0;
                if (HasCoupon)
                {
                    if (Coupon.CouponGroup.MinimumAmount > 0)
                    {
                        if (Coupon.CouponGroup.MinimumAmount <= TotalBasePrice)
                        {
                            discount = DiscountPerUnit;
                        }
                    }
                    else
                    {
                        discount = DiscountPerUnit;
                    }
                }
                return discount > unitPrice ? unitPrice : discount;
            }
        }
        public decimal DiscountPerUnit
        {
            get
            {
                decimal discount = 0;
                if (HasCoupon)
                {
                    switch ((CouponType)Coupon.CouponGroup.CouponType)
                    {
                        case CouponType.Amount:
                            discount = Coupon.CouponGroup.Reduction;
                            break;
                        case CouponType.Percent:
                            discount = unitPrice * Coupon.CouponGroup.Reduction / 100;
                            break;
                    }
                }
                return discount > unitPrice ? unitPrice : discount;
            }
        }
        public decimal TotalDiscount
        {
            get { return Quantity * Discount; }
        }
        public decimal PriceAfterDiscount
        {
            get
            {
                return UnitPrice - Discount;
            }
        }
        public decimal SubTotal
        {
            get { return Quantity * PriceAfterDiscount; }
        }
        public decimal TotalPrice
        {
            get { return SubTotal + TotalShippingCost; }
        }

        public Dictionary<string, object> SpecialData
        {
            get { return specialData; }
            set { specialData = value; }
        }

        public Coupon Coupon
        {
            get { return coupon; }
            set
            {
                if (value != null)
                {
                    coupon = value;
                    if (HasCoupon)
                    {
                        switch ((CouponType)coupon.CouponGroup.CouponType)
                        {
                            case CouponType.Amount:
                                discountPrice = unitPrice - coupon.CouponGroup.Reduction;
                                break;
                            case CouponType.Percent:
                                discountPrice = unitPrice - (unitPrice * coupon.CouponGroup.Reduction / 100);
                                break;
                        }
                    }
                }
            }
        }

        public bool HasCoupon
        {
            get
            {
                return Coupon != null && Coupon.CouponGroup != null;
            }
        }

        public CartItem(Item item)
            : this(item, 1)
        {
        }

        public CartItem(Item item, int quantity)
        {
            this.Item = item;
            this.unitPrice = item.CheapestPrice;
            this.shippingCost = item.ShippingCost;
            this.FirstName = item.FirstName;
            this.LastName = item.LastName;
            this.PersonalMessage = item.PersonalMessage;
            this.MessageFrom = item.MessageFrom;
            this.DlgCardStyleId = item.DlgCardStyleId;
            this.Email = item.Email;
            this.itemPriceDlgCardId = item.itemPriceDlgCardId;
            this.Quantity = quantity;
        }

        public bool Equals(CartItem cartItem)
        {
            return cartItem.ItemId == this.ItemId;
        }

        public bool Equals(CartItem x, CartItem y)
        {
            return x.ItemId == y.ItemId;
        }

        public int GetHashCode(CartItem obj)
        {
            return obj.GetHashCode();
        }

        public string ProductType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int DlgCardStyleId { get; set; }

        public string PersonalMessage { get; set; }

        public string MessageFrom { get; set; }

        public string Email { get; set; }

        public int itemPriceDlgCardId { get; set; }

        public DateTime? ReserveDate { get; set; }

        public bool UseItemCodeAsDescription
        {
            get
            {
                return useItemCodeAsDescription;
            }
            set
            {
                useItemCodeAsDescription = value;
            }
        }

        public void ClearCoupon()
        {
            this.coupon = null;
        }
    }
}
