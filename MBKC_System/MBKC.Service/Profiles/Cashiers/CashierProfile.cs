using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Cashiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Cashiers
{
    public class CashierProfile : Profile
    {
        public CashierProfile()
        {
            CreateMap<Cashier, GetCashierResponse>().ReverseMap();
        }
    }
}
