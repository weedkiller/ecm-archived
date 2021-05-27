using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansLesGolfs.Base;
using System.Data;

namespace DansLesGolfs.BLL
{
    public class DLGCardObj
    {

	#region Fields  
	public int DLGCardId;
	public int ItemId;
	public int SaleId;
	public string FirstName;
	public string LastName;
	public string Email;
	public string CardNumber;
	public string Message;
	public decimal BeginBalance;
	public DateTime InsertDate;
	public DateTime UpdateDate;
	public int UserId;
	public bool Active;
	#endregion 

	#region Properties 
	#endregion 

	#region Constructors 
	public DLGCardObj()
	{
	}

    public DLGCardObj(DataRow dr)
    {
        DLGCardId = DataManager.ToInt(dr["DLGCardId"]);
        ItemId = DataManager.ToInt(dr["ItemId"]);
        SaleId = DataManager.ToInt(dr["SaleId"]);
        FirstName = DataManager.ToString(dr["FirstName"]);
        LastName = DataManager.ToString(dr["LastName"]);
        Email = DataManager.ToString(dr["Email"]);
        CardNumber = DataManager.ToString(dr["CardNumber"]);
        Message = DataManager.ToString(dr["Message"]);
        BeginBalance = Convert.ToDecimal(dr["BeginBalance"]);
        UserId = DataManager.ToInt(dr["UserId"]);
        Active = DataManager.ToBoolean(dr["Active"]);
    }

	#endregion 

    }
}
