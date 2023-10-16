using AutoMapper;
using MBKC.Repository.GrabFood.Models;
using MBKC.Repository.Infrastructures;
using MBKC.Service.DTOs.BrandPartners.Requests;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Service.Services.Implementations
{
    public class BrandPartnerService : IBrandPartnerService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public BrandPartnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task<List<GrabFoodAuthenticationResponse>> CreateBrandPartnerAsync(PostBrandPartnerRequest postBrandPartnerRequest)
        {
            try
            {
                List<GrabFoodAuthenticationResponse> grabFoodAuthenticationResponses = new List<GrabFoodAuthenticationResponse>();
                foreach (var brandPartner in postBrandPartnerRequest.BrandPartners)
                {
                    GrabFoodAccount grabFoodAccount = new GrabFoodAccount() { Username = brandPartner.Username, Password = brandPartner.Password };
                    GrabFoodAuthenticationResponse grabFoodAuthenticationResponse = await this._unitOfWork.GrabFoodRepository.LoginGrabFood(grabFoodAccount);
                    grabFoodAuthenticationResponses.Add(grabFoodAuthenticationResponse);
                }
                return grabFoodAuthenticationResponses;
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
