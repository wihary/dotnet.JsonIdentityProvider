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
        /// <summary>The identity user root path.</summary>
        private const string IdentityUserRootPath = "Identity:userRootPath";

        /// <summary>The identity claims root path.</summary>
        private const string IdentityClaimsRootPath = "Identity:claimsRootPath";

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

            this.InitializeService();
        }

        /// <summary>
        ///
        /// </summary>
        public void InitializeService()
        {
            // Check user storage folder
            var userDirectoryName = Path.GetDirectoryName(this.config[IdentityUserRootPath]);
            if (!Directory.Exists(userDirectoryName))
            {
                Directory.CreateDirectory(userDirectoryName);
            }

            // Check claims storage folder
            var claimsDirectoryName = Path.GetDirectoryName(this.config[IdentityClaimsRootPath]);
            if (!Directory.Exists(claimsDirectoryName))
            {
                Directory.CreateDirectory(claimsDirectoryName);
            }

            this.LoadClaims();
            this.LoadUsers();
        }

        /// <summary>Method to load claims.</summary>
        private void LoadClaims()
        {
            // Check if storage folder already contains a set of user and claims
            // If so load them otherwise, create default setup
            if (File.Exists(this.config[IdentityClaimsRootPath]))
            {
                this.LoadClaimsDbFromFile();
            }
            else
            {
                this.LoadDefaultClaimsContextAsync();
            }
        }

        /// <summary>Method to load users.</summary>
        private void LoadUsers()
        {
            // Check if storage folder already contains a set of user and claims
            // If so load them otherwise, create default setup
            if (File.Exists(this.config[IdentityUserRootPath]))
            {
                this.LoadUserDbFromFile();
            }
            else
            {
                this.LoadDefaultUsersContextAsync();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual ApiUser GetUserByName(string userName)
        {
            return this.UserContext.Where(user => user.NormalizedUserName == userName).FirstOrDefault();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual List<IdentityUserClaim<string>> GetUserClaims(string userId)
        {
            return this.UserContext.Where(usr => usr.Id == userId).FirstOrDefault()?.Claims;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IdentityUserClaim<string> GetClaimByName(string name)
        {
            var roleClaim = new IdentityUserClaim<string>();
            roleClaim.InitializeFromClaim(this.ClaimContext.FirstOrDefault(claim => claim.Type == name));
            return roleClaim;
        }

        /// <summary>Method to create user and write users in json file</summary>
        /// <param name="user">User to create</param>
        /// <returns>Returns whether or not the user was created.</returns>
        public virtual bool CreateUserAndCommitAsync(ApiUser user)
        {
            if (this.UserContext.Any(usr => usr.NormalizedUserName == user.NormalizedUserName))
            {
                return false;
            }

            this.UserContext.Add(user);
            this.CommitUsers();
            return true;
        }

        /// <summary> Method to update an user if this one aren't created. </summary>
        /// <param name="user">The user to update</param>
        /// <returns>Returns the result of identity update</returns>
        /// <Exception>Exception safe</Exception>
        public virtual IdentityResult UpdateUserAndCommit(ApiUser user)
        {
            try
            {
                if (this.UserContext.Any(usr => usr.NormalizedUserName == user.NormalizedUserName))
                {
                    this.UserContext.RemoveAll(usr => usr.NormalizedUserName == user.NormalizedUserName);
                    this.UserContext.Add(user);

                    this.CommitUsers();

                    return IdentityResult.Success;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while updating a user {ex}");
            }

            return IdentityResult.Failed();
        }

        /// <summary> Method to write user and claims to json files </summary>
        private void CommitUsers()
        {
            var jsonUser = JsonConvert.SerializeObject(this.UserContext.ToList());

            try
            {
                File.WriteAllText(this.config[IdentityUserRootPath], jsonUser);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while comitting user db changes {ex}");
            }
        }

        /// <summary>Method to write claims to json files.</summary>
        private void CommitClaimsAsync()
        {
            var jsonClaims = JsonConvert.SerializeObject(this.ClaimContext.ToList());
            try
            {
                File.WriteAllText(this.config[IdentityClaimsRootPath], jsonClaims);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while comitting claims db changes {ex}");
            }
        }

        /// <summary> Method to create default users </summary>
        private void LoadDefaultUsersContextAsync()
        {
            // create users
            var user = new ApiUser { UserName = "root", NormalizedUserName = "ROOT" };
            user.Claims.Add(this.GetClaimByName("SuperUser"));
            user.PasswordHash = "AQAAAAEAACcQAAAAEPEklpcD6/h4WXtS4mzEY76idBGQQ42lVnnKyXig8dFxMuq1/mtcp6LcqTGt4tuS+Q=="; // P@ssword1234

            this.UserContext.Add(user);

            this.CommitUsers();
        }

        /// <summary>Method to create basic claims.</summary>
        private void LoadDefaultClaimsContextAsync()
        {
            // Create basic claims
            this.ClaimContext.Add(new Claim("SuperUser", "True"));
            this.ClaimContext.Add(new Claim("IsAdmin", "True"));

            this.CommitClaimsAsync();
        }

        /// <summary> Method to load users from json file </summary>
        private void LoadUserDbFromFile()
        {
            try
            {
                var userDb = File.ReadAllText(this.config[IdentityUserRootPath]);
                this.UserContext = JsonConvert.DeserializeObject<List<ApiUser>>(userDb);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while reading from user db files {ex}");
            }
        }

        /// <summary> Method to load claims from json file </summary>
        private void LoadClaimsDbFromFile()
        {
            try
            {
                var userDb = File.ReadAllText(this.config[IdentityClaimsRootPath]);
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Converters.Add(new ClaimConverter());

                this.ClaimContext = JsonConvert.DeserializeObject<List<Claim>>(userDb, settings);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while reading from user db files {ex}");
            }
        }
    }
}