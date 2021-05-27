using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DansLesGolfs
{
    public class TransactionHelper
    {
        public static string GetPaymentTypeText(string paymentType)
        {
            string paymentTypeText = Resources.Resources.GetFree;
            switch (paymentType)
            {
                //case "lydia":
                //    paymentTypeText = "Lydia Mobile Payment";
                //    break;
                case "lydia":
                case "creditcard":
                    paymentTypeText = Resources.Resources.CreditCard;
                    break;
                case "paypal":
                    paymentTypeText = "Paypal";
                    break;
                case "check":
                    paymentTypeText = Resources.Resources.Check;
                    break;
                case "free":
                    paymentTypeText = Resources.Resources.GetFree;
                    break;
                default:
                    paymentTypeText = Resources.Resources.CreditCard;
                    break;
            }
            return paymentTypeText;
        }
        public static string GetCategoryText(int itemTypeId)
        {
            string categoryText = Resources.Resources.Product;
            switch (itemTypeId)
            {
                case (int)ItemType.Type.Product:
                    categoryText = Resources.Resources.Product;
                    break;
                case (int)ItemType.Type.GreenFee:
                    categoryText = Resources.Resources.GreenFees;
                    break;
                case (int)ItemType.Type.StayPackage:
                    categoryText = Resources.Resources.StayPackages;
                    break;
                case (int)ItemType.Type.GolfLesson:
                    categoryText = Resources.Resources.GolfLessons;
                    break;
                case (int)ItemType.Type.DrivingRange:
                    categoryText = Resources.Resources.DrivingRanges;
                    break;
                case (int)ItemType.Type.DLGCard:
                    categoryText = Resources.Resources.DLGCard;
                    break;
                default:
                    categoryText = Resources.Resources.Product;
                    break;
            }
            return categoryText;
        }
    }
}