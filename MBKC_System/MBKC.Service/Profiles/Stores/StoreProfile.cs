using AutoMapper;
using MBKC.Service.DTOs.Stores;

using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.Stores
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<Store, GetStoreResponse>().ForMember(x => x.Status, opt => opt.MapFrom(src => StatusUtil.ChangeStoreStatus(src.Status)))
                                                .ForMember(x => x.WalletBalance, opt => opt.MapFrom(src => src.Wallet.Balance))
                                                .ForMember(x => x.UserDevices, opt => opt.MapFrom(src => src.StoreAccounts.FirstOrDefault().Account.UserDevices));

            CreateMap<Store, GetStoreResponseForPrivateAPI>().ForMember(x => x.UserDevices, opt => opt.MapFrom(src => src.StoreAccounts.FirstOrDefault().Account.UserDevices))
                                                             .ForMember(x => x.Address, opt => opt.MapFrom(src => src.KitchenCenter.Address));
        }
    }
}
