using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DansLesGolfs.BLL
{
    public class AddDlgCard
    {
        [Required(ErrorMessage = "*")]
        public string CardName { get; set; }

        public string Amount { get; set; }

        public int DlgCardId { get; set; }

        public int UserId { get; set; }

        public int? ItemId { get;  set; }

        [Required(ErrorMessage = "*")]
        public int DlgPrice { get; set; }
    }
}
