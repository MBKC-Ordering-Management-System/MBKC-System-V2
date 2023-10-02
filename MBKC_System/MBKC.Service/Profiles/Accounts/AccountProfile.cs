using AutoMapper;
using MBKC.Service.DTOs.Accounts;
using MBKC.Repository.Models;
using MBKC.Repository.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Accounts
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountResponse>().ForMember(dept => dept.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
        }
    }
}