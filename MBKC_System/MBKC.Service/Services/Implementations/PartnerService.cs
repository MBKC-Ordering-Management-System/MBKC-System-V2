using AutoMapper;
using MBKC.Service.Services.Interfaces;
using MBKC.Repository.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Exceptions;
using MBKC.Service.Constants;
using MBKC.Service.Utils;
using MBKC.Repository.Models;
using MBKC.Repository.Enums;

namespace MBKC.Service.Services.Implementations
{
    public class PartnerService : IPartnerService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public PartnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreatePartner(PostPartnerRequest postPartnerRequest)
        {
            string folderName = "Partners";
            string logoId = "";
            bool uploaded = false;
            try
            {
                var checkDupplicatedName = await _unitOfWork.PartnerRepository.GetPartnerByNameAsync(postPartnerRequest.Name);
                var checkDupplicatedUrl = await _unitOfWork.PartnerRepository.GetPartnerByWebUrlAsync(postPartnerRequest.WebUrl);
                if (checkDupplicatedName != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedPartnerName);
                }

                if (checkDupplicatedUrl != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedWebUrl);
                }
                // Upload image to firebase
                FileStream fileStream = FileUtil.ConvertFormFileToStream(postPartnerRequest.Logo);
                Guid guild = Guid.NewGuid();
                logoId = guild.ToString();
                string urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, logoId);
                if (urlImage != null)
                {
                    uploaded = true;
                }

                Partner partner = new Partner()
                {
                    Name = postPartnerRequest.Name,
                    Status = (int)PartnerEnum.Status.ACTIVE,
                    WebUrl = postPartnerRequest.WebUrl,
                    Logo = urlImage + $"&logoId={logoId}"
                };
                await this._unitOfWork.PartnerRepository.CreatePartnerAsync(partner);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.PartnerMessage.DupplicatedPartnerName))
                {
                    fieldName = "Name";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.DupplicatedWebUrl))
                {
                    fieldName = "Web Url";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (uploaded)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
