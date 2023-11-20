namespace MBKC.PrivateAPI.Constants
{
    public static class APIEndPointConstant
    {
        private const string RootEndPoint = "/api";
        private const string ApiVersion = "/v1";
        private const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Configuration
        {
            public const string ConfigurationsEndPoint = ApiEndpoint + "/configurations";
        }

        public static class Store
        {
            public const string StoresEndPoint = ApiEndpoint + "/stores";
        }

        public static class Order
        {
            public const string OrdersEndPoint = ApiEndpoint + "/orders";
            public const string OrderEndPoint = OrdersEndPoint + "/{id}";
        }

        public static class UserDevice
        {
            public const string UserDevicesEndPoint = ApiEndpoint + "user-devices";
            public const string UserDeviceEndPoint = UserDevicesEndPoint + "/{id}";
        }
    }
}
