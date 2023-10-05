using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Constants
{
    public static class EmailMessageConstant
    {
        public static class CommonMessage
        {
            public const string Message = "Here is email and password to access to the system.";
        }
        public static class KitchenCenter
        {
            public const string Message = "You have been assigned as Kitchen Center Manager for the kitchen center";
        }

        public static class Brand
        {
            public const string Message = "You have been assigned as Brand Manager for the brand";
        }

        public static class Store
        {
            public const string Message = "You have been assigned as Store Manager for the store";
        }

        public static class Cashier
        {
            public const string Message = "You have been created as Cashier for the kitchen center";
        }
    }
}
