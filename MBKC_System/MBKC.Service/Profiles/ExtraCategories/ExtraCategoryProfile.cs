using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.ExtraCategories
{
    public class ExtraCategoryProfile : Profile
    {
        public ExtraCategoryProfile()
        {
            CreateMap<ExtraCategory, GetExtraCategoryResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeCategoryStatus(src.ExtraCategoryNavigation.Status)))
                                                                .ForMember(dept => dept.Code, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.Code))
                                                                .ForMember(dept => dept.Description, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.Description))
                                                                .ForMember(dept => dept.Name, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.Name))
                                                                .ForMember(dept => dept.Type, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.Type))
                                                                .ForMember(dept => dept.DisplayOrder, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.DisplayOrder))
                                                                .ForMember(dept => dept.ImageUrl, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.ImageUrl))
                                                                .ForMember(dept => dept.ExtraProducts, opt => opt.MapFrom(src => src.ExtraCategoryNavigation.Products)).ReverseMap();
        }
    }
}
