using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.PartnerProducts;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.PartnerProducts
{
    public class PartnerProductProfile : Profile
    {
        public PartnerProductProfile()
        {
            CreateMap<PartnerProduct, GetPartnerProductResponse>()
                .ForMember(dept => dept.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dept => dept.StoreName, opt => opt.MapFrom(src => src.StorePartner.Store.Name))
                .ForMember(dept => dept.PartnerName, opt => opt.MapFrom(src => src.StorePartner.Partner.Name))
                .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangePartnerProductStatus(src.Status)))
                .ReverseMap();

            CreateMap<PartnerProduct, GetPartnerProductForPrivateAPI>();
        }
    }
}
