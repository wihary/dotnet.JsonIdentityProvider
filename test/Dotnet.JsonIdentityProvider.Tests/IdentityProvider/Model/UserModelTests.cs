namespace Dotnet.JsonIdentityProvider.Tests.IdentityProvider.Model
{
    using System.Linq;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
    using NUnit.Framework;
    
    public class UserModelTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Category("UserModel Model Object")]
        public void UserModel_Instanciation()
        {
            var userModel = new UserModel();

            Assert.NotNull(userModel.Claims);
            Assert.IsEmpty(userModel.Name);
            Assert.IsEmpty(userModel.Password);
        }

        [Test]
        [Category("GetClaimAsObjectList")]
        public void GetClaimAsObjectList_emptyList()
        {
            var userModel = new UserModel();
            var result = userModel.GetClaimAsObjectList();

            Assert.NotNull(result);
            Assert.Zero(result.Count);
        }

        [Test]
        [Category("GetClaimAsObjectList")]
        public void GetClaimAsObjectList_populateInvalidList()
        {
            var userModel = new UserModel();
            userModel.Claims.Add("");
            var result = userModel.GetClaimAsObjectList();

            Assert.NotNull(result);
            Assert.Zero(result.Count);
        }

        [Test]
        [Category("GetClaimAsObjectList")]
        public void GetClaimAsObjectList_populateValidList()
        {
            var userModel = new UserModel();
            userModel.Claims.Add("type:value");
            var result = userModel.GetClaimAsObjectList();

            Assert.NotNull(result);
            Assert.AreEqual(result.First().Type, "type");
            Assert.AreEqual(result.First().Value, "value");
        }
    }
}