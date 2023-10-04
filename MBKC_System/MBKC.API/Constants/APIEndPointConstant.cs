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
            public const string UpdatingStatusProductEndpoint = ProductEndpoint + "/updating-status";
        }

        public static class Partner
        {
            public const string PartnersEndpoint = ApiEndpoint + "/partners";
            public const string PartnerEndpoint = PartnersEndpoint + "/{id}";
        }

    }
}
