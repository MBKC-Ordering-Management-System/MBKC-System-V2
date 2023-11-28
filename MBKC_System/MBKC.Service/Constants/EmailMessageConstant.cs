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

        public static class StorePartner
        {
            public const string Message = "The MBKC system has just received a request from you to map products in the MBKC system and GrabFood's system. " +
                                          "The system cannot perform mapping of some products in the Excel file attached to this email from GrabFood. " +
                                          "Please review whether the product in the MKBC system is in accordance with regulations to perform mapping or not.";
        }

        public static class Order
        {
            public const string Message = "The MBKC system has automatically canceled orders from your store because these orders were not processed today.";
        }
    }
}
