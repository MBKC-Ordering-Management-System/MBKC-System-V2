using AutoMapper;
using MBKC.BAL.Services.Interfaces;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
{
    public class ExtraCategoryService : IExtraCategoryService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ExtraCategoryService(IUnitOfWork unitOfWork, IMapper mapper)

    public class KitchenCenterMoneyExchangeService : IKitchenCenterMoneyExchangeService
    {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;

            
            
        }

        
    }
}
