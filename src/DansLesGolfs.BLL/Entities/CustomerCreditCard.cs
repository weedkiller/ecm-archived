//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DansLesGolfs.BLL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerCreditCard
    {
        public int CreditCardId { get; set; }
        public long UserId { get; set; }
        public Nullable<int> CardTypeId { get; set; }
        public string CardNumberE { get; set; }
        public string CardNumberX { get; set; }
        public string CardHolderName { get; set; }
        public string CardExpireX { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserId { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual User User { get; set; }
    }
}
