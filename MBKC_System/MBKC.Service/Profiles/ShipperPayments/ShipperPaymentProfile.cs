using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.ShipperPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.ShipperPayments
{
    public class ShipperPaymentProfile : Profile
    {
        public ShipperPaymentProfile()
        {
            CreateMap<ShipperPayment, GetShipperPayemtResponse>();
        }
    }
}
