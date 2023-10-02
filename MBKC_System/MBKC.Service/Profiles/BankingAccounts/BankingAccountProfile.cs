using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.BankingAccounts
{
    public class BankingAccountProfile : Profile
    {
        public BankingAccountProfile()
        {
            CreateMap<BankingAccount, GetBankingAccountResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeBankingAccountStatus(src.Status)));
        }
    }
}
