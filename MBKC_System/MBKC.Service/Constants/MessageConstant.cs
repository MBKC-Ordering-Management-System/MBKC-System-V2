using MBKC.Repository.Models;

namespace MBKC.Service.Constants
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
            public const string NotExistOrderPartnerId = "Order parnter id does not exist in the system.";
            public const string NotExistCashierId = "Cashier id does not exist in the system.";
            public const string InvalidItemsPerPage = "Items per page number is required more than 0.";
            public const string InvalidCurrentPage = "Current page number is required more than 0.";
            public const string NotExistPartnerId = "Partner id does not exist in the system.";
            public const string NotExistAccountId = "Account id does not exist in the system.";
            public const string InvalidPartnerId = "Partner id is not suitable id in the system.";
            public const string CategoryIdNotBelongToBrand = "Category id does not belong to your brand.";
            public const string CategoryIdNotBelongToStore = "Category id does not belong to your store.";
            public const string AlreadyExistPartnerProduct = "Mapping product already exists in the system.";
            public const string NotExistPartnerProduct = "Mapping priduct does not exist in the system.";

        }

        public static class LoginMessage
        {
            public const string DisabledAccount = "Account has been disabled.";
            public const string InvalidEmailOrPassword = "Email or Password is invalid.";
        }

        public static class AccountMessage
        {
            public const string AccountIdNotBelongYourAccount = "Account id does not belong to your account.";
            public const string AccountNoLongerActive = "Your account is no longer active.";
        }

        public static class VerificationMessage
        {
            public const string NotAuthenticatedEmailBefore = "Email has not been previously authenticated.";
            public const string ExpiredOTPCode = "OTP code has expired.";
            public const string NotMatchOTPCode = "Your OTP code does not match the previously sent OTP code.";
        }

        public static class ReGenerationMessage
        {
            public const string InvalidAccessToken = "Access token is invalid.";
            public const string NotExpiredAccessToken = "Access token has not yet expired.";
            public const string NotExistAuthenticationToken = "You do not have the authentication tokens in the system.";
            public const string NotExistRefreshToken = "Refresh token does not exist in the system.";
            public const string NotMatchAccessToken = "Your access token does not match the registered access token.";
            public const string ExpiredRefreshToken = "Refresh token expired.";
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
            public const string NoOneAvailable = "There is no kitchen center available.";

        }

        public static class BrandMessage
        {
            public const string InvalidStatusFilter = "Key status filter is required ACTIVE, INACTIVE in the system.";
            public const string NotBelongToBrand = "Brand id does not belong to your brand.";
            public const string DeactiveBrand_Delete = "Brand cannot delete because that was deleted before.";
            public const string DeactiveBrand_Update = "Brand was deleted before, so this brand cannot update.";
            public const string ManagerEmailExisted = "Brand manager email already existed in the system.";
            public const string RoleNotSuitable = "Role is not suitable";
            public const string ProductNotBelongToBrand = "This product not belong to brand.";
            public const string KeySortNotExist = "Key sort are ASC or DESC";
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
            public const string StoresWithStatusNameParam = "Status is required some type such as: Active, Inactive, Rejected, Be comfirming.";
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
            public const string KeySortNotExist = "Key sort are ASC or DESC";
        }

        public static class BankingAccountMessage
        {
            public const string BankingAccountNotBelongToKitchenCenter = "Banking account id does not belong to your kitchen center.";
            public const string NumberAccountExisted = "Number account already existed in the system.";
            public const string BankingAccountIsInactive = "Banking account is no longer active.";
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
            public const string ProductNameNotFollowingFormat = "Name of product Type CHILD is required following format: 'ParentName - Size x' With x is a your chosen size options.";
            public const string ProductNameTypeChildNotAllowUpdate = "Name of product which is type CHILD does not allow update.";
            public const string ProductIdNotParentType = "Product id is not a PARENT type.";
            public const string ExcelFileHasNoData = "This excel file has no data.";
            public const string DuplicateProductCode = "Your excel file is duplicating the product code.";
        }

        public static class PartnerMessage
        {
            public const string DupplicatedPartnerName = "Name already exist in the system.";
            public const string DupplicatedWebUrl = "Web Url already exist in the system.";
            public const string DeactivePartner_Update = "Partner was deleted before, so this partner cannot update.";
            public const string DeactivePartner_Delete = "Partner cannot delete because that was deleted before.";
            public const string DeactivePartner_Get = "Partner cannot get because that was deleted before.";
            public const string KeySortNotExist = "Key sort are ASC or DESC";
            public const string PartnerHasPartnerStoreActive_Update = "Partner can not update status because active stores is using this partner.";
            public const string PartnerHasPartnerStoreActive_Delete = "Partner can not delete status because active stores is using this partner.";


        }

        public static class CashierMessage
        {
            public const string CashierIdNotBelongToKitchenCenter = "Cashier id does not belong to your kitchen center.";
            public const string CashierIdNotBelogToCashier = "Cashier id is not suitable with your account.";
            public const string StatusIsRequiredWithKitchenCenterManager = "Status is not null";
            public const string StatusIsNotRequiredWithCashier = "Cashier does not allow to update Status property.";
            public const string NoOneAvailable = "There is no cashier available.";
        }

        public static class StorePartnerMessage
        {
            public const string InactiveStore_Create = "This store has been inactive or disabled.";
            public const string StoreNotBelongToBrand = "Store does not belong to brand.";
            public const string LinkedWithParner = "This store is already linked to this partner.";
            public const string UsernameExisted = "Username already exist in the system.";
            public const string NotLinkedWithParner = "This store is not linked to this partner and it is still active.";
            public const string DeactiveStorePartner_Update = "Can't update store partner has been deactivated.";
            public const string DupplicatedPartnerId_Create = "Partner Id cannot be duplicated in the partnerAccounts list.";
            public const string KeySortNotExist = "Key sort are ASC or DESC";
            public const string GrabFoodAccountMustBeStoreManager = "GrabFood Account must be Store Manager Role.";
            public const string ItemOnGrabfoodCanNotMapping = "The item on GrabFood cannot be mapped to any product in the MBKC System.";
            public const string ModifierGroupOnGrabfoodCanNotMapping = "The modifier group on GrabFood cannot be mapped to any product in the MBKC System.";
            public const string BrandHasNoActiveProduct = "Brand has no active products";
        }

        public static class PartnerProductMessage
        {
            public const string ProductCodeExisted = "Product Code already exist in the system.";
            public const string DeactiveProduct_Create_Update = "This product is Deactive.";
            public const string InactiveProduct_Create_Update = "This product is Inactive.";
            public const string InactiveStore_Update = "This store has been inactive.";
            public const string StatusInValid = "Status is AVAILABLE, OUT_OF_STOCK_TODAY or OUT_OF_STOCK_INDENTIFINITELY.";
            public const string KeySortNotExist = "Key sort are ASC or DESC";
            public const string ProductCodeNotExistInGrabFoodSystem = "Product code does not exist in the GrabFood System.";
            public const string PriceNotMatchWithProductInGrabFoodSystem = "Price does not match with product in the GrabFood System.";
            public const string StatusNotMatchWithProductInGrabFoodSystem = "Status does not match with product status in the GrabFood System.";
        }

        public static class OrderMessage
        {
            public const string OrderNotBelongToKitchenCenter = "Order partner id does not belong to your kitchen center.";
            public const string OrderShipperPhoneNotMatch = "Shipper phone does not match with shipper phone in order.";
            public const string OrderIsPreparing = "This order is PREPARING status, so You can not confirm completed order.";
            public const string OrderIsReady = "This order is READY status, so You can not confirm completed order.";
            public const string OrderIsUpcoming = "This order is UPCOMING status, so You can not confirm completed order.";
            public const string OrderIsCompleted = "This order is already COMPLETED status, so You can not confirm completed order..";
            public const string OrderIsCancelled = "This order has been CANCELLED status, so You can not confirm completed order.";
            public const string OrderAlreadyPaid = "This order has been paid, so You can not confirm completed order.";
        }

        public static class WalletMessage
        {
            public const string BalanceIsInvalid = "Your balance is not enough to transfer money.";
        }

        public static class MoneyExchangeMessage
        {
            public const string StoreIdNotBelogToKitchenCenter = "Store id does not belong to your kitchen center.";
            public const string BalanceIsInvalid = "This store balance is invalid.";
            public const string BalanceDoesNotEnough = "This store does not have enough balance to make a withdraw.";
            public const string AlreadyTransferredToStore = "The money has been transferred to the store today.";
            public const string AlreadyTransferredToKitchenCenter = "The money has been transferred to the kitchen center today.";
            public const string TransferToStoreSuccessfully = "Transfer money to store successfully.";
            public const string TransferToKitchenCenterSuccessfully = "Transfer money to kitchen center successfully.";
            public const string NotExistJobId = "Job id is not existed in the MBKC System.";
            public const string ConfigDoesNotExist = "Config to run background job does not exist.";
            public const string TimeKitchenCenterMustEarlier = "The scheduling time of money transfer to the kitchen center must be at least 1 minute earlier than the money transfer time to the store.";
            public const string TimeStoreMustLater = "The scheduling time of money transfer to the store must be at least 1 minute later than the money transfer time to the kitchen center.";
        }

    }
}
