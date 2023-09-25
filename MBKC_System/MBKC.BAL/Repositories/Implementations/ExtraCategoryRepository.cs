using AutoMapper;
using MBKC.BAL.Repositories.Interfaces;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Repositories.Implementations
{
    public class ExtraCategoryRepository : IExtraCategoryRepository
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ExtraCategoryRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        
    }
}
