using AutoMapper;
using MBKC.Service.DTOs.Products;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetProductResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeProductStatusStatus(src.Status)))
                                                    .ForMember(dept => dept.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                                                    .ForMember(dept => dept.CategoryId, opt => opt.MapFrom(src => src.Category.CategoryId))
                                                    .ForMember(dest => dest.PartnerProducts, opt => opt.Ignore())
                                                    .ReverseMap();
        }
    }
}
