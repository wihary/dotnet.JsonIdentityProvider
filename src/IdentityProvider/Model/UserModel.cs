namespace Dotnet.JsonIdentityProvider.IdentityProvider.Model
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class UserModel
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string[] Claims { get; set; }

        public List<Claim> GetClaimAsObjectList()
        {
            var result = new List<Claim>();
            foreach (var claim in this.Claims)
            {
                var splitedClaim = claim.Split(":");
                result.Add(new Claim(splitedClaim[0], splitedClaim[1]));
            }

            return result;
        }
    }
}