using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Cashiers.Responses;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.Utils;
using Redis.OM.Searching.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Cashiers
{
    public class CashierProfile : Profile
    {
        public CashierProfile()
        {
            CreateMap<Cashier, GetCashierResponse>().ForMember(dept => dept.Email, opt => opt.MapFrom(src => src.Account.Email))
                                                    .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeCashierStatus(src.Account.Status)))
                                                    .ForMember(dept => dept.Gender, otp => otp.MapFrom(src => src.Gender ? CashierEnum.Gender.MALE.ToString().First() + CashierEnum.Gender.MALE.ToString().Substring(1).ToLower() : CashierEnum.Gender.FEMALE.ToString().First() + CashierEnum.Gender.FEMALE.ToString().Substring(1).ToLower()))
                                                    .ReverseMap();
        }
    }
}
