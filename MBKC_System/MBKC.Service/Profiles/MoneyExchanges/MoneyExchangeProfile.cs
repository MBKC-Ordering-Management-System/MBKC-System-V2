using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.MoneyExchanges
{
    public class MoneyExchangeProfile : Profile
    {
        public MoneyExchangeProfile()
        {
            CreateMap<MoneyExchange, GetMoneyExchangeResponse>()
                .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeMoneyExchangeStatus(src.Status)))
                .ForMember(dept => dept.ExchangeType, opt => opt.MapFrom(src => StatusUtil.ChangeMoneyExchangeType(src.ExchangeType)));
        }
    }
}
