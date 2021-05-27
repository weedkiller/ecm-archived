using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DansLesGolfs.BLL;

namespace DansLesGolfs.Models
{
    public class UserMessageViewModel
    {
        public List<UserMessageModel> UserMessageModelList { get; set; }

        public int PageIndex { get; set; }

        public string SearchText { get; set; }
    }
}