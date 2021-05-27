using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Base;

namespace DansLesGolfs.Controllers
{
    public class DLGCardController : BaseFrontController
    {
        private readonly int ItemCardTypeId = 6;

        public DLGCardController()
        {
            ViewBag.BodyClasses = "product green-fee";
        }

        public ActionResult Index()
        {
            var culturecookie = ViewBag.CurrentCulture != null ? (string)ViewBag.CurrentCulture : "en-US";

            if (culturecookie == "fr-FR")
            {
                return View("DlgOffer_fr");
            }
            else
            {
                return View("DlgOffer_en");
            }            
        }

        public ActionResult DlgOrder()
        {
            
            ItemDlgCardCore vm = new ItemDlgCardCore();
            vm = DlgCardItem();

            if (vm != null)
            {
                return View("DlgOrder", vm);
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult SaveDLGCardDetail(ItemDlgCardCore vm)
        {

            if (ModelState.IsValid)
            {
                ShoppingCart cart = ShoppingCart.Instance;
                Item item = DataAccess.GetItem(vm.ItemId);

                item.ProductType = "dlgcard";
                item.FirstName = vm.Firstname;
                item.LastName = vm.Lastname;
                item.PersonalMessage = vm.PersonalMessage;
                item.MessageFrom = vm.MessageFrom;
                item.Email = vm.Email;
                item.itemPriceDlgCardId = vm.itemPriceDlgCardId;

                var listStyleimage = DataAccess.GetDLGCardStyle(vm.ItemId);

                item.ItemImages = new List<ItemImage>();

                foreach (var data in listStyleimage)
                {
                    if (data.CardStyleId == vm.DlgCardStyleId)
                    {
                        item.ItemImages.Add(new ItemImage
                        {
                           ImageName = data.ImageName,
                           BaseName = data.ImageName
                        });
                    }
                }

                item.DlgCardStyleId = vm.DlgCardStyleId;

                if (item != null)
                {

                    cart.AddDLGItem(item,vm.itemPriceDlgCardId,vm.Quantity); ;
                    cart.addDlgCardCart.Add(vm);
                }

                //for (int i = 0; i < vm.Quantity; i++)
                //{
                //    DataAccess.SaveDlgCard(vm.ItemId, vm.Firstname, vm.Lastname, vm.Email, vm.PersonalMessage, vm.itemPriceDlgCardId, 1, vm.DlgCardStyleId);
                //}

                return RedirectToAction("index", "cart");
            }

            return View("DlgOrder", vm);
        }

        [HttpPost]
        public PartialViewResult GetItemPriceDlgCardPartial(int itemId)
        {
            ItemDlgCardCore itemDlgCardCore = new ItemDlgCardCore();
            itemDlgCardCore.ItemId = itemId;
            //get item price by itemid
            List<ItemPriceDlgCard> itemPriceDlgCard = DataAccess.GetItemPriceDlgCard(itemId);
            itemDlgCardCore.itemPriceDlgCardId = itemPriceDlgCard[0].ItemPriceId;

            itemDlgCardCore.ItemPriceDlgCardList = itemPriceDlgCard;

            return PartialView("_itemPricePartial", itemDlgCardCore);
        }

        [HttpPost]
        public PartialViewResult GetDLGCardStylePartial(int itemId)
        {
            ItemDlgCardCore itemDlgCardCore = new ItemDlgCardCore();
            itemDlgCardCore.ItemId = itemId;

            List<DLGCardStyle> dLGCardStyleList = DataAccess.GetDLGCardStyle(itemId);
            itemDlgCardCore.DLGCardStyleList = dLGCardStyleList;

            return PartialView("_CardStylePartial", itemDlgCardCore);
        }



        private ItemDlgCardCore DlgCardItem()
        {

            ItemDlgCardCore itemDlgCardCore = new ItemDlgCardCore();
            itemDlgCardCore.ItemTypeId = ItemCardTypeId;

            //get item dlg card
            List<ItemDlgCard> itemDlgCard = DataAccess.GetItemDlgCardByItemTypeId(6,0,null,null,null,null,null);

            if (itemDlgCard.Count > 0)
            {
                itemDlgCardCore.ItemId = itemDlgCard[0].ItemId;
                itemDlgCardCore.PersonalMessage = itemDlgCard[0].PersonalMessage;

                //get item price by itemid
                List<ItemPriceDlgCard> itemPriceDlgCard = DataAccess.GetItemPriceDlgCard(itemDlgCardCore.ItemId);
                itemDlgCardCore.itemPriceDlgCardId = itemPriceDlgCard[0].ItemPriceId;

                itemDlgCardCore.ItemDlgCardList = itemDlgCard;
                itemDlgCardCore.ItemPriceDlgCardList = itemPriceDlgCard;

                List<DLGCardStyle> dLGCardStyleList = DataAccess.GetDLGCardStyle(itemDlgCardCore.ItemId);
                itemDlgCardCore.DLGCardStyleList = dLGCardStyleList;

                if (Auth.User != null)
                {
                    itemDlgCardCore.MessageFrom = Auth.User.Fullname;
                }

                return itemDlgCardCore;

            }
            else
            {
                return null;
            }
        }

        //public ActionResult testsendemail()
        //{
        //    User user = new BLL.User();
        //    user.Active = true;
        //    user.Address = "kid";
        //    user.Birthdate = DateTime.UtcNow;
        //    user.City = "city";
        //    user.CountryId = 1;
        //    user.CustomerTypeId = 1;
        //    user.Email = "stdsitsuwan2008@gmail.com";
        //    user.ExpiredDate = DateTime.UtcNow.AddDays(200);
        //    user.Firstname = "kid";
        //    user.Lastname = "kid";
        //    user.Middlename = "xx";

        //    EmailArguments emailArgs = new EmailArguments();
        //    emailArgs.Subject = "testkid";
        //    emailArgs.To.Add("stdsitsuwan2008@gmail.com", "testkid");
        //    emailArgs.SenderEmail = "stdsitsuwan2008@gmail.com";
        //    emailArgs.MailBody = GetHTMLFromView("~/Views/_Shared/MP/Front/_testsendemail.cshtml", user);
        //    emailArgs.Username = "psupapak15@gmail.com";
        //    emailArgs.Password = "Aa1234!@";
        //    emailArgs.SMTPServer = "smtp.gmail.com";
        //    emailArgs.Port = 587;
        //    EmailHelper.SendEmailWithAttachments(emailArgs);

        //    return View();

        //}


    }
}
