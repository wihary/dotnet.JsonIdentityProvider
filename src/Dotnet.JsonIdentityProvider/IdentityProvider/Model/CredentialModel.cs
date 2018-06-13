namespace Dotnet.JsonIdentityProvider.IdentityProvider.Model
{
    using System.ComponentModel.DataAnnotations;

    public class CredentialModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}