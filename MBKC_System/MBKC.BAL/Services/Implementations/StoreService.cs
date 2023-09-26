using AutoMapper;
using MBKC.BAL.DTOs.Stores;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<GetStoresResponse> GetStoresAsync(string? searchValue, int? currentPage, int? itemPerPage)
        {
            try
            {
                 this._unitOfWork.StoreRepository
            } catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
