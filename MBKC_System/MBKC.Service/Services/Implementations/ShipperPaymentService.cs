using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Service.Services.Interfaces;

namespace MBKC.Service.Services.Implementations
{
    public class ShipperPaymentService : IShipperPaymentService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ShipperPaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
    }
}
