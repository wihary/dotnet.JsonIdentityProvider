namespace Dotnet.JsonIdentityProvider.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;

    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration config;
        private readonly UserManager<ApiUser> userManager;
        private readonly IPasswordHasher<ApiUser> passwordHash;

        public UserController(ILogger<UserController> logger, IConfiguration config, UserManager<ApiUser> userManager, IPasswordHasher<ApiUser> passwordCheck)
        {
            this.logger = logger;
            this.config = config;
            this.passwordHash = passwordCheck;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize(Policy = "SuperUsers")]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> CreateUserAsync([FromBody] UserModel model)
        {
            try
            {
                // Format model to api onject and send creation request
                var newUser = new ApiUser
                {
                    UserName = model.Name
                };
                newUser.PasswordHash = passwordHash.HashPassword(newUser, model.Password);

                var result = await this.userManager.CreateAsync(newUser);
                if (result == IdentityResult.Success)
                {
                    // Retrieve created user in order to give him the requested claims
                    var persistedUser = await this.userManager.FindByNameAsync(model.Name);
                    result = await this.userManager.AddClaimsAsync(persistedUser, model.GetClaimAsObjectList());

                    if (result == IdentityResult.Success)
                    {
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error while creating new user {ex}");
            }

            return BadRequest();
        }
    }
}