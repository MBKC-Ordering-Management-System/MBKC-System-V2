using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.OrdersHistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.OrderHistories
{
    public class OrderHistoryProfile : Profile
    {
        public OrderHistoryProfile()
        {
            CreateMap<OrderHistory, OrderHistoryResponse>().ReverseMap();
        }
    }
}
