using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.StorePartners;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.StorePartners
{
    public class StorePartnerProfile : Profile
    {
        public StorePartnerProfile()
        {
            CreateMap<StorePartner, GetStorePartnerResponse>()
               .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeBrandStatus(src.Status)))
               .ForMember(dept => dept.PartnerName, opt => opt.MapFrom(src => src.Partner.Name))
               .ForMember(dept => dept.PartnerLogo, opt => opt.MapFrom(src => src.Partner.Logo))
               .ReverseMap();

            CreateMap<StorePartner, GetStorePartnerForPrivateAPI>();
            CreateMap<StorePartner, GetStorePartnerWithPartnerOnlyResponse>();
        }
    }
}
