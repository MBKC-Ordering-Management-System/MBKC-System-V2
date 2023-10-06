using AutoMapper;
using MBKC.Service.DTOs.Categories;

using MBKC.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, GetCategoryResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeCategoryStatus(src.Status)))
                                                      .ForMember(dept => dept.ExtraCategories, opt => opt.MapFrom(src => src.ExtraCategoryProductCategories))
                                                      .ReverseMap();
            CreateMap<Category, GetExtraCategoryResponse>().ReverseMap();
        }
    }
}
