using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.ShipperPayments;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.ShipperPayments
{
    public class ShipperPaymentProfile : Profile
    {
        public ShipperPaymentProfile()
        {
            CreateMap<ShipperPayment, GetShipperPaymentOrderResponse>();
            CreateMap<ShipperPayment, GetShipperPaymentResponse>()
                .ForMember(dept => dept.KCBankingAccountName, opt => opt.MapFrom(src => src.BankingAccount.Name))
                .ForMember(dept => dept.FinalTotalPrice, opt => opt.MapFrom(src => src.Order.FinalTotalPrice))
                .ForMember(dept => dept.OrderPartnerId, opt => opt.MapFrom(src => src.Order.OrderPartnerId))
                .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeShipperPaymentStatus(src.Status)));

        }
    }
}
