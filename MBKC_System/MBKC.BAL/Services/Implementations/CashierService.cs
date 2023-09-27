using AutoMapper;
using MBKC.BAL.Services.Interfaces;
using MBKC.DAL.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
{ 

    public class CashierService : ICashierService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CashierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
    }
}
