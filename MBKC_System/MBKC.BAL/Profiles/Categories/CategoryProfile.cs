using AutoMapper;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.Utils;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, GetCategoryResponse>().ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeCategoryStatus(src.Status)))
                                                        .ReverseMap();
        }
    }
}
