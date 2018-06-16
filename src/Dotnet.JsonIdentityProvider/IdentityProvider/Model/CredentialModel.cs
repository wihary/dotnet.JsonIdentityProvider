namespace Dotnet.JsonIdentityProvider.IdentityProvider.Model
{
    using System.ComponentModel.DataAnnotations;

    public class CredentialModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}