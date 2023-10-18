using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.PartnerProducts;
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
                .ReverseMap();
        }
    }
}
