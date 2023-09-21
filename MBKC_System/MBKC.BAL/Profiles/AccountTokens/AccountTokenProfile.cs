using AutoMapper;
using MBKC.BAL.DTOs.AccountTokens;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.AccountTokens
{
    public class AccountTokenProfile: Profile
    {
        public AccountTokenProfile()
        {
            CreateMap<AccountToken, AccountTokenRedisModel>().ForMember(dept => dept.AccountId, opt => opt.MapFrom(src => src.AccountId.ToString()));
            CreateMap<AccountTokenRedisModel, AccountToken>().ForMember(dept => dept.AccountId, opt => opt.MapFrom(src => int.Parse(src.AccountId)));
        }
    }
}
