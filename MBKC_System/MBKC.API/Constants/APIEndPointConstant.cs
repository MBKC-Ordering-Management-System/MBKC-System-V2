namespace MBKC.API.Constants
{
    public static class APIEndPointConstant
    {
        private const string RootEndPoint = "/api";
        private const string ApiVersion = "/v1";
        private const string ApiEndpoint = RootEndPoint + ApiVersion;


        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/authentications";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string ReGenerationTokens = AuthenticationEndpoint + "/regeneration-tokens";
            public const string PasswordResetation = AuthenticationEndpoint + "/password-resetation";
        }

        public static class Verification
        {
            public const string VerificationEndpoint = ApiEndpoint + "/verifications";
            public const string EmailVerificationEndpoint = VerificationEndpoint + "/email-verification";
            public const string OTPVerificationEndpoint = VerificationEndpoint + "/otp-verification";
        }

        public static class KitchenCenter
        {
            public const string KitchenCentersEndpoint = ApiEndpoint + "/kitchen-centers";
            public const string KitchenCenterEndpoint = KitchenCentersEndpoint + "/{id}";
            public const string KitchenCenterProfileEndpoint = KitchenCentersEndpoint + "/Profile";
            public const string UpdatingStatusKitchenCenter = KitchenCenterEndpoint + "/updating-status";
        }

        public static class Brand
        {
            public const string BrandsEndpoint = ApiEndpoint + "/brands";
            public const string BrandEndpoint = BrandsEndpoint + "/{id}";
            public const string BrandProfileEndpoint = BrandsEndpoint + "/profile";
            public const string UpdatingStatusBrand = BrandEndpoint + "/updating-status";
            public const string UpdatingProfileBrand = BrandEndpoint + "/profile";
        }

        public static class Store
        {
            public const string StoresEndpoint = ApiEndpoint + "/stores";
            public const string StoreEndpoint = StoresEndpoint + "/{id}";
            public const string StoreProfileEndpoint = StoresEndpoint + "/profile";
            public const string UpdateingStatusStore = StoreEndpoint + "/updating-status";
            public const string ConfirmRegistrationStore = StoreEndpoint + "/confirming-registration";
            public const string ActiveAndInactiveStoresEndPoint = StoresEndpoint + "/active-inactive-stores";
        }

        public static class Category
        {
            public const string CategoriesEndpoint = ApiEndpoint + "/categories";
            public const string CategoryEndpoint = CategoriesEndpoint + "/{id}";
            public const string ExtraCategoriesEndpoint = CategoryEndpoint + "/extra-categories";
        }

        public static class BankingAccount
        {
            public const string BankingAccountsEndpoint = ApiEndpoint + "/banking-accounts";
            public const string BankingAccountEndpoint = BankingAccountsEndpoint + "/{id}";
            public const string UpdatingStatusBankingAccountEndpoint = BankingAccountEndpoint + "/updating-status";
        }

        public static class Product
        {
            public const string ProductsEndpoint = ApiEndpoint + "/products";
            public const string ProductEndpoint = ProductsEndpoint + "/{id}";
            public const string ProductWithNumberSoldEndpoint = ProductsEndpoint + "/products-sold";
            public const string UpdatingStatusProductEndpoint = ProductEndpoint + "/updating-status";
            public const string ImportFileEndpoint = ProductsEndpoint + "/import-file";
        }

        public static class Partner
        {
            public const string PartnersEndpoint = ApiEndpoint + "/partners";
            public const string PartnerEndpoint = PartnersEndpoint + "/{id}";
            public const string UpdatingPartnerStatusEndpoint = PartnerEndpoint + "/updating-status";
        }

        public static class Cashier
        {
            public const string CashiersEndpoint = ApiEndpoint + "/cashiers";
            public const string CashierEndpoint = CashiersEndpoint + "/{id}";
            public const string CashierProfileEndpoint = CashierEndpoint + "/profile";
            public const string UpdatingCashierStatusEndpoint = CashierEndpoint + "/updating-status";
            public const string CashierReportEndpoint = CashiersEndpoint + "/report";
        }

        public static class Account
        {
            public const string AccountEndpoint = ApiEndpoint + "/accounts" + "/{id}";
        }

        public static class StorePartner
        {
            public const string StorePartnersEndpoint = ApiEndpoint + "/store-partners";
            public const string StorePartnerEndpoint = StorePartnersEndpoint + "/stores/{storeId}/partners/{partnerId}";
            public const string UpdatingStatusStorePartnerEndpoint = StorePartnerEndpoint + "/updating-status";
            public const string PartnerInformationEndpoint = StorePartnersEndpoint + "/stores/{id}";
        }

        public static class PartnerProduct
        {
            public const string PartnerProductsEndpoint = ApiEndpoint + "/partner-products";
            public const string PartnerProductEndpoint = PartnerProductsEndpoint + "/products/{productId}/partners/{partnerId}/stores/{storeId}";
            public const string UpdatingStatusPartnerProductEndpoint = PartnerProductEndpoint + "/updating-status";

        }

        public static class Order
        {
            public const string OrdersEndpoint = ApiEndpoint + "/orders";
            public const string OrderEndpoint = OrdersEndpoint + "/{id}";
            public const string ConfirmOrderToCompletedEndpoint = OrdersEndpoint + "/confirm-order-to-completed";
            public const string ChangeOrderToReadyEndpoint = OrderEndpoint + "/change-order-to-ready";
            public const string ChangeOrderToReadyDeliveryEndpoint = OrderEndpoint + "/change-order-to-ready-delivery";
            public const string CancelOrderEndpoint = OrderEndpoint + "/cancel-order";

        }

        public static class MoneyExchange
        {
            public const string MoneyExchangesEndpoint = ApiEndpoint + "/money-exchanges";
            public const string MoneyExchangeToKitchenCenter = MoneyExchangesEndpoint + "/money-exchange-to-kitchen-center";
            public const string WithdrawMoneyToStore = MoneyExchangesEndpoint + "/withdraw-money-to-store";
            public const string MoneyExchangesWithDrawEndpoint = MoneyExchangesEndpoint + "/withdraw-type";
        }

        public static class Configuration
        {
            public const string ConfigurationsEndpoint = ApiEndpoint + "/configurations";
            public const string ConfigurationEndpoint = ConfigurationsEndpoint + "/{id}";
        }

        public static class Wallet
        {
            public const string WalletsEndpoint = ApiEndpoint + "/wallets";
            public const string WalletEndpoint = WalletsEndpoint + "/transaction-money-exchange-shipper-payment-information";
        }

        public static class ShipperPayment
        {
            public const string ShipperPaymentsEndpoint = ApiEndpoint + "/shipper-payments";
        }

        public static class DashBoard
        {
            public const string DashBoardsEndpoint = ApiEndpoint + "/dashboards";
            public const string AdminDashBoardEndpoint = DashBoardsEndpoint + "/admin";
            public const string KitchenCenterDashBoardEndpoint = DashBoardsEndpoint + "/kitchen-center";
            public const string BrandDashBoardEndpoint = DashBoardsEndpoint + "/brand";
            public const string StoreDashBoardEndpoint = DashBoardsEndpoint + "/store";
            public const string CashierDashBoardEndpoint = DashBoardsEndpoint + "/cashier";
        }

        public static class UserDevice
        {
            public const string UserDevicesEndpoint = ApiEndpoint + "/user-devices";
            public const string UserDeviceEndPoint = UserDevicesEndpoint + "/{id}";
        }
    }
}
