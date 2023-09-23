using AutoMapper;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Exceptions;
using MBKC.BAL.Repositories.Interfaces;
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

namespace MBKC.BAL.Repositories.Implementations
{
    public class KitchenCenterRepository : IKitchenCenterRepository
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        public KitchenCenterRepository(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
        }

        public async Task CreateKitchenCenterAsync(CreateKitchenCenterRequest newKitchenCenter, Email emailOption, FireBaseImage firebaseImageOption)
        {
            string logoId = "";
            string folderName = "KitchenCenters";
            bool IsUploaded = false;
            try
            {
                AccountRedisModel accountRedisModel = await this._unitOfWork.AccountRedisDAO.GetAccountWithEmailAsync(newKitchenCenter.ManagerEmail);
                if(accountRedisModel == null)
                {
                    Account existedAccount = await this._unitOfWork.AccountDAO.GetActiveAccountAsync(newKitchenCenter.ManagerEmail);
                    if(existedAccount != null)
                    {
                        accountRedisModel = this._mapper.Map<AccountRedisModel>(existedAccount);
                        await this._unitOfWork.AccountRedisDAO.AddAccountAsync(accountRedisModel);
                        throw new BadRequestException("Manager Email already existed in the system.");
                    }
                    Role role = await this._unitOfWork.RoleDAO.GetRoleAsync((int)RoleEnum.Role.KITCHEN_CENTER_MANAGER);
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
                    KitchenCenter kitchenCenter = new KitchenCenter()
                    {
                        Name = newKitchenCenter.Name,
                        Address = newKitchenCenter.Address,
                        Manager = managerAccount,
                        Status = (int)KitchenCenterEnum.Status.ACTIVE,
                        Wallet = wallet
                    };
                    Guid guid = Guid.NewGuid();
                    logoId = guid.ToString();
                    FileUtil.SetCredentials(firebaseImageOption);
                    FileStream logoFileStream = FileUtil.ConvertFormFileToStream(newKitchenCenter.Logo);
                    string logoLink = await FileUtil.UploadImageAsync(logoFileStream, folderName, logoId);
                    if(logoLink != null && logoLink.Length > 0)
                    {
                        IsUploaded = true;
                    }
                    kitchenCenter.Logo = logoLink + $"&logoId={logoId}";
                    await this._unitOfWork.KitchenCenterDAO.CreateKitchenCenterAsync(kitchenCenter);
                    await this._unitOfWork.CommitAsync();
                    KitchenCenterRedisModel kitchenCenterRedisModel = this._mapper.Map<KitchenCenterRedisModel>(kitchenCenter);
                    await this._unitOfWork.KitchenCenterRedisDAO.CreateKitchenCenterAsync(kitchenCenterRedisModel);
                } else
                {
                    throw new BadRequestException("Manager Email already existed in the system.");
                }
            } catch(BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Manager Email", ex.Message);
                throw new BadRequestException(error); 
            }
            catch (Exception ex)
            {
                if (IsUploaded)
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
                if(kitchenCenterId <= 0)
                {
                    throw new BadRequestException("Kitchen Center Id is not suitable id in the system.");
                }
                KitchenCenterRedisModel kitchenCenterRedisModel = await this._unitOfWork.KitchenCenterRedisDAO.GetKitchenCenterAsync(kitchenCenterId.ToString());
                if(kitchenCenterRedisModel == null)
                {
                    KitchenCenter kitchenCenter = await this._unitOfWork.KitchenCenterDAO.GetKitchenCenterAsync(kitchenCenterId);
                    if(kitchenCenter == null)
                    {
                        throw new NotFoundException("Kitchen Center Id does not exist in the system.");
                    }
                    kitchenCenterRedisModel = this._mapper.Map<KitchenCenterRedisModel>(kitchenCenter);
                    await this._unitOfWork.KitchenCenterRedisDAO.CreateKitchenCenterAsync(kitchenCenterRedisModel);
                }
                AccountRedisModel kitchenCenterManagerRedisModel = await this._unitOfWork.AccountRedisDAO.GetAccountAsync(kitchenCenterRedisModel.ManagerId.ToString());
                if(kitchenCenterManagerRedisModel == null)
                {
                    Account kitchenCenterManagerAccount = await this._unitOfWork.AccountDAO.GetAccountAsync(kitchenCenterRedisModel.ManagerId);
                    kitchenCenterManagerRedisModel = this._mapper.Map<AccountRedisModel>(kitchenCenterManagerAccount);
                    await this._unitOfWork.AccountRedisDAO.AddAccountAsync(kitchenCenterManagerRedisModel);
                }
                GetKitchenCenterResponse getKitchenCenterResponse = this._mapper.Map<GetKitchenCenterResponse>(kitchenCenterRedisModel);
                getKitchenCenterResponse.KitchenCenterManagerEmail = kitchenCenterManagerRedisModel.Email;
                if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.INACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.INACTIVE.ToString()[0]) + KitchenCenterEnum.Status.INACTIVE.ToString().ToLower().Substring(1);
                } else if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.ACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.ACTIVE.ToString()[0]) + KitchenCenterEnum.Status.ACTIVE.ToString().ToLower().Substring(1);
                } else if (getKitchenCenterResponse.Status.Equals(((int)KitchenCenterEnum.Status.DEACTIVE).ToString()))
                {
                    getKitchenCenterResponse.Status = char.ToUpper(KitchenCenterEnum.Status.DEACTIVE.ToString()[0]) + KitchenCenterEnum.Status.DEACTIVE.ToString().ToLower().Substring(1);
                }
                return getKitchenCenterResponse;
            }
            catch(BadRequestException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new BadRequestException(error);
            }
            catch(NotFoundException ex)
            {
                string error = ErrorUtil.GetErrorString("Kitchen Center Id", ex.Message);
                throw new NotFoundException(error);
            }
            catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        public async Task<GetKitchenCentersResponse> GetKitchenCentersAsync(int? itemsPerPage, int? currentPage, string? searchValue)
        {
            try
            {
                if(itemsPerPage != null && itemsPerPage <= 0)
                {
                    throw new BadRequestException("Items per page number is required more than 0.");
                }
                if(currentPage != null && currentPage <= 0)
                {
                    throw new BadRequestException("Current page number is required more than 0.");
                }
                string? searchValueWithoutUnicode = null;
                if(searchValue != null)
                {
                    searchValueWithoutUnicode = StringUtil.RemoveSign4VietnameseString(searchValue);
                }
                IList<KitchenCenterRedisModel> kitchenCenterRedisModels = await this._unitOfWork.KitchenCenterRedisDAO.GetKitchenCentersAsync(searchValue, searchValueWithoutUnicode);
                if (kitchenCenterRedisModels == null)
                {
                    List<KitchenCenter> kitchenCenters = await this._unitOfWork.KitchenCenterDAO.GetKitchenCentersAsync(searchValue, searchValueWithoutUnicode);
                    if(kitchenCenters == null)
                    {
                        GetKitchenCentersResponse getKitchenCentersResponse = new GetKitchenCentersResponse()
                        {
                            NumberItems = 0,
                            TotalPage = 0,
                            KitchenCenters = null
                        };
                        return getKitchenCentersResponse;
                    }
                    kitchenCenterRedisModels = this._mapper.Map<IList<KitchenCenterRedisModel>>(kitchenCenters);
                    foreach (var kitchenCenter in kitchenCenterRedisModels)
                    {
                        await this._unitOfWork.KitchenCenterRedisDAO.CreateKitchenCenterAsync(kitchenCenter);
                    }
                }
                if(itemsPerPage == null)
                {
                    itemsPerPage = 5;
                }
                if(currentPage == null)
                {
                    currentPage = 1;
                }
                int numberItems = kitchenCenterRedisModels.Count();
                int totalPage = (int)((numberItems + itemsPerPage) / itemsPerPage);
                kitchenCenterRedisModels = kitchenCenterRedisModels.Skip(itemsPerPage.Value * (currentPage.Value - 1)).Take(itemsPerPage.Value).ToList();
                List<GetKitchenCenterResponse> getKitchenCenterResponses = new List<GetKitchenCenterResponse>();
                foreach (var kitchenCenter in kitchenCenterRedisModels)
                {
                    AccountRedisModel kitchenCenterManagerRedisModel = await this._unitOfWork.AccountRedisDAO.GetAccountAsync(kitchenCenter.ManagerId.ToString());
                    if (kitchenCenterManagerRedisModel == null)
                    {
                        Account kitchenCenterManagerAccount = await this._unitOfWork.AccountDAO.GetAccountAsync(kitchenCenter.ManagerId);
                        kitchenCenterManagerRedisModel = this._mapper.Map<AccountRedisModel>(kitchenCenterManagerAccount);
                        await this._unitOfWork.AccountRedisDAO.AddAccountAsync(kitchenCenterManagerRedisModel);
                    }
                    GetKitchenCenterResponse getKitchenCenterResponse = this._mapper.Map<GetKitchenCenterResponse>(kitchenCenter);
                    getKitchenCenterResponse.KitchenCenterManagerEmail = kitchenCenterManagerRedisModel.Email;
                    getKitchenCenterResponses.Add(getKitchenCenterResponse);
                }
                GetKitchenCentersResponse getKitchenCenters = new GetKitchenCentersResponse()
                {
                    NumberItems = numberItems,
                    TotalPage = totalPage,
                    KitchenCenters = getKitchenCenterResponses
                };
                return getKitchenCenters;
            } 
            catch(BadRequestException ex)
            {
                string fieldName = "";
                if(ex.Message.Equals("Items per page number is required more than 0."))
                {
                    fieldName = "Items Per Page";
                }
                if(ex.Message.Equals("Current page number is required more than 0."))
                {
                    fieldName = "Current Page";
                }
                string error = ErrorUtil.GetErrorString(fieldName, ex.Message);
                throw new BadRequestException(error);
            }
            catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }

        /*public async Task UpdateKitchenCenterAsync(int kitchenCenterId, UpdateKitchenCenterRequest kitchenCenter, Email emailOption, FireBaseImage firebaseImageOption)
        {
            try
            {
                KitchenCenterRedisModel kitchenCenterRedisModel = await this._unitOfWork.KitchenCenterRedisDAO.GetKitchenCenterAsync(kitchenCenterId.ToString());
                if(kitchenCenterRedisModel == null)
                {
                    KitchenCenter existedKitchenCenter = await this._unitOfWork.KitchenCenterDAO.GetKitchenCenterAsync(kitchenCenterId);
                    if(existedKitchenCenter == null)
                    {
                        throw new NotFoundException("Kitchen center id does not exist in the system.");
                    }
                    kitchenCenterRedisModel = this._mapper.Map<KitchenCenterRedisModel>(existedKitchenCenter);
                    await this._unitOfWork.KitchenCenterRedisDAO.CreateKitchenCenterAsync(kitchenCenterRedisModel);
                }
                AccountRedisModel managerAccountRedisModel = await this._unitOfWork.AccountRedisDAO.GetAccountAsync(kitchenCenterRedisModel.ManagerId.ToString());
                if(managerAccountRedisModel == null)
                {
                    Account managerAccount = await this._unitOfWork.AccountDAO.GetAccountAsync(kitchenCenterRedisModel.ManagerId);
                    managerAccountRedisModel = this._mapper.Map<AccountRedisModel>(managerAccount);
                    await this._unitOfWork.AccountRedisDAO.AddAccountAsync(managerAccountRedisModel);
                }

                if(managerAccountRedisModel.Email.Equals(kitchenCenter.ManagerEmail) == false)
                {
                    //xoa cu
                    // tao moi
                }

            } catch(Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                throw new Exception(error);
            }
        }*/
    }
}
