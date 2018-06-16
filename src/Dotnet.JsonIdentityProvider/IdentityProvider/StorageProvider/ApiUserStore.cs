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
            return await Task.FromResult(this.jsonStorage.UpdateUserAndCommit(user));
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
                this.logger.LogError($"Exception occured while creating user {ex}");
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedUserName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ApiUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            ApiUser result = null;

            try
            {
                result = this.jsonStorage.GetUserByName(normalizedUserName);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception occured while looking for user {ex}");
            }

            return Task.FromResult(result);
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
            return Task.FromResult(user?.Id);
        }

        public Task<string> GetUserNameAsync(ApiUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.UserName);
        }



        public async Task SetNormalizedUserNameAsync(ApiUser user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user != null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<Claim>> GetClaimsAsync(ApiUser user, CancellationToken cancellationToken)
        {
            try
            {
                var userClaims = await Task.FromResult(this.jsonStorage.GetUserClaims(user.Id));

                return userClaims != null ? userClaims.ConvertAll(claim => claim.ToClaim()) : new List<Claim>();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Exception occured while looking for user claim {ex}");
            }

            return new List<Claim>();
        }

        /// <summary>Add claims to a user as an asynchronous operation.</summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The collection of <see cref="T:System.Security.Claims.Claim" />s to add.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task AddClaimsAsync(ApiUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if(claims == null)
                return; 

            foreach (var claim in claims)
            {
                try
                {
                    user.Claims.Add(this.jsonStorage.GetClaimByName(claim.Type));
                }
                catch (Exception ex)
                {
                    this.logger.LogError($"Exception occured while adding claim to user {ex}");
                }  
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