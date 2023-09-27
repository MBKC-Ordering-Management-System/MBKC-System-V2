using AutoMapper;
using MBKC.BAL.DTOs.Products;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetProductResponse>().ReverseMap();
        }
    }
}
