using MBKC.Service.DTOs.ShipperPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Interfaces
{
    public interface IShipperPaymentService
    {
        public Task<GetShipperPaymentsResponse> GetShipperPayments(GetShipperPaymentsRequest getShipperPaymentsRequest, IEnumerable<Claim> claims);
    }
}
