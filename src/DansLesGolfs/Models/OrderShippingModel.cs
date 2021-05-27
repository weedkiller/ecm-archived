using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DansLesGolfs.Models
{
    public class OrderShippingModel
    {
        private string transactionId = string.Empty;
        private bool paypalSuccess = false;

        public ShoppingCart Cart { get; set; }
        public string OrderNumber { get; set; }
        public string TransactionId
        {
            get
            {
                if (String.IsNullOrEmpty(transactionId) || String.IsNullOrWhiteSpace(transactionId))
                    GenerateTransactionId();
                return transactionId;
            }
            set { transactionId = value; }
        }
        public long CustomerId { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public int CountryId { get; set; }
        public string Phone { get; set; }
        public string PhoneCountryCode { get; set; }
        public string MobilePhone { get; set; }
        public string MobilePhoneCountryCode { get; set; }
        public string MobilePhoneComplete
        {
            get { return "+" + MobilePhoneCountryCode.Replace("+", "") + (MobilePhone.Length > 1 && MobilePhone.StartsWith("0") ? MobilePhone.Substring(1) : MobilePhone); }
        }
        public string ShippingAddress { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public int ShippingCountryId { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingPhoneCountryCode { get; set; }
        public string ShippingPhoneComplete
        {
            get { return "+" + ShippingPhoneCountryCode.Replace("+", "") + ShippingPhone; }
        }
        public long AddressId { get; set; }
        public int DlgCardStyleId { get; set; }
        public bool PaypalSuccess
        {
            get { return paypalSuccess; }
            set { paypalSuccess = value; }
        }
        public string LydiaRequestId { get; set; }

        public OrderShippingModel()
        {
            GenerateTransactionId();
        }

        public Order GetOrder()
        {
            if (TransactionId == null || OrderNumber == null)
                GenerateTransactionId();

            // Create order.
            Order order = new Order()
            {
                OrderId = 0,
                OrderNumber = this.OrderNumber,
                OrderDate = DateTime.Now,
                AddressId = this.AddressId,
                CustomerId = CustomerId,
                TransactionId = TransactionId,
                RequestId = this.LydiaRequestId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                PaymentType = "creditcard",
                PaymentStatus = "pending",
                Active = true,
                IsNoticed = false,
                CouponId = 0,
                ReductionRate = 0,
                ReductionType = 0
            };

            if(Cart.HasCoupon && Cart.Coupon.CouponGroup != null)
            {
                order.CouponId = Cart.Coupon.CouponId;
                order.ReductionRate = Cart.Coupon.CouponGroup.Reduction;
                order.ReductionType = Cart.Coupon.CouponGroup.CouponType;
            }

            // Create order items.
            order.OrderItems = new List<OrderItem>();
            OrderItem tempOrderItem = null;
            foreach (CartItem i in Cart.Items)
            {
                tempOrderItem = new OrderItem()
                {
                    OrderItemId = 0,
                    OrderId = order.OrderId,
                    ItemId = i.ItemId,
                    SiteId = i.Item.SiteId,
                    VatId = i.Item.TaxId,
                    VatRate = i.Item.TaxRate,
                    ItemName = i.Description,
                    UnitPrice = i.UnitPrice,
                    ShippingCost = i.ShippingCost,
                    ReserveDate = i.ReserveDate,
                    Quantity = i.Quantity,
                    ItemCouponId = 0,
                    ReductionRate = 0,
                    ReductionType = 0
                };
                if(i.HasCoupon)
                {
                    tempOrderItem.ItemCouponId = i.Coupon.CouponId;
                    tempOrderItem.ReductionType = i.Coupon.CouponGroup.CouponType;
                    tempOrderItem.ReductionRate = i.Coupon.CouponGroup.Reduction;
                }
                order.OrderItems.Add(tempOrderItem);
            }
            return order;
        }

        public void Clear(long customerId = 0)
        {
            Cart.Items.Clear();
            TransactionId = "";
            CustomerId = customerId;
            AddressId = 0;
            paypalSuccess = false;
            LydiaRequestId = string.Empty;
            GenerateTransactionId();
        }

        public void GenerateTransactionId()
        {
            this.TransactionId = DateTime.Now.ToString("yyMMddHHmm000");
            this.OrderNumber = StringHelper.RandomString(5);
        }
    }
}