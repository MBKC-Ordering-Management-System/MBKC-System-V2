using MBKC.Repository.Enums;
using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.GrabFoods;
using MBKC.Service.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Utils
{
    public static class GrabFoodUtil
    {
        public static PartnerProductsFromGrabFood GetPartnerProductsFromGrabFood(GrabFoodMenu grabFoodMenu, List<Category> storeCategories, int storeId, int partnerId, DateTime createdDate)
        {
            try
            {
                PartnerProductsFromGrabFood partnerProductsFromGrabFood = new PartnerProductsFromGrabFood();
                List<PartnerProduct> newPartnerProducts = new List<PartnerProduct>();
                List<PartnerProduct> oldPartnerProducts = new List<PartnerProduct>();
                List<NotMappingGrabFoodItem> notMappingGrabFoodItems = new List<NotMappingGrabFoodItem>();
                List<NotMappingGrabFoodModifierGroup> notMappingGrabFoodModifierGroups = new List<NotMappingGrabFoodModifierGroup>();
                Product existedProduct = null;
                List<GrabFoodModifier> CreatedMappingModifiers = new List<GrabFoodModifier>();
                List<GrabFoodModifierGroup> CreatedMappingModifierGroups = new List<GrabFoodModifierGroup>();

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
                                bool isMapped = false;
                                if (string.IsNullOrWhiteSpace(item.ItemCode))
                                {
                                    // compare name
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(item.ItemName.ToLower()) && x.Status != (int)ProductEnum.Status.DISABLE);
                                    if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is null)
                                    {
                                        newPartnerProducts.Add(new PartnerProduct()
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
                                        isMapped = true;
                                    }
                                    else if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is not null)
                                    {
                                        PartnerProduct existedPartnerProduct = existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId);
                                        existedPartnerProduct.ProductCode = item.ItemID;
                                        existedPartnerProduct.Status = item.AvailableStatus;
                                        existedPartnerProduct.Price = item.PriceInMin;
                                        existedPartnerProduct.UpdatedDate = DateTime.Now;
                                        
                                        oldPartnerProducts.Add(existedPartnerProduct);
                                        isMapped = true;
                                    }
                                }
                                else
                                {
                                    // compare code
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Code.ToLower().Equals(item.ItemCode.ToLower()) && x.Status != (int)ProductEnum.Status.DISABLE);
                                    if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is null)
                                    {
                                        newPartnerProducts.Add(new PartnerProduct()
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
                                        isMapped = true;
                                    }
                                    else if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is not null)
                                    {
                                        PartnerProduct existedPartnerProduct = existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId);
                                        existedPartnerProduct.ProductCode = item.ItemID;
                                        existedPartnerProduct.Status = item.AvailableStatus;
                                        existedPartnerProduct.Price = item.PriceInMin;
                                        existedPartnerProduct.UpdatedDate = DateTime.Now;

                                        oldPartnerProducts.Add(existedPartnerProduct);
                                        isMapped = true;
                                    }
                                }

                                if(isMapped == false)
                                {
                                    notMappingGrabFoodItems.Add(new NotMappingGrabFoodItem()
                                    {
                                        GrabFoodItem = item,
                                        Reason = MessageConstant.StorePartnerMessage.ItemOnGrabfoodCanNotMapping
                                    });
                                }
                            }
                        }

                        if (grabFoodItemsWithModifierGroup.Count > 0)
                        {
                            foreach (var item in grabFoodItemsWithModifierGroup)
                            {
                                bool isMapped = false;
                                if (string.IsNullOrEmpty(item.ItemCode))
                                {
                                    // compare name
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(item.ItemName.ToLower()) && x.Status != (int)ProductEnum.Status.DISABLE);
                                    if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is null)
                                    {
                                        if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                                        {
                                            newPartnerProducts.Add(new PartnerProduct()
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
                                                        if(CreatedMappingModifiers.Contains(modifier) == false)
                                                        {
                                                            CreatedMappingModifiers.Add(modifier);
                                                        }
                                                    }
                                                    if(CreatedMappingModifierGroups.Contains(grabFoodModifierGroup) == false)
                                                    {
                                                        CreatedMappingModifierGroups.Add(grabFoodModifierGroup);
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
                                                    
                                                    if (childItemFromGrabFood is not null && childProduct.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()) && childProduct.Status != (int)ProductEnum.Status.DISABLE)
                                                    {
                                                        /*string[] modifierNameParts = childItemFromGrabFood.ModifierName.Split(" ");
                                                        string productCode = item.ItemID + "-";
                                                        foreach (var modifierNamePart in modifierNameParts)
                                                        {
                                                            productCode += modifierNamePart;
                                                        }*/
                                                        newPartnerProducts.Add(new PartnerProduct()
                                                        {
                                                            PartnerId = partnerId,
                                                            StoreId = storeId,
                                                            CreatedDate = createdDate,
                                                            ProductCode = childItemFromGrabFood.ModifierID,
                                                            Status = item.AvailableStatus,
                                                            Price = item.PriceInMin + childItemFromGrabFood.PriceInMin,
                                                            ProductId = childProduct.ProductId,
                                                            MappedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now
                                                        });
                                                    }
                                                }
                                            }
                                            isMapped = true;
                                        }
                                        else if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()))
                                        {
                                            newPartnerProducts.Add(new PartnerProduct()
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
                                            isMapped = true;
                                        }
                                    } 
                                    else if(existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is not null)
                                    {
                                        PartnerProduct existedPartnerProduct = existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId);
                                        existedPartnerProduct.ProductCode = item.ItemID;
                                        existedPartnerProduct.Status = item.AvailableStatus;
                                        existedPartnerProduct.Price = item.PriceInMin;
                                        existedPartnerProduct.UpdatedDate = DateTime.Now;

                                        oldPartnerProducts.Add(existedPartnerProduct);
                                        isMapped = true;
                                    }
                                }
                                else
                                {
                                    // compare code
                                    existedProduct = existedCategory.Products.SingleOrDefault(x => x.Code.ToLower().Equals(item.ItemCode.ToLower()) && x.Status != (int)ProductEnum.Status.DISABLE);
                                    if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is null)
                                    {
                                        if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()))
                                        {
                                            newPartnerProducts.Add(new PartnerProduct()
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
                                                        if (CreatedMappingModifiers.Contains(modifier) == false)
                                                        {
                                                            CreatedMappingModifiers.Add(modifier);
                                                        }
                                                    }
                                                    if (CreatedMappingModifierGroups.Contains(grabFoodModifierGroup) == false)
                                                    {
                                                        CreatedMappingModifierGroups.Add(grabFoodModifierGroup);
                                                    }
                                                }
                                            }
                                            if (existedProduct.ChildrenProducts is not null && existedProduct.ChildrenProducts.Count() > 0)
                                            {
                                                foreach (var childProduct in existedProduct.ChildrenProducts)
                                                {
                                                    GrabFoodModifier childItemFromGrabFood = nameProductsFollowingRule.SingleOrDefault(x => x.Key.ToLower().Equals(childProduct.Name.ToLower())).Value;
                                                    if (childItemFromGrabFood is not null && childProduct.Type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()) && childProduct.Status != (int)ProductEnum.Status.DISABLE)
                                                    {
                                                        /*string[] modifierNameParts = childItemFromGrabFood.ModifierName.Split(" ");
                                                        string productCode = item.ItemID + "-";
                                                        foreach (var modifierNamePart in modifierNameParts)
                                                        {
                                                            productCode += modifierNamePart;
                                                        }*/
                                                        newPartnerProducts.Add(new PartnerProduct()
                                                        {
                                                            PartnerId = partnerId,
                                                            StoreId = storeId,
                                                            CreatedDate = createdDate,
                                                            ProductCode = childItemFromGrabFood.ModifierID,
                                                            Status = item.AvailableStatus,
                                                            Price = item.PriceInMin + childItemFromGrabFood.PriceInMin,
                                                            ProductId = childProduct.ProductId,
                                                            MappedDate = DateTime.Now,
                                                            UpdatedDate = DateTime.Now
                                                        });
                                                    }
                                                }
                                            }
                                            isMapped = true;
                                        }
                                        else if (existedProduct.Type.ToLower().Equals(ProductEnum.Type.SINGLE.ToString().ToLower()))
                                        {
                                            newPartnerProducts.Add(new PartnerProduct()
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
                                            isMapped = true;
                                        }
                                    }
                                    else if (existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is not null)
                                    {
                                        PartnerProduct existedPartnerProduct = existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId);
                                        existedPartnerProduct.ProductCode = item.ItemID;
                                        existedPartnerProduct.Status = item.AvailableStatus;
                                        existedPartnerProduct.Price = item.PriceInMin;
                                        existedPartnerProduct.UpdatedDate = DateTime.Now;

                                        oldPartnerProducts.Add(existedPartnerProduct);
                                        isMapped = true;
                                    }
                                }

                                if (isMapped == false)
                                {
                                    notMappingGrabFoodItems.Add(new NotMappingGrabFoodItem()
                                    {
                                        GrabFoodItem = item,
                                        Reason = MessageConstant.StorePartnerMessage.ItemOnGrabfoodCanNotMapping
                                    });
                                }
                            }
                        }
                    }
                    else if(existedCategory is null)
                    {
                        foreach (var grabFoodItem in grabFoodCategory.Items)
                        {
                            notMappingGrabFoodItems.Add(new NotMappingGrabFoodItem()
                            {
                                GrabFoodItem = grabFoodItem,
                                Reason = MessageConstant.StorePartnerMessage.ItemOnGrabfoodCanNotMapping
                            });
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
                            existedProduct = existedCategory.Products.SingleOrDefault(x => x.Name.ToLower().Equals(grabFoodModifier.ModifierName.ToLower()) && x.Status != (int)ProductEnum.Status.DISABLE);
                            if (CreatedMappingModifiers.Contains(grabFoodModifier) == false && existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is null)
                            {
                                /*string[] modifierNameParts = grabFoodModifier.ModifierName.Split(" ");
                                string productCode = "";
                                foreach (var modifierNamePart in modifierNameParts)
                                {
                                    productCode += modifierNamePart[0].ToString().ToUpper();
                                }*/
                                newPartnerProducts.Add(new PartnerProduct()
                                {
                                    PartnerId = partnerId,
                                    StoreId = storeId,
                                    CreatedDate = createdDate,
                                    ProductCode = grabFoodModifier.ModifierID,
                                    Status = grabFoodModifier.AvailableStatus,
                                    Price = grabFoodModifier.PriceInMin,
                                    ProductId = existedProduct.ProductId,
                                    MappedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now
                                });
                                if (CreatedMappingModifiers.Contains(grabFoodModifier) == false)
                                {
                                    CreatedMappingModifiers.Add(grabFoodModifier);
                                }
                            }
                            else if (CreatedMappingModifiers.Contains(grabFoodModifier) == false &&  existedProduct is not null && existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId) is not null)
                            {
                                PartnerProduct existedPartnerProduct = existedProduct.PartnerProducts.SingleOrDefault(x => x.ProductId == existedProduct.ProductId);
                                existedPartnerProduct.ProductCode = grabFoodModifier.ModifierID;
                                existedPartnerProduct.Status = grabFoodModifier.AvailableStatus;
                                existedPartnerProduct.Price = grabFoodModifier.PriceInMin;
                                existedPartnerProduct.UpdatedDate = DateTime.Now;

                                oldPartnerProducts.Add(existedPartnerProduct);
                            }
                        }
                    }
                    if(CreatedMappingModifierGroups.Contains(grabFoodModifierGroup) == false)
                    {
                        notMappingGrabFoodModifierGroups.Add(new NotMappingGrabFoodModifierGroup()
                        {
                            GrabFoodModifierGroup = grabFoodModifierGroup,
                            Reason = MessageConstant.StorePartnerMessage.ModifierGroupOnGrabfoodCanNotMapping
                        });
                    }
                }
                partnerProductsFromGrabFood.NewPartnerProducts = newPartnerProducts;
                partnerProductsFromGrabFood.OldPartnerProducts = oldPartnerProducts;
                partnerProductsFromGrabFood.NotMappingFromGrabFood = new NotMappingFromGrabFood()
                {
                    NotMappingGrabFoodItems = notMappingGrabFoodItems,
                    NotMappingGrabFoodModifierGroups = notMappingGrabFoodModifierGroups
                };
                return partnerProductsFromGrabFood;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void CheckProductCodeFromGrabFood(GrabFoodMenu grabFoodMenu, string productCode, string type, decimal price, int status, bool isUpdated, string? productCodeParentProduct, string productName)
        {
            try
            {
                bool isExisted = false;
                GrabFoodItem parentGrabFoodItem = null;
                if(productCodeParentProduct is not null)
                {
                    foreach (var category in grabFoodMenu.Categories)
                    {
                        parentGrabFoodItem = category.Items.FirstOrDefault(x => x.ItemID.Trim().ToLower().Equals(productCodeParentProduct.Trim().ToLower()));
                        if(parentGrabFoodItem is not null)
                        {
                            break;
                        }
                    }
                }
                foreach (var category in grabFoodMenu.Categories)
                {
                    GrabFoodItem grabFoodItem = category.Items.FirstOrDefault(x => x.ItemID.Trim().ToLower().Equals(productCode.Trim().ToLower()));
                    if (grabFoodItem is null)
                    {
                        if (grabFoodMenu.ModifierGroups is not null && grabFoodMenu.ModifierGroups.Count() > 0)
                        {
                            foreach (var modifierGroup in grabFoodMenu.ModifierGroups)
                            {
                                GrabFoodModifier grabFoodModifier = modifierGroup.Modifiers.FirstOrDefault(x => x.ModifierID.Trim().ToLower().Equals(productCode.Trim().ToLower()));
                                if (grabFoodModifier is not null)
                                {
                                    isExisted = true;
                                    string nameOfRule = grabFoodModifier.ModifierName;
                                    if (type.ToLower().Equals(ProductEnum.Type.CHILD.ToString().ToLower()))
                                    {
                                        nameOfRule = $"{parentGrabFoodItem.ItemName} - {grabFoodModifier.ModifierName}";
                                    }

                                    if(nameOfRule.ToLower().Equals(productName.ToLower()) == false)
                                    {
                                        throw new BadRequestException(MessageConstant.PartnerProductMessage.GrabFoodProductWithProductCodeNotMatchWithProductSystem);
                                    }
                                    if (grabFoodModifier.PriceInMin != (price - parentGrabFoodItem.PriceInMin))
                                    {
                                        throw new BadRequestException(MessageConstant.PartnerProductMessage.PriceNotMatchWithProductInGrabFoodSystem);
                                    }
                                    if (grabFoodModifier.AvailableStatus != status && isUpdated == false)
                                    {
                                        throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusNotMatchWithProductInGrabFoodSystem);
                                    }
                                    break;
                                }
                            }
                        }
                    } else
                    {
                        isExisted = true;
                        string nameOfRule = grabFoodItem.ItemName;
                        if (productName.ToLower().Equals(nameOfRule.ToLower()) == false)
                        {
                            throw new BadRequestException(MessageConstant.PartnerProductMessage.GrabFoodProductWithProductCodeNotMatchWithProductSystem);
                        }
                        if (type.ToLower().Equals(ProductEnum.Type.PARENT.ToString().ToLower()) == false)
                        {
                            if (grabFoodItem.PriceInMin != price)
                            {
                                throw new BadRequestException(MessageConstant.PartnerProductMessage.PriceNotMatchWithProductInGrabFoodSystem);
                            }
                        }
                        if (grabFoodItem.AvailableStatus != status && isUpdated == false)
                        {
                            throw new BadRequestException(MessageConstant.PartnerProductMessage.StatusNotMatchWithProductInGrabFoodSystem);
                        }
                    }
                    if (isExisted)
                    {
                        break;
                    }
                }
                if(isExisted == false)
                {
                    throw new BadRequestException(MessageConstant.PartnerProductMessage.ProductCodeNotExistInGrabFoodSystem);
                }
            } catch(BadRequestException ex)
            {
                throw new BadRequestException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
