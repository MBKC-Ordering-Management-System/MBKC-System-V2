using AutoMapper;
using MBKC.Service.DTOs.Brands;

using MBKC.Repository.Enums;
using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.Brands
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, GetBrandResponse>()
                            .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeBrandStatus(src.Status)));

            CreateMap<Brand, UpdateBrandRequest>().ReverseMap();
        }
    }
}
