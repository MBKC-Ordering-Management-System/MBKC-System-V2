using AutoMapper;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Configurations;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Newtonsoft.Json;
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

        public async Task UpdateConfigurationAsync(PutConfigurationRequest putConfigurationRequest)
        {
            try
            {
                List<Configuration> configurations = await this._unitOfWork.ConfigurationRepository.GetConfigurationsAsync();
                Configuration firstConfiguration = configurations.First();
                bool isChanged = false;
                TimeSpan startTime;
                TimeSpan endTime;

                TimeSpan.TryParse(putConfigurationRequest.ScrawlingOrderStartTime, out startTime);
                TimeSpan.TryParse(putConfigurationRequest.ScrawlingOrderEndTime, out endTime);

                if(TimeSpan.Compare(firstConfiguration.ScrawlingOrderStartTime, startTime) != 0 || TimeSpan.Compare(firstConfiguration.ScrawlingOrderEndTime, endTime) != 0)
                {
                    isChanged = true;
                }

                firstConfiguration.ScrawlingOrderStartTime = startTime;
                firstConfiguration.ScrawlingOrderEndTime = endTime;
                this._unitOfWork.ConfigurationRepository.UpdateConfigurationAsync(firstConfiguration);
                await this._unitOfWork.CommitAsync();
                if (isChanged)
                {
                    string jsonMessage = JsonConvert.SerializeObject(firstConfiguration);
                    this._unitOfWork.RabbitMQRepository.SendMessage(jsonMessage, "Modified_Configurations");
                }
            } catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
