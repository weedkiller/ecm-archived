using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;
using DansLesGolfs.BLL;

namespace DansLesGolfs.Models
{
    public class ProductsListModel
    {
        private List<Item> items;
        public List<Item> Items
        {
            get { return items; }
            set { items = value; }
        }

        private int page = 1;
        public int Page
        {
            get { return page; }
            set { page = value < 1 ? 1 : value; }
        }

        private int totalPages = 1;
        public int TotalPages
        {
            get { return totalPages; }
            set { totalPages = value < 1 ? 1 : value; }
        }

        private bool isShowStar = true;
        public bool IsShowStar
        {
            get { return isShowStar; }
            set { isShowStar = value; }
        }

        private bool isShowNoItemText = true;
        public bool IsShowNoItemText
        {
            get { return isShowNoItemText; }
            set { isShowNoItemText = value; }
        }

        private bool isShowWrapper = true;
        public bool IsShowWrapper
        {
            get { return isShowWrapper; }
            set { isShowWrapper = value; }
        }
    }
}