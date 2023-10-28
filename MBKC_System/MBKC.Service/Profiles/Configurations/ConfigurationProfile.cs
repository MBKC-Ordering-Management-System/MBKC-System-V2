using AutoMapper;
using MBKC.Repository.Models;
using MBKC.Service.DTOs.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Profiles.Configurations
{
    public class ConfigurationProfile: Profile
    {
        public ConfigurationProfile()
        {
            CreateMap<Configuration, GetConfigurationResponse>();
        }
    }
}
