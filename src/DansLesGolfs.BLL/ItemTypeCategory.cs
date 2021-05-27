using DansLesGolfs.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.BLL
{
    public class ItemTypeCategory
    {
        #region Fields
        private int itemTypeId;
        private string itemTypeName;
        private int itemId;
        private string itemCode;
        private string itemDesc;
        public string ItemSlug { get; set; }
        public string ItemName { get; set; }

        #endregion


        #region Properties

        public int ItemTypeId
        {
            get { return itemTypeId; }
            set { itemTypeId = value; }
        }

        public string ItemTypeName
        {
            get { return itemTypeName; }
            set { itemTypeName = value; }
        }        

        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        public string ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }        

        public string ItemDesc
        {
            get { return itemDesc; }
            set { itemDesc = value; }
        }        

        #endregion

        #region Constructors
        public ItemTypeCategory()
        {
        }

        public ItemTypeCategory(DataRow row)
        {
            ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
            ItemTypeName = DataManager.ToString(row["ItemTypeName"]);
            
        }
        #endregion

        public string ItemTypeNameFr { get; set; }
    }
}
