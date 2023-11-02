using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.OrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.OrderDetails
{
    public class OrderDetailProfile : Profile
    {
        public OrderDetailProfile()
        {
            CreateMap<OrderDetail, GetOrderDetailResponse>();
        }
    }
}