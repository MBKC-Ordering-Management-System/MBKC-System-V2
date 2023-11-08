using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.OrdersHistories;
using MBKC.Service.Utils;
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
            CreateMap<OrderHistory, OrderHistoryResponse>()
                .ForMember(dept => dept.SystemStatus, opt => opt.MapFrom(src => StatusUtil.ChangeSystemOrderStatus(src.SystemStatus)))
                .ForMember(dept => dept.PartnerOrderStatus, opt => opt.MapFrom(src => StatusUtil.ChangePartnerOrderStatus(src.PartnerOrderStatus)))
                .ReverseMap();
        }
    }
}
