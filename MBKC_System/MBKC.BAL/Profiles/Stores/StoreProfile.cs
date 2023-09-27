using AutoMapper;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.Stores
{
    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<Store, GetStoreResponse>().ForMember(x => x.StoreManagerEmail, opt => 
                                                                                     opt.MapFrom(src => 
                                                                                                 src.StoreAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.STORE_MANAGER 
                                                                                                                                    && x.Account.Status == (int)StoreEnum.Status.ACTIVE).Account.Email))
                                                .ForMember(x => x.Status, opt => opt.MapFrom(src => StatusUtil.ChangeStoreStatus(src.Status)));
        }
    }
}
