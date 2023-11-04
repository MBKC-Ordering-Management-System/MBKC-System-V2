using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Orders
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, GetOrderResponse>()
                .ForMember(dept => dept.SystemStatus, opt => opt.MapFrom(src => StatusUtil.ChangeSystemOrderStatus(src.SystemStatus)))
                .ForMember(dept => dept.PartnerOrderStatus, opt => opt.MapFrom(src => StatusUtil.ChangePartnerOrderStatus(src.PartnerOrderStatus)))
                .ForMember(dest => dest.TotalQuantity, opt => opt.Ignore());
        }
    }
}
