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
        public static List<PartnerProduct> GetPartnerProductsFromGrabFood(GrabFoodMenu grabFoodMenu, List<Category> storeCategories, int storeId, int partnerId, DateTime createdDate)
        {
            try
            {
                List<PartnerProduct> partnerProducts = new List<PartnerProduct>();
                Product existedProduct = null;
                List<GrabFoodModifier> CreatedMappingProduct = new List<GrabFoodModifier>();

                foreach (var grabFoodCategory in grabFoodMenu.Categories)
                {
                    Category existedCategory = storeCategories.SingleOrDefault(x => x.Name.ToLower().Equals(grabFoodCategory.CategoryName.ToLower()));
                    if (existedCategory is not null && existedCategory.Type.ToLower().Equals(CategoryEnum.Type.NORMAL.ToString().ToLower()))
                    {
                        List<GrabFoodItem> grabFoodItemsWithModifierGroup = grabFoodCategory.Items.Where(x => x.LinkedModifierGroupIDs != null).ToList();
                        List<GrabFoodItem> grabFoodItemsWithoutModifierGroup = grabFoodCategory.Items.Where(x => x.LinkedModifierGroupIDs == null).ToList();

                        if (grabFoodItemsWithoutModifierGroup.Count > 0)
                        {
                            foreach (var item in grabFoodItemsWithoutModifierGroup)
                            {
                                if (string.IsNullOrWhiteSpace(item.ItemCode))
                                {
                                    // compare name
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(item.ItemName.ToLower()) && x.SellingPrice == item.PriceInMin);
                                    if (existedProduct is not null)
                                    {
                                        partnerProducts.Add(new PartnerProduct()
                                        {
                                            PartnerId = partnerId,
                                            StoreId = storeId,
                                            CreatedDate = createdDate,
                                            ProductCode = item.ItemID,
                                            Status = item.AvailableStatus,
                                            Price = item.PriceInMin,
                                            ProductId = existedProduct.ProductId,
                                            MappedDate = DateTime.Now,
                                            UpdatedDate = DateTime.Now
                                        });
                                    }
                                }
                                else
                                {
                                    // compare code
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Code.ToLower().Equals(item.ItemCode.ToLower()));
                                    if (existedProduct is not null)
                                    {
                                        partnerProducts.Add(new PartnerProduct()
                                        {
                                            PartnerId = partnerId,
                                            StoreId = storeId,
                                            CreatedDate = createdDate,
                                            ProductCode = item.ItemID,
                                            Status = item.AvailableStatus,
                                            Price = item.PriceInMin,
                                            ProductId = existedProduct.ProductId,
                                            MappedDate = DateTime.Now,
                                            UpdatedDate = DateTime.Now
                                        });
                                    }
                                }
                            }
                        }

                        if (grabFoodItemsWithModifierGroup.Count > 0)
                        {
                            foreach (var item in grabFoodItemsWithModifierGroup)
                            {
                                if (string.IsNullOrEmpty(item.ItemCode))
                                {
                                    // compare name
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(item.ItemName.ToLower()));
                                    if (existedProduct is not null)
                                    {
                                        if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                                        {
                                            partnerProducts.Add(new PartnerProduct()
                                            {
                                                PartnerId = partnerId,
                                                StoreId = storeId,
                                                CreatedDate = createdDate,
                                                ProductCode = item.ItemID,
                                                Status = item.AvailableStatus,
                                                Price = 0,
                                                ProductId = existedProduct.ProductId,
                                                MappedDate = DateTime.Now,
                                                UpdatedDate = DateTime.Now
                                            });
                                            Dictionary<string, GrabFoodModifier> nameProductsFollowingRule = new Dictionary<string, GrabFoodModifier>(StringComparer.InvariantCultureIgnoreCase);
                                            foreach (var linkedModifierGroupId in item.LinkedModifierGroupIDs)
                                            {
                                                GrabFoodModifierGroup grabFoodModifierGroup = grabFoodMenu.ModifierGroups.SingleOrDefault(x => x.ModifierGroupID.ToString().ToLower().Equals(linkedModifierGroupId.ToLower()));
                                                if (grabFoodModifierGroup is not null)
                                                {
                                                    foreach (var modifier in grabFoodModifierGroup.Modifiers)
                                                    {
                                                        string nameProductWithFollowingRule = $"{item.ItemName} - {modifier.ModifierName}";
                                                        nameProductsFollowingRule.Add(nameProductWithFollowingRule, modifier);
                                                        if(CreatedMappingProduct.Contains(modifier) == false)
                                                        {
                                                            CreatedMappingProduct.Add(modifier);
                                                        }
                                                    }
                                                }
                                            }
                                            if (existedProduct.ChildrenProducts is not null && existedProduct.ChildrenProducts.Count() > 0)
                                            {
                                                foreach (var childProduct in existedProduct.ChildrenProducts)
                                                {
                                                    GrabFoodModifier childItemFromGrabFood = null;
                                                    if (nameProductsFollowingRule.ContainsKey(childProduct.Name))
                                                    {
                                                        childItemFromGrabFood = nameProductsFollowingRule[childProduct.Name];
                                                    }
                                                    
                                                    if (childItemFromGrabFood is not null && childProduct.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                                                    {
                                                        string[] modifierNameParts = childItemFromGrabFood.ModifierName.Split(" ");
                                                        string productCode = item.ItemID + "-";
                                                        foreach (var modifierNamePart in modifierNameParts)
                                                        {
                                                            productCode += modifierNamePart;
                                                        }
                                                        partnerProducts.Add(new PartnerProduct()
                                                        {
                                                            PartnerId = partnerId,
                                                            StoreId = storeId,
                                                            CreatedDate = createdDate,
                                                            ProductCode = productCode,
                                                            Status = item.AvailableStatus,
                                                            Price = item.PriceInMin + childItemFromGrabFood.PriceInMin,
                                                            ProductId = childProduct.ProductId,
                                                            MappedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        else if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()))
                                        {
                                            partnerProducts.Add(new PartnerProduct()
                                            {
                                                PartnerId = partnerId,
                                                StoreId = storeId,
                                                CreatedDate = createdDate,
                                                ProductCode = item.ItemID,
                                                Status = item.AvailableStatus,
                                                Price = item.PriceInMin,
                                                ProductId = existedProduct.ProductId,
                                                MappedDate = DateTime.Now,
                                                UpdatedDate = DateTime.Now
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    // compare code
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Code.ToLower().Equals(item.ItemCode.ToLower()));
                                    if (existedProduct is not null)
                                    {
                                        if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                                        {
                                            partnerProducts.Add(new PartnerProduct()
                                            {
                                                PartnerId = partnerId,
                                                StoreId = storeId,
                                                CreatedDate = createdDate,
                                                ProductCode = item.ItemID,
                                                Status = item.AvailableStatus,
                                                Price = 0,
                                                ProductId = existedProduct.ProductId,
                                                MappedDate = DateTime.Now,
                                                UpdatedDate = DateTime.Now
                                            });
                                            Dictionary<string, GrabFoodModifier> nameProductsFollowingRule = new Dictionary<string, GrabFoodModifier>();
                                            foreach (var linkedModifierGroupId in item.LinkedModifierGroupIDs)
                                            {
                                                GrabFoodModifierGroup grabFoodModifierGroup = grabFoodMenu.ModifierGroups.SingleOrDefault(x => x.ToString().ToLower().Equals(linkedModifierGroupId));
                                                if (grabFoodModifierGroup is not null)
                                                {
                                                    foreach (var modifier in grabFoodModifierGroup.Modifiers)
                                                    {
                                                        string nameProductWithFollowingRule = $"{item.ItemName} - {modifier.ModifierName}";
                                                        nameProductsFollowingRule.Add(nameProductWithFollowingRule, modifier);
                                                        if (CreatedMappingProduct.Contains(modifier) == false)
                                                        {
                                                            CreatedMappingProduct.Add(modifier);
                                                        }
                                                    }
                                                }
                                            }
                                            if (existedProduct.ChildrenProducts is not null && existedProduct.ChildrenProducts.Count() > 0)
                                            {
                                                foreach (var childProduct in existedProduct.ChildrenProducts)
                                                {
                                                    GrabFoodModifier childItemFromGrabFood = nameProductsFollowingRule.SingleOrDefault(x => x.Key.ToLower().Equals(childProduct.Name.ToLower())).Value;
                                                    if (childItemFromGrabFood is not null && childProduct.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                                                    {
                                                        string[] modifierNameParts = childItemFromGrabFood.ModifierName.Split(" ");
                                                        string productCode = item.ItemID + "-";
                                                        foreach (var modifierNamePart in modifierNameParts)
                                                        {
                                                            productCode += modifierNamePart;
                                                        }
                                                        partnerProducts.Add(new PartnerProduct()
                                                        {
                                                            PartnerId = partnerId,
                                                            StoreId = storeId,
                                                            CreatedDate = createdDate,
                                                            ProductCode = productCode,
                                                            Status = item.AvailableStatus,
                                                            Price = item.PriceInMin + childItemFromGrabFood.PriceInMin,
                                                            ProductId = childProduct.ProductId,
                                                            MappedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        else if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()))
                                        {
                                            partnerProducts.Add(new PartnerProduct()
                                            {
                                                PartnerId = partnerId,
                                                StoreId = storeId,
                                                CreatedDate = createdDate,
                                                ProductCode = item.ItemID,
                                                Status = item.AvailableStatus,
                                                Price = item.PriceInMin,
                                                ProductId = existedProduct.ProductId,
                                                MappedDate = DateTime.Now,
                                                UpdatedDate = DateTime.Now
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var grabFoodModifierGroup in grabFoodMenu.ModifierGroups)
                {
                    Category existedCategory = storeCategories.SingleOrDefault(x => x.Name.ToLower().Equals(grabFoodModifierGroup.ModifierGroupName.ToLower()));
                    if(existedCategory is not null && existedCategory.Type.ToLower().Equals(CategoryEnum.Type.EXTRA.ToString().ToLower()))
                    {
                        foreach (var grabFoodModifier in grabFoodModifierGroup.Modifiers)
                        {
                            existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(grabFoodModifier.ModifierName.ToLower()));
                            if (CreatedMappingProduct.Contains(grabFoodModifier) == false && existedProduct is not null)
                            {
                                string[] modifierNameParts = grabFoodModifier.ModifierName.Split(" ");
                                string productCode = "";
                                foreach (var modifierNamePart in modifierNameParts)
                                {
                                    productCode += modifierNamePart[0].ToString().ToUpper();
                                }
                                partnerProducts.Add(new PartnerProduct()
                                {
                                    PartnerId = partnerId,
                                    StoreId = storeId,
                                    CreatedDate = createdDate,
                                    ProductCode = productCode,
                                    Status = grabFoodModifier.AvailableStatus,
                                    Price = grabFoodModifier.PriceInMin,
                                    ProductId = existedProduct.ProductId,
                                    MappedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now
                                });
                                if (CreatedMappingProduct.Contains(grabFoodModifier) == false)
                                {
                                    CreatedMappingProduct.Add(grabFoodModifier);
                                }
                            }
                        }
                    }
                }
                return partnerProducts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
