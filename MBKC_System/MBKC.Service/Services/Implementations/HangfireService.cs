using AutoMapper;
using Hangfire;
using MBKC.Repository.Infrastructures;
using MBKC.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class HangfireService : IHangfireService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public HangfireService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }
        public async Task MoneyExchangeToStoreAsync()
        {
            await Task.Delay(1);
            Console.WriteLine($"Execute here at: {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
        }
    }
}
