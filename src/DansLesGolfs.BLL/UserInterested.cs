using DansLesGolfs.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class UserInterested
    {
        #region Fields

        private int userInterestedId;
        private int userId;
        private int itemTypeId;

        #endregion

        #region Properties

        public int UserInterestedId
        {
            get { return userInterestedId; }
            set { userInterestedId = value; }

        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public int ItemTypeId
        {
            get { return itemTypeId; }
            set { itemTypeId = value; }
        }

        #endregion

        #region Constructors

        public UserInterested()
        {

        }

        public UserInterested(DataRow row)
        {
            
            UserId = DataManager.ToInt(row["UserId"]);
            //ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
            UserInterestedId = DataManager.ToInt(row["InterestId"]);            
        }

        #endregion
    }
}
