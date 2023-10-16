using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Service.Services.Interfaces;

namespace MBKC.Service.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
    }
}
