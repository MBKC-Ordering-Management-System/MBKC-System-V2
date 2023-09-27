using AutoMapper;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.DAL.Enums;
using MBKC.DAL.Infrastructures;
using MBKC.DAL.Models;
using MBKC.DAL.RedisModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.BAL.Services.Implementations
{
    public class KitchenCenterService : IKitchenCenterService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public KitchenCenterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateKitchenCenterAsync(CreateKitchenCenterRequest newKitchenCenter, Email emailOption, FireBaseImage firebaseImageOption)
        {
            string logoId = "";
            string folderName = "KitchenCenters";
            bool isUploaded = false;
            try
            {
                Account existedAccount = await this._unitOfWork.AccountRepository.GetActiveAccountAsync(newKitchenCenter.ManagerEmail);
                if (existedAccount != null)
                {
                    throw new BadRequestException("Manager Email already existed in the system.");
                }
                Role role = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.KITCHEN_CENTER_MANAGER);
                Account managerAccount = new Account()
                {
                    Email = newKitchenCenter.ManagerEmail,
                    Password = "12345678",
                    Status = (int)AccountEnum.Status.ACTIVE,
                    Role = role
                };
                Wallet wallet = new Wallet()
                {
                    Balance = 0
                };
                Guid guid = Guid.NewGuid();
                logoId = guid.ToString();
                FileUtil.SetCredentials(firebaseImageOption);
                FileStream logoFileStream = FileUtil.ConvertFormFileToStream(newKitchenCenter.Logo);
                string logoLink = await FileUtil.UploadImageAsync(logoFileStream, folderName, logoId);
                if (logoLink != null && logoLink.Length > 0)
                {
                    isUploaded = true;
                } 
                logoLink += $"&logoId={logoId}";
                KitchenCenter kitchenCenter = new KitchenCenter()
                {
                    Name = newKitchenCenter.Name,
                    Address = newKitchenCenter.Address,
                    Manager = managerAccount,
                    Status = (int)KitchenCenterEnum.Status.ACTIVE,
                    Wallet = wallet,
                    Logo = logoLink
                };
                await this._unitOfWork.KitchenCenterRepository.CreateKitchenCenterAsync(kitchenCenter);
                await this._unitOfWork.CommitAsync();
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Manager Email", ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    FileUtil.SetCredentials(firebaseImageOption);
                    await FileUtil.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetKitchenCenterResponse> GetKitchenCenterAsync(int kitchenCenterId)
        {
            try
            {
                if (kitchenCenterId <= 0)
                {
                    throw new BadRequestException("Kitchen Center Id is not suitable id in the system.");
                }
                KitchenCenter kitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (kitchenCenter == null)
                {
                    throw new NotFoundException("Kitchen Center Id does not exist in the system.");
                }
                GetKitchenCenterResponse getKitchenCenterResponse = this._mapper.Map<GetKitchenCenterResponse>(kitchenCenter);
                if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.INACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.INACTIVE.ToString()[0]) + KitchenCenterEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                }
                else if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.ACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.ACTIVE.ToString()[0]) + KitchenCenterEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                }
                else if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.DEACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.DEACTIVE.ToString()[0]) + KitchenCenterEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
                }
                return getKitchenCenterResponse;
            }
            catch (BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new BadRequestException(error);
            }
            catch (NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new NotFoundException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetKitchenCentersResponse> GetKitchenCentersAsync(int? itemsPerPage, int? currentPage, string? searchValue)
        {
            try
            {
                if (itemsPerPage != null && itemsPerPage <= 0)
                {
                    throw new BadRequestException("Items per page number is required more than 0.");
                } else if(itemsPerPage == null)
                {
                    itemsPerPage = 5;
                }
                if (currentPage != null && currentPage <= 0)
                {
                    throw new BadRequestException("Current page number is required more than 0.");
                } else if(currentPage == null)
                {
                    currentPage = 1;
                }
                int numberItems = 0;
                List<KitchenCenter> kitchenCenters = null;
                if (searchValue != null && StringUtil.IsUnicode(searchValue))
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(searchValue, null);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(searchValue, null, numberItems, itemsPerPage.Value, currentPage.Value);
                } else if (searchValue != null && StringUtil.IsUnicode(searchValue) == false)
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(null, searchValue);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(null, searchValue, numberItems, itemsPerPage.Value, currentPage.Value);
                } else if(searchValue == null)
                {
                    numberItems = await this._unitOfWork.KitchenCenterRepository.GetNumberKitchenCentersAsync(null, null);
                    kitchenCenters = await this._unitOfWork.KitchenCenterRepository.GetKitchenCentersAsync(null, null, numberItems, itemsPerPage.Value, currentPage.Value);
                }
                if (kitchenCenters == null || kitchenCenters != null && kitchenCenters.Count() == 0)
                {
                    List<GetKitchenCenterResponse> kitchenCenterResponses = this._mapper.Map<List<GetKitchenCenterResponse>>(kitchenCenters);
                    GetKitchenCentersResponse getKitchenCentersResponse = new GetKitchenCentersResponse()
                    {
                        NumberItems = 0,
                        TotalPage = 0,
                        KitchenCenters = kitchenCenterResponses
                    };
                    return getKitchenCentersResponse;
                }
                int totalPage = (int)((numberItems + itemsPerPage) / itemsPerPage);
                List<GetKitchenCenterResponse> getKitchenCenterResponses = this._mapper.Map<List<GetKitchenCenterResponse>>(kitchenCenters);
                foreach (var kitchenCenter in getKitchenCenterResponses)
                {
                    if (kitchenCenter.Status.Equals(((int)KitchenCenterEnum.Status.INACTIVE).ToString()))
                    {
                        kitchenCenter.Status = char.ToUpper(KitchenCenterEnum.Status.INACTIVE.ToString()[0]) + KitchenCenterEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                    }
                    else if (kitchenCenter.Status.Equals(((int)KitchenCenterEnum.Status.ACTIVE).ToString()))
                    {
                        kitchenCenter.Status = char.ToUpper(KitchenCenterEnum.Status.ACTIVE.ToString()[0]) + KitchenCenterEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                    }
                    else if (kitchenCenter.Status.Equals(((int)KitchenCenterEnum.Status.DEACTIVE).ToString()))
                    {
                        kitchenCenter.Status = char.ToUpper(KitchenCenterEnum.Status.DEACTIVE.ToString()[0]) + KitchenCenterEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
                    }
                }
                GetKitchenCentersResponse getKitchenCenters = new GetKitchenCentersResponse()
                {
                    NumberItems = numberItems,
                    TotalPage = totalPage,
                    KitchenCenters = getKitchenCenterResponses
                };
                return getKitchenCenters;
            }
            catch (BadRequestException ex)
            {
                string fieldName = "";
                if (ex.Message.Equals("Items per page number is required more than 0."))
                {
                    fieldName = "Items Per Page";
                }
                if (ex.Message.Equals("Current page number is required more than 0."))
                {
                    fieldName = "Current Page";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task UpdateKitchenCenterAsync(int kitchenCenterId, UpdateKitchenCenterRequest updatedKitchenCenter, Email emailOption, FireBaseImage firebaseImageOption)
        {
            string folderName = "KitchenCenters";
            bool isUploaded = false;
            string logoId = "";
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if (existedKitchenCenter == null)
                {
                    throw new NotFoundException("Kitchen center id does not exist in the system.");
                } else if(existedKitchenCenter.Status == (int)KitchenCenterEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException("Kitchen center was deleted before, so this kitchen center cannot update.");
                }

                if (existedKitchenCenter.Manager.Email.Equals(updatedKitchenCenter.ManagerEmail) == false)
                {
                    Account existedAccount = await this._unitOfWork.AccountRepository.GetAccountAsync(updatedKitchenCenter.ManagerEmail);
                    if(existedAccount != null)
                    {
                        throw new BadRequestException("Manager Email already existed in the system.");
                    }
                    
                    existedKitchenCenter.Manager.Status = (int)AccountEnum.Status.DEACTIVE;
                    this._unitOfWork.AccountRepository.UpdateAccount(existedKitchenCenter.Manager);
                    
                    Role kitchenCenterManagerRole = await this._unitOfWork.RoleRepository.GetRoleAsync((int)RoleEnum.Role.KITCHEN_CENTER_MANAGER);
                    Account newManagerAccount = new Account()
                    {
                        Email = updatedKitchenCenter.ManagerEmail,
                        Password = "12345678",
                        Status = (int)AccountEnum.Status.ACTIVE,
                        Role = kitchenCenterManagerRole
                    };
                    existedKitchenCenter.Manager = newManagerAccount;
                }

                if(updatedKitchenCenter.DeletedLogo != null)
                {
                    string imageId = FileUtil.GetImageIdFromUrlImage(existedKitchenCenter.Logo, "logoId");
                    FileUtil.SetCredentials(firebaseImageOption);
                    await FileUtil.DeleteImageAsync(imageId, folderName);
                    if(updatedKitchenCenter.NewLogo == null)
                    {
                        throw new BadRequestException("New logo is required when deleting the old logo.");
                    }
                }
                
                if(updatedKitchenCenter.NewLogo != null)
                {
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileUtil.SetCredentials(firebaseImageOption);
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(updatedKitchenCenter.NewLogo);
                    string logoLink = await FileUtil.UploadImageAsync(logoFileStream, folderName, logoId);
                    if (logoLink != null && logoLink.Length > 0)
                    {
                        isUploaded = true;
                    }
                    logoLink += $"&logoId={logoId}";
                    existedKitchenCenter.Logo = logoLink;
                }
                existedKitchenCenter.Name = updatedKitchenCenter.Name;
                existedKitchenCenter.Address = updatedKitchenCenter.Address;
                if (updatedKitchenCenter.Status.ToLower().Equals(KitchenCenterEnum.Status.ACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.ACTIVE;
                } else if (updatedKitchenCenter.Status.ToLower().Equals(KitchenCenterEnum.Status.INACTIVE.ToString().ToLower()))
                {
                    existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.INACTIVE;
                }
                this._unitOfWork.KitchenCenterRepository.UpdateKitchenCenter(existedKitchenCenter);
                await this._unitOfWork.CommitAsync();
            }
            catch(NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new NotFoundException(error);
            }
            catch(BadRequestException ex)
            {
                string fieldName = "";
                if(ex.Message.Equals("Manager Email already existed in the system."))
                {
                    fieldName = "Manager Email";
                } else if (ex.Message.Equals("New logo is required when deleting the old logo."))
                {
                    fieldName = "New Logo";
                } else if (ex.Message.Equals("Kitchen center was deleted before, so this kitchen center cannot update."))
                {
                    fieldName = "Kitchen center";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch (Exception ex)
            {
                if (isUploaded)
                {
                    FileUtil.SetCredentials(firebaseImageOption);
                    await FileUtil.DeleteImageAsync(logoId, folderName);
                }
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task DeleteKitchenCenterAsync(int kitchenCenterId)
        {
            try
            {
                KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterRepository.GetKitchenCenterAsync(kitchenCenterId);
                if(existedKitchenCenter == null)
                {
                    throw new NotFoundException("Kitchen center id does not exist in the system.");
                } else if(existedKitchenCenter.Status == (int)KitchenCenterEnum.Status.DEACTIVE)
                {
                    throw new BadRequestException("Kitchen center cannot delete because that was deleted before.");
                }
                if(existedKitchenCenter.Stores != null && existedKitchenCenter.Stores.Count() > 0 && existedKitchenCenter.Stores.Any(x => x.Status == (int)StoreEnum.Status.ACTIVE) == false)
                {
                    throw new BadRequestException("The kitchen center has active stores, so this kitchen center cannot be deleted.");
                }
                existedKitchenCenter.Status = (int)KitchenCenterEnum.Status.DEACTIVE;
                this._unitOfWork.KitchenCenterRepository.UpdateKitchenCenter(existedKitchenCenter);
                await this._unitOfWork.CommitAsync();
            }
            catch(BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center", ex.Message);
                throw new BadRequestException(error);
            }
            catch(NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new BadRequestException(error);
            }
            catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }
    }
}
