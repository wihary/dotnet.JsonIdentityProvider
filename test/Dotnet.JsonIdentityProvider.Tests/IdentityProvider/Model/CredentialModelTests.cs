namespace Dotnet.JsonIdentityProvider.Tests.IdentityProvider.Model
{
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
    using NUnit.Framework;

    public class CredentialModelTests
    {
        [Test]
        [Category("CredentialModel Model Object")]
        public void CredentialModel_Instanciation()
        {
            var credentialModel = new CredentialModel();

            Assert.IsEmpty(credentialModel.UserName);
            Assert.IsEmpty(credentialModel.Password);
        }
    }
}