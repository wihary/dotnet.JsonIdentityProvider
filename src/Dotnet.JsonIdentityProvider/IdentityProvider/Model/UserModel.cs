namespace Dotnet.JsonIdentityProvider.IdentityProvider.Model
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class UserModel
    {
        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public List<string> Claims { get; set; } = new List<string>();

        public List<Claim> GetClaimAsObjectList()
        {
            var result = new List<Claim>();
            foreach (var claim in this.Claims)
            {
                var splitedClaim = claim.Split(":");
                if(splitedClaim.Length == 2)
                    result.Add(new Claim(splitedClaim[0], splitedClaim[1]));
            }

            return result;
        }
    }
}