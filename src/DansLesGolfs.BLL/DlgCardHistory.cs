using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class DlgCardHistory
    {
        public int ItemId { get; set; }

        public int SaleId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CardNumber { get; set; }

        public float BeginBalance { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UserId { get; set; }

        public int SelectedCardStyleId { get; set; }

        public string Message { get; set; }

        public int DlgCardId { get; set; }
    }
}
