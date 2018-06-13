namespace Dotnet.JsonIdentityProvider.IdentityProvider.Model
{
    using System.Collections.Generic;
    using System.Security.Principal;
    using Microsoft.AspNetCore.Identity;

    public class ApiUser : IdentityUser
    {
        public List<IdentityUserClaim<string>> Claims { get; set; } = new List<IdentityUserClaim<string>>();
    }
}