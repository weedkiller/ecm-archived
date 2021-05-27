using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class DLGCardBalanceAdmin
    {
        
        public int Id { get; set; }
        public int DLGCardId { get; set; }
        public int UserId { get; set; }
        public int ActionType { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public DateTime InsertDate { get; set; }
        public int SaleId { get; set; }
        public bool Active { get; set; }

        public string Description { get; set; }
    }
}
