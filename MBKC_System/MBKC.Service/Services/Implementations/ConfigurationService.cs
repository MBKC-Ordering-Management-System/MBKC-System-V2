using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class ConfigurationService: IConfigurationService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ConfigurationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<List<GetConfigurationResponse>> GetConfigurationsAsync()
        {
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                return this._mapper.Map<List<GetConfigurationResponse>>(configurations);
            } catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
