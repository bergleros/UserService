using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using UserService.Models;
using Microsoft.AspNetCore.DataProtection;
using System.Net;

namespace UserService.Controllers.Tests
{
    [TestClass]
    public class UserControllerTests : WebApplicationFactory<Program>
    {
        // This test sends requests to an in-memory instance of the application and tests the execution all the way through
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // This method override gives an entry point to configure the application before it starts up, 
            // typically this is where I would e.g. replace any external dependencies with mocks
            base.ConfigureWebHost(builder);
        }

        private HttpClient _client;

        [TestInitialize]
        public void Init()
        {
            _client = CreateClient();
        }

        [TestMethod]
        public async Task GetTest_WhenValidSecret_ReturnsUserWithUserId()
        {
            string secret = "abc";
            HttpRequestMessage request = new(HttpMethod.Get, $"user.api/v1/user?secret={secret}");
            HttpResponseMessage response = await _client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            UserModel user = JsonConvert.DeserializeObject<UserModel>(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(secret, user.UserSecret);
            Assert.IsTrue(user.UserId > 0);
        }

        [TestMethod]
        public async Task GetTest_WhenSecretTooShort_ReturnsBadRequestWithInfo()
        {
            string secret = "ab";
            HttpRequestMessage request = new(HttpMethod.Get, $"user.api/v1/user?secret={secret}");
            HttpResponseMessage response = await _client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(content.Contains("minimum length"));
        }

        [TestMethod]
        public async Task GetTest_WhenSecretTooLong_ReturnsBadRequestWithInfo()
        {
            string secret = Guid.NewGuid().ToString();
            HttpRequestMessage request = new(HttpMethod.Get, $"user.api/v1/user?secret={secret}");
            HttpResponseMessage response = await _client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(content.Contains("maximum length"));
        }
    }
}