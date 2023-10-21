using MBKC.Repository.GrabFood.Models;
using MBKC.Service.GrabFoods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class ExcelUtil
    {
        public static DataTable GetNoneMappingGrabFoobItemData(List<NotMappingGrabFoodItem> notMappingGrabFoodItems)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "GrabFood Item";
            dataTable.Columns.Add("ItemID", typeof(string));
            dataTable.Columns.Add("ItemCode", typeof(string));
            dataTable.Columns.Add("ItemName", typeof(string));
            dataTable.Columns.Add("PriceInMin", typeof(decimal));
            dataTable.Columns.Add("Reason", typeof(string));

            notMappingGrabFoodItems.ForEach(item =>
            {
                dataTable.Rows.Add(item.GrabFoodItem.ItemID, item.GrabFoodItem.ItemCode, item.GrabFoodItem.ItemName, item.GrabFoodItem.PriceInMin, item.Reason);
            });
            return dataTable;
        }
    }
}
