using MBKC.Repository.Enums;
using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class GrabFoodUtil
    {
        /*public List<PartnerProduct> GetPartnerProductsFromGrabFood(GrabFoodMenu grabFoodMenu, List<Category> storeCategories, int storeId, int partnerId, DateTime createdDate)
        {
            try
            {
                List<PartnerProduct> partnerProducts = new List<PartnerProduct>();

                foreach (var category in storeCategories)
                {
                    GrabFoodCategory grabFoodCategory = grabFoodMenu.Categories.SingleOrDefault(x => x.CategoryID.ToLower().Equals(category.Code.ToLower()));
                    if(grabFoodCategory is not null)
                    {
                        List<GrabFoodItem> grabFoodItemsWithModifierGroup = grabFoodCategory.Items.Where(x => x.LinkedModifierGroupIDs != null).ToList();
                        List<GrabFoodItem> grabFoodItemsWithoutModifierGroup = grabFoodCategory.Items.Where(x => x.LinkedModifierGroupIDs == null).ToList();
                        if(grabFoodItemsWithoutModifierGroup.Count > 0)
                        {
                            foreach (var item in grabFoodItemsWithoutModifierGroup)
                            {
                                if (string.IsNullOrWhiteSpace(item.ItemCode))
                                {
                                    // compare name
                                } else
                                {
                                    // compare code
                                }
                            }
                        }
                    }
                }

                #region Delete
                *//*foreach (var grabFoodCategory in grabFoodMenu.Categories)
                {
                    foreach (var grabFoodItem in grabFoodCategory.Items)
                    {
                        Product existedProductInSystem = storeCategories.Products.SingleOrDefault(x => x.Code.ToLower().Equals(grabFoodItem.ItemCode.ToLower()));
                        if (existedProductInSystem is not null)
                        {
                            if (existedProductInSystem.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                            {

                            }
                            partnerProducts.Add(new PartnerProduct()
                            {
                                PartnerId = partnerId,
                                StoreId = storeId,
                                CreatedDate = createdDate,
                                ProductCode = grabFoodItem.ItemCode,
                                Status = grabFoodItem.AvailableStatus,
                                Price = grabFoodItem.PriceInMin,
                                ProductId = existedProductInSystem.ProductId,
                                MappedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        else
                        {
                            Dictionary<string, GrabFoodModifier> nameProductsFollowingRule = new Dictionary<string, GrabFoodModifier>();
                            foreach (var modifierGroup in grabFoodItem.LinkedModifierGroupIDs)
                            {
                                GrabFoodModifierGroup grabFoodModifierGroup = grabFoodMenu.ModifierGroups.SingleOrDefault(x => x.ModifierGroupID.Equals(modifierGroup));
                                if(grabFoodModifierGroup is not null)
                                {
                                    foreach (var modifier in grabFoodModifierGroup.Modifiers)
                                    {
                                        string nameProductFollowingRule = $"{grabFoodItem.ItemName} {modifier.ModifierName}";
                                        nameProductsFollowingRule.Add(nameProductFollowingRule, modifier);
                                    }
                                }
                            }

                            foreach (var nameProduct in nameProductsFollowingRule)
                            {
                                Product existedProductInSystemWithCOmpareName = storeCategories.Products.SingleOrDefault(x => x.Name.ToLower().Equals(nameProduct.Key.ToLower()));
                                if (existedProductInSystemWithCOmpareName is not null)
                                {

                                }
                            }
                        }
                    }
                }*//*
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/
    }
}
