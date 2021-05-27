using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace DansLesGolfs.BLL
{
    public class PromotionModel
    {
        public int? PromotionId { get; set; }

        
        public string PromotionImage { get; set; }

        public string PromotionBrandImage { get; set; }

        [Required()]
        public string PromotionContent { get; set; }

        [Required()]
        public string PromotionTimecontent { get; set; }

        //[Required()]
        public string BrandName { get; set; }

        public int ToltalItemCount { get; set; }

        public List<SelectListItem> BrandsList { get; set; }

        public int BrandNameId { get; set; }
    }
}
