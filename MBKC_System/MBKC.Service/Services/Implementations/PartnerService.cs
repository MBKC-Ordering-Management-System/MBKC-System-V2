using AutoMapper;
using MBKC.Repository.Enums;
using MBKC.Repository.Infrastructures;
using MBKC.Repository.Models;
using MBKC.Service.Constants;
using MBKC.Service.DTOs.Partners;
using MBKC.Service.Exceptions;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;

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

        public async Task UpdatePartnerAsync(int partnerId, UpdatePartnerRequest updatePartnerRequest)
        {
            string folderName = "Partners";
            string logoId = "";
            bool isUploaded = false;
            bool isDeleted = false;
            try
            {
                if (partnerId <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }
                var partner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(partnerId);
                if (partner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }
                if (partner.Status == (int)PartnerEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Update);
                }

                var checkDupplicatedName = await _unitOfWork.PartnerRepository.GetPartnerByNameAsync(updatePartnerRequest.Name);
                var checkDupplicatedWebUrl = await _unitOfWork.PartnerRepository.GetPartnerByWebUrlAsync(updatePartnerRequest.WebUrl);

                if (checkDupplicatedName != null && checkDupplicatedName.PartnerId != partnerId)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DupplicatedPartnerName);
                }
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
                    await this._unitOfWork.FirebaseStorageRepository.DeleteImageAsync(FileUtil.GetImageIdFromUrlImage(oldLogo, "logoId"), folderName);
                    isDeleted = true;
                }

                partner.Name = updatePartnerRequest.Name;
                partner.WebUrl = updatePartnerRequest.WebUrl;

                if (updatePartnerRequest.Status.ToLower().Equals(PartnerEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    partner.Status = (int)PartnerEnum.Status.ACTIVE;
                }
                else if (updatePartnerRequest.Status.ToLower().Equals(PartnerEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    partner.Status = (int)PartnerEnum.Status.INACTIVE;
                    //Inactive store partner
                    if (partner.StorePartners.Any())
                    {
                        foreach (var p in partner.StorePartners)
                        {
                            p.Status = (int)PartnerEnum.Status.INACTIVE;
                        }
                    }
                }

                _unitOfWork.PartnerRepository.UpdatePartner(partner);
                await _unitOfWork.CommitAsync();

            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId))
                {
                    fieldName = "Partner id";
                }
                else if (ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Update))
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

        public async Task<GetPartnersResponse> GetPartnersAsync(string? keySearchName, int? pageNumber, int? pageSize, bool? isGetAll)
        {
            var partners = new List<Partner>();
            var partnerResponse = new List<GetPartnerResponse>();
            if (pageNumber != null && pageNumber <= 0)
            {
                throw new BadRequestException(MessageConstant.CommonMessage.InvalidCurrentPage);
            }
            else if (pageNumber == null)
            {
                pageNumber = 1;
            }
            if (pageSize != null && pageSize <= 0)
            {
                throw new BadRequestException(MessageConstant.CommonMessage.InvalidItemsPerPage);
            }
            else if (pageSize == null)
            {
                pageSize = 5;
            }

            int numberItems = 0;
            if (keySearchName != null && StringUtil.IsUnicode(keySearchName))
            {
                numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(keySearchName, null);
                partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(keySearchName, null, pageSize.Value, pageNumber.Value);
            }
            else if (keySearchName != null && StringUtil.IsUnicode(keySearchName) == false)
            {
                numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(null, keySearchName);
                partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(null, keySearchName, pageSize.Value, pageNumber.Value);
            }
            else if (keySearchName == null)
            {
                numberItems = await this._unitOfWork.PartnerRepository.GetNumberPartnersAsync(null, null);
                partners = await this._unitOfWork.PartnerRepository.GetPartnersAsync(null, null, pageSize.Value, pageNumber.Value);
            }
            this._mapper.Map(partners, partnerResponse);

            int totalPages = 0;
            if (numberItems > 0 && isGetAll == null || numberItems > 0 && isGetAll != null && isGetAll == false)
            {
                totalPages = (int)((numberItems + pageSize.Value) / pageSize.Value);
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

        public async Task<GetPartnerResponse> GetPartnerByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new BadRequestException(MessageConstant.CommonMessage.InvalidPartnerId);
                }
                var existedPartner = await _unitOfWork.PartnerRepository.GetPartnerByIdAsync(id);
                if (existedPartner == null)
                {
                    throw new NotFoundException(MessageConstant.CommonMessage.NotExistPartnerId);
                }

                if (existedPartner.Status == (int)PartnerEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Get);
                }

                var partnerResponse = this._mapper.Map<GetPartnerResponse>(existedPartner);

                return partnerResponse;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals(MessageConstant.CommonMessage.InvalidPartnerId)
                    || ex.Message.Equals(MessageConstant.PartnerMessage.DeactivePartner_Get))
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

                if (partner.Status == (int)PartnerEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException(MessageConstant.PartnerMessage.DeactivePartner_Delete);
                }

                partner.Status = (int)PartnerEnum.Status.DEACTIVE;

                if (partner.StorePartners.Any())
                {
                    //Deactive store partner.
                    foreach (var p in partner.StorePartners)
                    {
                        p.Status = (int)StorePartnerEnum.Status.DEACTIVE;
                    }
                }

                this._unitOfWork.PartnerRepository.UpdatePartner(partner);
                this._unitOfWork.Commit();
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
