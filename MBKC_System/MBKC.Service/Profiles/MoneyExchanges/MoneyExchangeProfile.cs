using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.MoneyExchanges;

namespace MBKC.Service.Profiles.MoneyExchanges
{
    public class MoneyExchangeProfile : Profile
    {
        public MoneyExchangeProfile()
        {
            CreateMap<MoneyExchange, GetMoneyExchangeResponse>();
        }

    }
}
