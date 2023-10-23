using AutoMapper;
using MBKC.Service.DTOs.KitchenCenters;

using MBKC.Repository.Models;
using MBKC.Repository.Redis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.Utils;

namespace MBKC.Service.Profiles.KitchenCenters
{
    public class KitchenCenterProfile : Profile
    {
        public KitchenCenterProfile()
        {
            CreateMap<KitchenCenter, GetKitchenCenterResponse>().ForMember(dept => dept.KitchenCenterManagerEmail, opt => opt.MapFrom(src => src.Manager.Email))
                                                                .ForMember(dept => dept.Status, opt => opt.MapFrom(src => StatusUtil.ChangeKitchenCenterStatus(src.Status)));
        }
    }
}
