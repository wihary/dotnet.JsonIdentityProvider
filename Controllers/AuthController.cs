namespace Dotnet.JsonIdentityProvider.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Dotnet.JsonIdentityProvider.IdentityProvider;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;
        private readonly IConfiguration config;
        private readonly UserManager<ApiUser> userManager;
        private readonly IPasswordHasher<ApiUser> passwordHash;

        public AuthController(ILogger<AuthController> logger, IConfiguration config, UserManager<ApiUser> userManager, IPasswordHasher<ApiUser> passwordCheck)
        {
            this.logger = logger;
            this.config = config;
            this.passwordHash = passwordCheck;
            this.userManager = userManager;
        }

        [HttpPost("token")]
        public async Task<ActionResult> GetTokenAsync([FromBody] CredentialModel model)
        {
            try
            {
                var user = await this.userManager.FindByNameAsync(model.UserName);
                if (user != null && this.passwordHash.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                {
                    // Now that the user has been identified, we store his claims into the JWT token to simplify authorization policy mapping
                    var userClaims = new []
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                    }.Union(await this.userManager.GetClaimsAsync(user));

                    // This two lines of code define the signing key and algorithm which being use as the token credentials
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:secretKey"]));
                    var tokenCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: config["Token:issuer"],
                        audience: config["Token:audience"],
                        claims: userClaims,
                        expires: DateTime.Now.AddMinutes(int.Parse(config["Token:duration"])),
                        signingCredentials: tokenCredentials
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expired = token.ValidTo
                    });
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error occured while generating client JWT : {ex}");
            }

            return this.BadRequest("Error while generating JWT token");
        }
    }
}