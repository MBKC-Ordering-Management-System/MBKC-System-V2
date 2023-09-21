using AutoMapper;
using MBKC.BAL.DTOs.Verifications;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Profiles.EmailVerifications
{
    public class EmailVerificationProfile: Profile
    {
        public EmailVerificationProfile()
        {
            CreateMap<EmailVerification, EmailVerificationRedisModel>().ReverseMap();
        }
    }
}
