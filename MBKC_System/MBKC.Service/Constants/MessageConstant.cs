﻿namespace MBKC.Service.Constants
{
    public static class MessageConstant
    {

        public static class CommonMessage
        {

            public const string NotExistEmail = "Email does not exist in the system.";
            public const string AlreadyExistEmail = "Email already exists in the system.";
            public const string AlreadyExistCitizenNumber = "Citizen number already exists in the system.";
            public const string InvalidKitchenCenterId = "Kitchen center id is not suitable id in the system.";
            public const string InvalidBrandId = "Brand id is not suitable id in the system.";
            public const string InvalidStoreId = "Store id is not suitable id in the system.";
            public const string InvalidCategoryId = "Category id is not suitable id in the system.";
            public const string InvalidBankingAccountId = "Banking account id is not suitable id in the system.";
            public const string InvalidCashierId = "Cashier id is not suitable id in the system.";
            public const string InvalidProductId = "Product id is not suitable id in the system.";
            public const string NotExistKitchenCenterId = "Kitchen center id does not exist in the system.";
            public const string NotExistKitchenCenter = "Kitchen center does not exist in the system.";
            public const string NotExistBrandId = "Brand id does not exist in the system.";
            public const string NotExistStoreId = "Store id does not exist in the system.";
            public const string NotExistCategoryId = "Category id does not exist in the system.";
            public const string NotExistBankingAccountId = "Banking account id does not exist in the system.";
            public const string NotExistProductId = "Product id does not exist in the system.";
            public const string NotExistCashierId = "Cashier id does not exist in the system.";
            public const string InvalidItemsPerPage = "Items per page number is required more than 0.";
            public const string InvalidCurrentPage = "Current page number is required more than 0.";
            public const string NotExistPartnerId = "Partner id does not exist in the system.";
            public const string InvalidPartnerId = "Partner id is not suitable id in the system.";
            public const string CategoryIdNotBelongToBrand = "Category id does not belong to your brand.";
        }

        public static class LoginMessage
        {
            public const string DisabledAccount = "Account has been disabled.";
            public const string InvalidEmailOrPassword = "Email or Password is invalid.";
        }

        public static class VerificationMessage
        {
            public const string NotAuthenticatedEmailBefore = "Email has not been previously authenticated.";
            public const string ExpiredOTPCode = "OTP code has expired.";
            public const string NotMatchOTPCode = "Your OTP code does not match with the previously sent OTP code.";
        }

        public static class ReGenerationMessage
        {
            public const string InvalidAccessToken = "Access token is invalid.";
            public const string NotExpiredAccessToken = "Access token has not yet expired.";
            public const string NotExistAuthenticationToken = "You do not has the authentication tokens in the system.";
            public const string NotExistRefreshToken = "Refresh token does not exist.";
            public const string NotMatchAccessToken = "Your access token does not match the registered access token before.";
            public const string ExpiredRefreshToken = "Your refresh token expired now.";
        }

        public static class ChangePasswordMessage
        {
            public const string NotAuthenticatedEmail = "Email has not been previously authenticated.";
            public const string NotVerifiedEmail = "Email is not yet authenticated with the previously sent OTP code.";
        }

        public static class KitchenCenterMessage
        {
            public const string DeactiveKitchenCenter_Update = "Kitchen center was deleted before, so this kitchen center cannot update.";
            public const string DeactiveKitchenCenter_Delete = "Kitchen center cannot delete because that was deleted before.";
            public const string ManagerEmailExisted = "Kitchen center manager email already existed in the system.";
            public const string ExistedActiveStores_Delete = "The kitchen center has active stores, so this kitchen center cannot be deleted.";
            public const string NotBelongToKitchenCenter = "Kitchen center id does not belong to your kitchen center.";
        }

        public static class BrandMessage
        {
            public const string InvalidStatusFilter = "Key status filter is required ACTIVE, INACTIVE in the system.";
            public const string NotBelongToBrand = "Brand id does not belong to your brand.";
            public const string DeactiveBrand_Delete = "Brand cannot delete because that was deleted before.";
            public const string DeactiveBrand_Update = "Brand was deleted before, so this brand cannot update.";
            public const string ManagerEmailExisted = "Brand manager email already existed in the system.";
            public const string RoleNotSuitable = "Role is not suitable";
        }

        public static class StoreMessage
        {
            public const string BrandNotJoinKitchenCenter = "Brand does not join into the kitchen center.";
            public const string KitchenCenterNotHaveBrand = "Kitchen center does not have this brand.";
            public const string BrandNotHaveStore = "Brand does not have this store in the system.";
            public const string KitchenCenterNotHaveStore = "Kitchen center does not have this store in the system.";
            public const string DeactiveStore_Update = "Store was deleted before, so this store cannot update.";
            public const string DeactiveStore_Delete = "Store cannot delete because that was deleted before.";
            public const string ManageremailExisted = "Store manager email already existed in the system.";
            public const string NotConfirmingStore = "Store is not a new store to confirm to become an ACTIVE store.";
            public const string NotRejectedResonForNewStore = "Rejected store registration is required a reason.";
            public const string StoreIdNotBelongToStore = "Store id does not belong to your store.";
        }

        public static class CategoryMessage
        {
            
            public const string CategoryCodeExisted = "Category code already exist in the system.";
            public const string DeactiveCategory_Delete = "Category cannot delete because that was deleted before.";
            public const string DeactiveCategory_Update = "Category was deleted before, so this category cannot update.";
            public const string InvalidCategoryType = "Type is required.";
            public const string NotExistCategoryType = "Type is required NORMAL or EXTRA.";
            public const string StatusInvalid = "Status is ACTIVE or INACTIVE.";
            public const string CategoryMustBeNormal = "CategoryId must be a NORMAL type.";
            public const string ExtraCategoryGreaterThan0 = "Extra category Id must be greater than 0.";
            public const string ListExtraCategoryIdIsExtraType = "List extra category Id need to be a EXTRA type.";
            public const string ListExtraCategoryIdIsActive = "List extra category Id need status is ACTIVE.";
            public const string ExtraCategoryIdNotBelongToBrand = "Extra category Id does not belong to brand.";
            public const string ExtraCategoryIdDoesNotExist = "Extra category Id does not exist in the system.";
        }

        public static class BankingAccountMessage
        {
            public const string BankingAccountNotBelongToKitchenCenter = "Your kitchen center does not have this banking account id.";
            public const string NumberAccountExisted = "Number account already existed in the system.";
        }

        public static class ProductMessage
        {
            public const string ProductCodeExisted = "Code already exist in the system.";
            public const string ParentProductIdNotExist = "Parent product id does not exist in the system.";
            public const string ParentProductIdNotBelongToBrand = "Parent product id does not belong to your brand.";
            public const string CategoryNotSuitableForSingleOrParentProductType = "Category id is not suitable type for SINGLE or PARENT product type.";
            public const string CategoryNotSuitableForEXTRAProductType = "Category id is not suitable type for EXTRA product type.";
            public const string CategoryIdNotBelongToStore = "Category id does not belong to your store.";
            public const string CategoryIdNotBelongToKitchenCenter = "Your kitchen center cannot get products with this category id.";
            public const string InvalidProductType = "Product type is required some types such as: SINGLE, PARENT, CHILD, EXTRA.";
            public const string ProductNotBelongToBrand = "Product id does not belong to your brand.";
            public const string ProductNotBelongToStore = "Product id does not belong to your store.";
            public const string ProductNotSpendToStore = "Product id does not spend to your kitchen center.";
            public const string EndswithProductNameNotContainSize = "Name must end with string: 'Size x' With x is a your chosen size options.";
            public const string ProductNameTypeChildNotAllowUpdate = "Name of product which is type CHILD does not allow update.";
            public const string ProductIdNotParentType = "Product id is not a PARENT type.";
        }

        public static class PartnerMessage
        {
            public const string DupplicatedPartnerName = "Name already exist in the system.";
            public const string DupplicatedWebUrl = "Web Url already exist in the system.";
            public const string DeactivePartner_Update = "Partner was deleted before, so this partner cannot update.";
            public const string DeactivePartner_Delete = "Partner cannot delete because that was deleted before.";
            public const string DeactivePartner_Get = "Partner cannot get because that was deleted before.";

        }

        public static class CashierMessage
        {
            public const string CashierIdNotBelongToKitchenCenter = "Cashier id does not belong to your kitchen center.";
        }
    }
}
