using AutoMapper;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.DAL.Models;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.KitchenCenters
{
    public class KitchenCenterProfile : Profile
    {
        public KitchenCenterProfile()
        {
            CreateMap<KitchenCenter, GetKitchenCenterResponse>().ForMember(dept => dept.KitchenCenterManagerEmail, opt => opt.MapFrom(src => src.Manager.Email));
        }
    }
}
