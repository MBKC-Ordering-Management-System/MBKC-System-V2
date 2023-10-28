using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Partners
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, GetPartnerResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangePartnerStatus(src.Status)))
                .ReverseMap();

            CreateMap<Partner, GetPartnerForPrivateAPI>();
        }
    }
}
