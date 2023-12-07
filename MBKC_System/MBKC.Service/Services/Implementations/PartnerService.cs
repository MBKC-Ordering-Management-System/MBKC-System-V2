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
                var checkDupplicatedWebUrl = await _unitOfWork.PartnerRepository.GetPartnerByWebUrlAsync(postPartnerRequest.WebUrl);
                if (checkDupplicatedName != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedPartnerName);
                }

                if (checkDupplicatedWebUrl != null)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedWebUrl);
                }
                // Upload image to firebase
                if (postPartnerRequest.Logo != null)
                {
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
                else
                {
                    Partner partner = new Partner()
                    {
                        Name = postPartnerRequest.Name,
                        Status = (int)PartnerEnum.Status.ACTIVE,
                        WebUrl = postPartnerRequest.WebUrl,
                        Logo = null
                    };
                    await this._unitOfWork.PartnerRepository.CreatePartnerAsync(partner);
                    await this._unitOfWork.CommitAsync();
                }
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

        public async Task UpdatePartnerAsync(int partnerId, UpdatePartnerRequest updatePartnerRequest)
        {
            string folderName = "Partners";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                var partner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }
                if (partner.Status == (int)PartnerEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Update);
                }

                var checkDupplicatedWebUrl = await _unitOfWork.PartnerRepository.GetPartnerByWebUrlAsync(updatePartnerRequest.WebUrl);

                if (checkDupplicatedWebUrl != null && checkDupplicatedWebUrl.PartnerId != partnerId)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedWebUrl);
                }

                string oldLogo = partner.Logo;
                if (updatePartnerRequest.Logo != null)
                {
                    // Upload image to firebase
                    FileStream fileStream = FileUtil.ConvertFormFileToStream(updatePartnerRequest.Logo);
                    Guid guild = Guid.NewGuid();
                    logoId = guild.ToString();
                    var urlImage = await this._unitOfWork.FirebaseStorageRepository.UploadImageAsync(fileStream, folderName, logoId);
                    if (urlImage != null)
                    {
                        isUploaded = true;
                    }
                    partner.Logo = urlImage + $"&logoId={logoId}";


                    //Delete image from database
                    if (oldLogo != null)
                    {
                        await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                        isDeleted = true;
                    }
                }
                partner.WebUrl = updatePartnerRequest.WebUrl;
                partner.TaxCommission = updatePartnerRequest.TaxCommission;
                if (updatePartnerRequest.Status.ToLower().Equals(PartnerEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    partner.Status = (int)PartnerEnum.Status.ACTIVE;
                }
                else if (updatePartnerRequest.Status.ToLower().Equals(PartnerEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    if (partner.StorePartners.Any(x => x.Status == (int)StorePartnerEnum.Status.ACTIVE))
                    {
                        throw new BadRequestException(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Update);
                    }
                    partner.Status = (int)PartnerEnum.Status.INACTIVE;
                }


                _unitOfWork.PartnerRepository.UpdatePartner(partner);
                await _unitOfWork.CommitAsync();

            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Update))
                {
                    fieldName = "Updated partner failed";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Update))
                {
                    fieldName = "Updated partner failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded && isDeleted == false)
                {
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetPartnersResponse> GetPartnersAsync(GetPartnersRequest getPartnersRequest)
        {
            try
            {
                var partners = new List<Partner>();
                var partnerResponse = new List<GetPartnerResponse>();
                
                int numberItems = 0;
                if (getPartnersRequest.SearchValue != null && StringUtil.IsUnicode(getPartnersRequest.SearchValue))
                {
                    numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(getPartnersRequest.SearchValue, null);
                    partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(getPartnersRequest.SearchValue, null, getPartnersRequest.CurrentPage, getPartnersRequest.ItemsPerPage,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("asc") ? getPartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("desc") ? getPartnersRequest.SortBy.Split("_")[0] : null, getPartnersRequest.IsGetAll);
                }
                else if (getPartnersRequest.SearchValue != null && StringUtil.IsUnicode(getPartnersRequest.SearchValue) == false)
                {
                    numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(null, getPartnersRequest.SearchValue);
                    partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(null, getPartnersRequest.SearchValue, getPartnersRequest.CurrentPage, getPartnersRequest.ItemsPerPage,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("asc") ? getPartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("desc") ? getPartnersRequest.SortBy.Split("_")[0] : null, getPartnersRequest.IsGetAll);
                }
                else if (getPartnersRequest.SearchValue == null)
                {
                    numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(null, null);
                    partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(null, null, getPartnersRequest.CurrentPage, getPartnersRequest.ItemsPerPage,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("asc") ? getPartnersRequest.SortBy.Split("_")[0] : null,
                                                                                                              getPartnersRequest.SortBy != null && getPartnersRequest.SortBy.ToLower().EndsWith("desc") ? getPartnersRequest.SortBy.Split("_")[0] : null, getPartnersRequest.IsGetAll);
                }
                this._mapper.Map(partners, partnerResponse);

                int totalPages = 0;
                if (numberItems > 0 && getPartnersRequest.IsGetAll == null || numberItems > 0 && getPartnersRequest.IsGetAll != null && getPartnersRequest.IsGetAll == false)
                {
                    totalPages = (int)((numberItems + getPartnersRequest.ItemsPerPage) / getPartnersRequest.ItemsPerPage);
                }

                if (numberItems == 0)
                {
                    totalPages = 0;
                }
                return new GetPartnersResponse()
                {
                    Partners = partnerResponse,
                    NumberItems = numberItems,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetPartnerResponse> GetPartnerByIdAsync(int id)
        {
            try
            {
                var existedPartner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(id);
                if (existedPartner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                if (existedPartner.Status == (int)PartnerEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Get);
                }

                var partnerResponse = this._mapper.Map<GetPartnerResponse>(existedPartner);

                return partnerResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Get))
                {
                    fieldName = "partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeActivePartnerByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }
                var partner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(id);

                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                if (partner.Status == (int)PartnerEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Delete);
                }

                if (partner.StorePartners.Any(x => x.Status == (int)StorePartnerEnum.Status.ACTIVE))
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Delete);
                }
                partner.Status = (int)PartnerEnum.Status.DISABLE;

                this._unitOfWork.PartnerRepository.UpdatePartner(partner);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Delete))
                {
                    fieldName = "Delete partner failed";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Delete))
                {
                    fieldName = "Delete partner failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdatePartnerStatusAsync(int partnerId, UpdatePartnerStatusRequest updatePartnerStatusRequest)
        {
            try
            {
                var partner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);

                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                if (partner.Status == (int)PartnerEnum.Status.DISABLE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Delete);
                }

                if (updatePartnerStatusRequest.Status.ToLower().Equals(PartnerEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    partner.Status = (int)PartnerEnum.Status.ACTIVE;
                }
                else if (updatePartnerStatusRequest.Status.ToLower().Equals(PartnerEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    if (partner.StorePartners.Any(x => x.Status == (int)StorePartnerEnum.Status.ACTIVE))
                    {
                        throw new BadRequestException(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Update);
                    }
                    partner.Status = (int)PartnerEnum.Status.INACTIVE;
                }
                this._unitOfWork.PartnerRepository.UpdatePartner(partner);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Delete))
                {
                    fieldName = "Update partner failed";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.PartnerHasPartnerStoreActive_Update))
                {
                    fieldName = "Update partner failed";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.NotExistPartnerId))
                {
                    fieldName = "Partner id";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
