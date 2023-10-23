using AutoMapper;
using MBKC.Service.DTOs.AccountTokens;
using MBKC.Repository.Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Repository.Models;

namespace MBKC.Service.Profiles.AccountTokens
{
    public class AccountTokenProfile: Profile
    {
        public AccountTokenProfile()
        {
            CreateMap<DTOs.AccountTokens.AccountToken, Repository.Redis.Models.AccountToken>().ReverseMap();
        }
    }
}
