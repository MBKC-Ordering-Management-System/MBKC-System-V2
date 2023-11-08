using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.MoneyExchanges;
using MBKC.Service.DTOs.Transactions;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Transactions
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, GetTransactionResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeTransactionStatus(src.Status)));
            CreateMap<Transaction, GetTransactionWalletResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeTransactionStatus(src.Status)));
        }
    }
}
