using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.MappingProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.MappingProducts
{
    public class MappingProductProfile : Profile
    {
        public MappingProductProfile()
        {
            CreateMap<MappingProduct, GetMappingProductResponse>()
                .ForMember(dept => dept.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dept => dept.StoreName, opt => opt.MapFrom(src => src.StorePartner.Store.Name))
                .ForMember(dept => dept.PartnerName, opt => opt.MapFrom(src => src.StorePartner.Partner.Name))
                .ReverseMap();
        }
    }
}
