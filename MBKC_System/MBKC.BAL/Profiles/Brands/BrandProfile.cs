﻿using AutoMapper;
using MBKC.BAL.DTOs.Brands;
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
            CreateMap<Brand, GetBrandResponse>().ReverseMap();
            CreateMap<Brand, UpdateBrandRequest>().ReverseMap();
        }
    }
}
