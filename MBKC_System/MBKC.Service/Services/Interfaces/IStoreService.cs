﻿using MBKC.Service.DTOs.Stores;
using System.Security.Claims;

namespace MBKC.Service.Services.Interfaces
{
    public interface IStoreService
    {
        public Task<GetStoresResponse> GetStoresAsync(string? searchValue, int? currentPage, int? itemsPerPage, int? brandId, int? kitchenCenterId, string? status, bool? isGetAll, IEnumerable<Claim> claims);
        public Task<GetStoreResponse> GetStoreAsync(int id, IEnumerable<Claim> claims);
        public Task CreateStoreAsync(RegisterStoreRequest registerStoreRequest, IEnumerable<Claim> claims);
        public Task UpdateStoreAsync(int storeId, UpdateStoreRequest updateStoreRequest, IEnumerable<Claim> claims);
        public Task DeleteStoreAsync(int storeId, IEnumerable<Claim> claims);
        public Task UpdateStoreStatusAsync(int storeId, UpdateStoreStatusRequest updateStoreStatusRequest, IEnumerable<Claim> claims);
        public Task ConfirmStoreRegistrationAsync(int storeId, ConfirmStoreRegistrationRequest confirmStoreRegistrationRequest);
        public Task<GetStoreResponse> GetStoreAsync(IEnumerable<Claim> claims);
    }
}
