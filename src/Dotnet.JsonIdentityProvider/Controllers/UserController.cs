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
    using System.Collections.Generic;
    using System.Linq;

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

        [HttpGet]
        [Authorize(Policy = "SuperUsers")]
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllUserAsync()
        {
            var response = new List<UserModel>();
            var persistedUsers = this.userManager.Users.ToList();

            foreach (var user in persistedUsers)
            {
                response.Add(new UserModel
                {
                    Name = user.UserName,
                    Claims = user.Claims.Select(x => x.ClaimType).ToList()
                });
            }

            return Ok(response);
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
            var result = new IdentityResult();
            try
            {
                // Format model to api onject and send creation request
                var newUser = new ApiUser
                {
                    UserName = model.Name
                };
                newUser.PasswordHash = passwordHash.HashPassword(newUser, model.Password);

                result = await this.userManager.CreateAsync(newUser);
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

        [HttpPost]
        [Authorize(Policy = "SuperUsers")]
        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateUserAsync([FromBody] UserModel model)
        {
            return Ok();
        }
        
        [HttpPut("{name}")]
        [Authorize(Policy = "SuperUsers")]
        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateUserClaims([FromRoute] string username, [FromBody] List<string> userClaims)
        {
            return Ok();
        }

        [HttpDelete("{name}")]
        [Authorize(Policy = "SuperUsers")]
        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteUserAsync([FromRoute] string username)
        {
            var result = new IdentityResult();

            //Format model to api onject and send creation request
            var updatedUser = new ApiUser
            {
                UserName = username
            };

            try
            {
                // Get existing user claims using its username
                var persistedUser = await this.userManager.FindByNameAsync(updatedUser.UserName);

                // first delete existing user
                result = await this.userManager.DeleteAsync(persistedUser);

                // then if everything ok recreat it
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error while deleting user {ex}");
            }

            return BadRequest();
        }
    }
}