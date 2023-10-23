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
            dataTable.TableName = "GrabFood Items";
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

        public static DataTable GetNoneMappingGrabFoodModifierGroupData(List<NotMappingGrabFoodModifierGroup> notMappingGrabFoodModifierGroups)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = "GrabFood Modifier Groups";
            dataTable.Columns.Add("ModifierGroupID", typeof(string));
            dataTable.Columns.Add("ModifierGroupName", typeof(string));
            dataTable.Columns.Add("ModifierID", typeof(string));
            dataTable.Columns.Add("ModifierName", typeof(string));
            dataTable.Columns.Add("PriceInMin", typeof(decimal));
            dataTable.Columns.Add("Reason", typeof(string));

            notMappingGrabFoodModifierGroups.ForEach(group =>
            {
                foreach (var modifier in group.GrabFoodModifierGroup.Modifiers)
                {
                    dataTable.Rows.Add(group.GrabFoodModifierGroup.ModifierGroupID, group.GrabFoodModifierGroup.ModifierGroupName, modifier.ModifierID, modifier.ModifierName, modifier.PriceInMin, group.Reason);
                }
            });
            return dataTable;
        }
    }
}
