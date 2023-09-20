using AutoMapper;
using MBKC.BAL.DTOs.Accounts;
using MBKC.DAL.Models;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.Accounts
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountRedisModel>().ForMember(dept => dept.RoleId, opt => opt.MapFrom(src => src.Role.RoleId))
                                                   .ForMember(dept => dept.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<AccountResponse, AccountRedisModel>().ReverseMap();
        }
    }
}