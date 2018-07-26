namespace Dotnet.JsonIdentityProvider.Tests.IdentityProvider.StorageProvider
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Dotnet.JsonIdentityProvider.IdentityProvider;
    using Dotnet.JsonIdentityProvider.IdentityProvider.Model;
    using Dotnet.JsonIdentityProvider.IdentityProvider.StorageProvider;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;

    public class ApiUserStoreTests
    {
        private Mock<ILogger<ApiUserStore>> logApiUserStore;
        private Mock<ILogger<JsonUserStore>> logJsonUserStore;
        private Mock<IConfiguration> config;

        [SetUp]
        public void Setup()
        {
            this.logApiUserStore = new Mock<ILogger<ApiUserStore>>();
            this.logJsonUserStore = new Mock<ILogger<JsonUserStore>>();
            this.config = new Mock<IConfiguration>();
        }

        [Test]
        [Category("UpdateAsync")]
        public async Task UpdateAsync_nominal()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.UpdateUserAndCommit(null)).Returns(IdentityResult.Success);

            var apiUserStore = new ApiUserStore(null, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.UpdateAsync(null, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.True(result.Succeeded);
        }

        [Test]
        [Category("UpdateAsync")]
        public async Task UpdateAsync_failure()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.UpdateUserAndCommit(null)).Returns(IdentityResult.Failed());

            var apiUserStore = new ApiUserStore(null, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.UpdateAsync(null, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.False(result.Succeeded);
        }

        [Test]
        [Category("CreateAsync")]
        public async Task CreateAsync_nominalCase()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);

            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.CreateAsync(null, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.True(result.Succeeded);
        }

        [Test]
        [Category("CreateAsync")]
        public async Task CreateAsync_Exception()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.CreateUserAndCommitAsync(null)).Throws<Exception>();

            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, this.config.Object, jsonUserStoreMoq.Object);
            var result = await apiUserStore.CreateAsync(null, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.False(result.Succeeded);
        }

        [Test]
        [Category("DeleteAsync")]
        public void DeleteAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.DeleteAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("FindByIdAsync")]
        public void FindByIdAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.FindByIdAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("FindByNameAsync")]
        public async Task FindByNameAsync()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserByName(string.Empty)).Returns(new ApiUser { NormalizedUserName = "TEST" });

            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, this.config.Object, jsonUserStoreMoq.Object);
            var result = await apiUserStore.FindByNameAsync(string.Empty, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.AreEqual(result.NormalizedUserName, "TEST");
        }

        [Test]
        [Category("FindByNameAsync")]
        public async Task FindByNameAsync_returnNull()
        {
            ApiUser response = null;
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserByName(string.Empty)).Returns(response);

            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, this.config.Object, jsonUserStoreMoq.Object);
            var result = await apiUserStore.FindByNameAsync(string.Empty, new CancellationToken());

            Assert.IsNull(result);
        }

        [Test]
        [Category("FindByNameAsync")]
        public async Task FindByNameAsync_exception()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserByName(string.Empty)).Throws<Exception>();

            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, this.config.Object, jsonUserStoreMoq.Object);
            var result = await apiUserStore.FindByNameAsync(string.Empty, new CancellationToken());

            Assert.IsNull(result);
        }

        [Test]
        [Category("HasPasswordAsync")]
        public void HasPasswordAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.HasPasswordAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("GetNormalizedUserNameAsync")]
        public void GetNormalizedUserNameAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.GetNormalizedUserNameAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("GetPasswordHashAsync")]
        public void GetPasswordHashAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.GetPasswordHashAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("GetUserIdAsync")]
        public async Task GetUserIdAsync_nominal()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            var result = await apiUserStore.GetUserIdAsync(new ApiUser { Id = "userTestId" }, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "userTestId");
        }

        [Test]
        [Category("GetUserIdAsync")]
        public async Task GetUserIdAsync_userNull()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            var result = await apiUserStore.GetUserIdAsync(null, new CancellationToken());

            Assert.IsNull(result);
        }

        [Test]
        [Category("GetUserNameAsync")]
        public async Task GetUserNameAsync_nominal()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            var result = await apiUserStore.GetUserNameAsync(new ApiUser { UserName = "userTestName" }, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "userTestName");
        }

        [Test]
        [Category("GetUserNameAsync")]
        public async Task GetUserNameAsync_userNull()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            var result = await apiUserStore.GetUserNameAsync(null, new CancellationToken());

            Assert.IsNull(result);
        }

        [Test]
        [Category("SetNormalizedUserNameAsync")]
        public async Task SetNormalizedUserNameAsync()
        {
            var apiUserStore = new ApiUserStore(null, null, null);

            await apiUserStore.SetNormalizedUserNameAsync(new ApiUser { UserName = "userTestName" }, string.Empty, new CancellationToken());

            Assert.Pass();
        }

        [Test]
        [Category("SetNormalizedUserNameAsync")]
        public async Task SetNormalizedUserNameAsync_userNull()
        {
            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, null);

            await apiUserStore.SetNormalizedUserNameAsync(null, string.Empty, new CancellationToken());

            Assert.Pass();
        }

        [Test]
        [Category("SetPasswordHashAsync")]
        public void SetPasswordHashAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.SetPasswordHashAsync(null, string.Empty, new CancellationToken()));
        }

        [Test]
        [Category("SetUserNameAsync")]
        public void SetUserNameAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.SetUserNameAsync(null, string.Empty, new CancellationToken()));
        }

        [Test]
        [Category("GetClaimsAsync")]
        public async Task GetClaimsAsync_nominal()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserClaims("testUserId")).Returns(new List<IdentityUserClaim<string>>
                {
                    new IdentityUserClaim<string> { ClaimType = "Admin", ClaimValue = "true" }
                });


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.GetClaimsAsync(new ApiUser { Id = "testUserId" }, new CancellationToken());

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        [Category("GetClaimsAsync")]
        public async Task GetClaimsAsync_returnNull()
        {
            List<IdentityUserClaim<string>> response = null;
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserClaims("testUserId")).Returns(response);


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.GetClaimsAsync(new ApiUser { Id = "testUserId" }, new CancellationToken());

            Assert.IsNotNull(result);
        }

        [Test]
        [Category("GetClaimsAsync")]
        public async Task GetClaimsAsync_exception()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetUserClaims("testUserId")).Throws<Exception>();


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var result = await apiUserStore.GetClaimsAsync(new ApiUser { Id = "testUserId" }, new CancellationToken());

            Assert.IsNotNull(result);
        }

        [Test]
        [Category("AddClaimsAsync")]
        public async Task AddClaimsAsync_nominal()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetClaimByName("test", "True")).Returns(new IdentityUserClaim<string>());


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var user = new ApiUser();
            var claims = new List<Claim>() { new Claim("test", "tst1"), new Claim("test", "tst2") };
            await apiUserStore.AddClaimsAsync(user, claims, new CancellationToken());

            Assert.IsNotNull(user.Claims);
            Assert.AreEqual(user.Claims.Count, 2);
        }

        [Test]
        [Category("AddClaimsAsync")]
        public async Task AddClaimsAsync_Exception()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetClaimByName("test", "True")).Throws<Exception>();


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var user = new ApiUser();
            var claims = new List<Claim>() { new Claim("test", "tst1"), new Claim("test", "tst2") };
            await apiUserStore.AddClaimsAsync(user, claims, new CancellationToken());

            Assert.IsNotNull(user.Claims);
            Assert.AreEqual(user.Claims.Count, 0);
        }

        [Test]
        [Category("AddClaimsAsync")]
        public async Task AddClaimsAsync_ClaimListNull()
        {
            var jsonUserStoreMoq = new Mock<JsonUserStore>(this.logJsonUserStore.Object, this.config.Object);
            jsonUserStoreMoq.Setup(method => method.GetClaimByName("test", "True")).Throws<Exception>();


            var apiUserStore = new ApiUserStore(this.logApiUserStore.Object, null, jsonUserStoreMoq.Object);
            var user = new ApiUser();
            await apiUserStore.AddClaimsAsync(user, null, new CancellationToken());

            Assert.IsNotNull(user.Claims);
            Assert.AreEqual(user.Claims.Count, 0);
        }

        [Test]
        [Category("ReplaceClaimAsync")]
        public void ReplaceClaimAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.ReplaceClaimAsync(null, null, null, new CancellationToken()));
        }

        [Test]
        [Category("RemoveClaimsAsync")]
        public void RemoveClaimsAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.RemoveClaimsAsync(null, null, new CancellationToken()));
        }

        [Test]
        [Category("GetUsersForClaimAsync")]
        public void GetUsersForClaimAsync_NotImplemented()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            Assert.Throws<NotImplementedException>(() => apiUserStore.GetUsersForClaimAsync(null, new CancellationToken()));
        }

        [Test]
        [Category("Dispose")]
        public void Dispose()
        {
            var apiUserStore = new ApiUserStore(null, null, null);
            apiUserStore.Dispose();

            Assert.Pass();
        }
    }
}