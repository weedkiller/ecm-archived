using System;
using System.Collections.Generic;
using System.Text;

namespace IG.Engine.EmailService.DataAccess
{

    public enum UserType
    { 
        Employee = 1,
        Partner =2,
        Supplier=3,
        Customer=4 //customer warranty
    }

    public enum CustomerGrade
    {
        VK1 = 1,
        VK2 = 2,
        VK3 = 3,
        VK4 = 4,
        VK5=5
    }

    public enum ItemTypeEnum
    {
        Unknow = 0,
        Items = 1,
        BorrowReturn = 2,
        ExchangeStock =3,
        Service=4
    }

    public enum POStatus
    {
        Waiting_For_Approve = 1,
        Approved = 2,
        Back_Order = 3,
        Completed = 4,
        Closed = 5
    }

    public enum BorrowStatus
    {
        Open=0,
        Close=1
    }

    public enum TransferStatus
    {
        Waiting = 0,
        Completed = 1,
        Cancel=2
    }

    public enum CheckoutStatus
    {
        Openning,
        Completed,
        Cancel
    }

    public enum ModulePermission
    {
        ExchangeManagement=37,
        ServiceManagement=41,
        InventoryManagement=42,
        BorrowReturnManagement=48
    }

    public enum Impressum
    {
        Nutzinger = 1,
        Zucker =2 
    }

    public class GlobalData
    {
        public const int MASTER_ADMIN_ROLE_ID = 0;

        //----------------------------Email----------------------------
        static public string DisplayName = "";
        //static public string Host = "smtp.1und1.de";
        //static public string UserName = "test@inconn.de";
        //static public string Password = "hham1551 ";
        static public string Host = "smtp.gmail.com";
        static public string UserName = "j.IG.Engine.EmailService@gmail.com";
        static public string Password = "goodday999";
        static public int Port = 587;
        //--------------------------------------------------------------
    }

    
}
