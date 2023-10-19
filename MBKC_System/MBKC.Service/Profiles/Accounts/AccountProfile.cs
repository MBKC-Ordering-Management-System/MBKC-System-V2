using AutoMapper;
using MBKC.Service.DTOs.Accounts;
using MBKC.Repository.Models;
using MBKC.Repository.Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.Accounts
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountResponse>().ForMember(dept => dept.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<Account, GetAccountResponse>().ForMember(dept => dept.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                                                    .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeAccountStatus(src.Status)));
        }
    }
}