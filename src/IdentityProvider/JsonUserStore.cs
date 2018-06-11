namespace Dotnet.JsonIdentityProvider.IdentityProvider
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Json;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;

    public class JsonUserStore
    {
        private List<ApiUser> UserContext;
        private List<Claim> ClaimContext;
        private readonly ILogger<JsonUserStore> logger;
        private readonly IConfiguration config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="passwordHasher"></param>
        public JsonUserStore(ILogger<JsonUserStore> logger, IConfiguration config)
        {
            this.logger = logger;
            this.config = config;
            this.UserContext = new List<ApiUser>();
            this.ClaimContext = new List<Claim>();

            // Check storage folder 
            if (!Directory.Exists(Path.GetDirectoryName(this.config["Identity:rootPath"])))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.config["Identity:rootPath"]));
            }

            // Check if storage folder already contains a set of user and claims
            // If so load them otherwise, create default setup
            if (File.Exists(this.config["Identity:rootPath"]))
            {
                this.LoadUserDbFromFile();
            }
            else
            {
                this.LoadDefaultContextAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ApiUser GetUserByName(string userName)
        {
            return this.UserContext.Where(user => user.NormalizedUserName == userName).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<IdentityUserClaim<string>> GetUserClaims(string userId)
        {
            return this.UserContext.Where(usr => usr.Id == userId).FirstOrDefault().Claims;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IdentityUserClaim<string> GetClaimByName(string name)
        {
            var roleClaim = new IdentityUserClaim<string>();
            roleClaim.InitializeFromClaim(this.ClaimContext.Where(claim => claim.Type == name).FirstOrDefault());

            return roleClaim;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CreateUserAndCommitAsync(ApiUser user)
        {
            if (!this.UserContext.Any(usr => usr.NormalizedUserName == user.NormalizedUserName))
            {
                this.UserContext.Add(user);

                this.CommitAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public IdentityResult UpdateUserAndCommitAsync(ApiUser user)
        {
            try
            {
                if (this.UserContext.Any(usr => usr.NormalizedUserName == user.NormalizedUserName))
                {
                    this.UserContext.RemoveAll(usr => usr.NormalizedUserName == user.NormalizedUserName);
                    this.UserContext.Add(user);

                    this.CommitAsync();

                    return IdentityResult.Success;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while updating a user {ex}");
            }


            return IdentityResult.Failed();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CommitAsync()
        {

            var jsonUser = JsonConvert.SerializeObject(this.UserContext.ToList());
            try
            {
                File.WriteAllText(this.config["Identity:rootPath"], jsonUser);
            }
            catch (System.Exception ex)
            {
                this.logger.LogError($"Error occured while comitting user db changes {ex}");
            }


            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void LoadDefaultContextAsync()
        {
            // Create basic claims
            this.ClaimContext.Add(new Claim("SuperUser", "True"));
            this.ClaimContext.Add(new Claim("IsAdmin", "True"));

            // create users
            var user = new ApiUser { UserName = "whary", NormalizedUserName = "WHARY" };
            user.Claims.Add(this.GetClaimByName("SuperUser"));
            user.PasswordHash = "AQAAAAEAACcQAAAAEGC1hTj0ArYi6rHfnn8gaGKSMds7PgCMMT6f6b+3x04xVQFcRmJkay99JoBncWEeyQ==";

            this.UserContext.Add(user);

            this.CommitAsync();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void LoadUserDbFromFile()
        {
            try
            {
                var userDb = File.ReadAllText(this.config["Identity:rootPath"]);
                this.UserContext = JsonConvert.DeserializeObject<List<ApiUser>>(userDb);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while reading from user db files {ex}");
            }
        }
    }
}