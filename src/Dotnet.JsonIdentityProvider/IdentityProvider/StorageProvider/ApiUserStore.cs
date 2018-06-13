namespace Dotnet.JsonIdentityProvider.IdentityProvider.StorageProvider
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;

    public class ApiUserStore : IUserStore<ApiUser>, IUserPasswordStore<ApiUser>, IUserClaimStore<ApiUser>
    {
        private readonly JsonUserStore jsonStorage;
        private readonly ILogger<ApiUserStore> logger;
        private readonly IConfiguration config;

        public ApiUserStore(ILogger<ApiUserStore> logger, IConfiguration config, JsonUserStore jsonStorage)
        {
            this.jsonStorage = jsonStorage;
            this.logger = logger;
            this.config = config;
        }


        public async Task<IdentityResult> UpdateAsync(ApiUser user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(this.jsonStorage.UpdateUserAndCommitAsync(user));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAsync(ApiUser user, CancellationToken cancellationToken)
        {
            try
            {
                await Task.FromResult(this.jsonStorage.CreateUserAndCommitAsync(user));
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create user {ex}");
            }
            return IdentityResult.Failed();
        }

        public Task<IdentityResult> DeleteAsync(ApiUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }



        public Task<ApiUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.jsonStorage.GetUserByName(normalizedUserName));
        }
        public Task<bool> HasPasswordAsync(ApiUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


        public Task<string> GetNormalizedUserNameAsync(ApiUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(ApiUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(ApiUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApiUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }



        public async Task SetNormalizedUserNameAsync(ApiUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            await Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(ApiUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetUserNameAsync(ApiUser user, string userName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


        public async Task<IList<Claim>> GetClaimsAsync(ApiUser user, CancellationToken cancellationToken)
        {
            var userClaims = await Task.FromResult(this.jsonStorage.GetUserClaims(user.Id));
            return userClaims.ConvertAll(claim => claim.ToClaim());
        }

        public async Task AddClaimsAsync(ApiUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                user.Claims.Add(this.jsonStorage.GetClaimByName("SuperUser"));
            }

            await Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(ApiUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveClaimsAsync(ApiUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<ApiUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}