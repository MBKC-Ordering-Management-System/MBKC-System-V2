using AutoMapper;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.Brands
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, GetBrandResponse>()
                            .ForMember(dept => dept.BrandManagerEmail, opt => opt.MapFrom(src => src.BrandAccounts.FirstOrDefault(x => x.Account.Role.RoleId == (int)RoleEnum.Role.BRAND_MANAGER 
                                                                                                                                    && x.Account.Status == (int)AccountEnum.Status.ACTIVE).Account.Email))
                            .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeBrandStatus(src.Status)));

            CreateMap<Brand, UpdateBrandRequest>().ReverseMap();
        }
    }
}
