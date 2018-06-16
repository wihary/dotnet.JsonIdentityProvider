namespace Dotnet.JsonIdentityProvider.Tests.IdentityProvider.Model
{
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
    using NUnit.Framework;

    public class ApiUserTests
    {
        [Test]
        [Category("ApiUser Model Object")]
        public void ApiUser_Instanciation()
        {
            var apiUser = new ApiUser();

            Assert.NotNull(apiUser.Claims);
        }
    }
}