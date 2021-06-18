using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication_MT4North.Resources;
using WebApplication_MT4North.Infrastructure;
using WebApplication_MT4North.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using WebApplication_MT4North.Models;

namespace WebApplication_MT4North.IntegrationTests
{
    [TestClass]
    public class BaseActivityControllerTests
    {
        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        private LoginRequest userCredentials = new LoginRequest
        {
            Email = "user1@localhost",
            Password = "P@ssw0rd1!"
        };

        private LoginRequest adminCredentials = new LoginRequest
        {
            Email = "user2@localhost",
            Password = "P@ssw0rd1!"
        };

        private ApplicationUser user;
        private ApplicationUser admin;

        private string authUser;
        private string authAdmin;

        private static Project _newProject;

        [TestInitialize]
        public async Task SetUpAsync()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;

            // login users
            authAdmin = await loginUserAsync(adminCredentials);
            authUser  = await loginUserAsync(userCredentials);

            // fetch user
            admin = await getUserAsync(authAdmin);
            user  = await getUserAsync(authUser);
        }

        private async Task<string> loginUserAsync(LoginRequest userCredentials)
        {
            // Login users
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(userCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            return loginResult.AccessToken;
        }

        private async Task<ApplicationUser> getUserAsync(string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Login users
            var getUserResponse = await _httpClient.GetAsync("api/account/user");

            Assert.AreEqual(HttpStatusCode.OK, getUserResponse.StatusCode);

            var userContent = await getUserResponse.Content.ReadAsStringAsync();
            var userResult = JsonSerializer.Deserialize<ApplicationUser>(userContent);
            return userResult;
        }

        private async Task<List<BaseActivityInfo>> getActivitiesAsync(HttpStatusCode expectedHttpStatusCode)
        {
            var getResponse = await _httpClient.GetAsync("api/BaseActivitiesInfos");

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<BaseActivityInfo>>(content);
            return result;
        }

        private async Task<BaseActivityInfo> getActivityAsync(int id, HttpStatusCode expectedHttpStatusCode)
        {
            
            var getResponse = await _httpClient.GetAsync("api/BaseActivitiesInfos/"+id);

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseActivityInfo>(content);
            return result;
        }

        private async Task<BaseActivityInfo> createActivity()
        {
            var theme = new Theme();
            var activity = new BaseActivityInfo
            {
                Name = "TestBastActivityInfo",
                Description = "Activity for UnitTest BaseActivitiesInfoControllerTest",
                Phase = "Test Phase",
                Theme = theme
            };

            // TODO: .....

            return activity;
        }

        [TestMethod]
        public async Task ShouldBeAbleToGetActivities()
        {
            //Both admin and user should be able to get activties
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            // Get for admin
            var activities1 = await getActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities1);
            Assert.IsTrue(activities1.Count > 0);
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Get for user
            var activities2 = await getActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities2);
            Assert.IsTrue(activities2.Count > 0);
            // Compare results
            Assert.AreEqual(activities1.Count, activities2.Count);
            CollectionAssert.AreEqual(activities1, activities2);
        }

        [TestMethod]
        public async Task ShouldBeAbleToGetSingleActivity()
        {
            //Both admin and user should be able to get activties
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            // Get for admin
            var activities1 = await getActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities1);
            Assert.IsTrue(activities1.Count > 0);
            var activity1 = await getActivityAsync(activities1[0].BaseActivityInfoId, HttpStatusCode.OK);
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Get for user
            var activities2 = await getActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities2);
            Assert.IsTrue(activities2.Count > 0);
            var activity2 = await getActivityAsync(activities2[0].BaseActivityInfoId, HttpStatusCode.OK);
            // Compare results
            Assert.AreEqual(activities1.Count, activities2.Count);
            Assert.AreEqual(activities1.Count, activities2.Count);
            Assert.AreEqual(activity1, activity2);
        }

        [TestMethod]
        public async Task ShouldBeAbleToCreateActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);

        }

        [TestMethod]
        public async Task ShouldBeNotAbleToCreateActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

        }

        [TestMethod]
        public async Task ShouldBeAbleToEditActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);

        }

        [TestMethod]
        public async Task ShouldBeNotAbleToEditActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

        }

        [TestMethod]
        public async Task ShouldBeAbleToDeleteActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);

        }

        [TestMethod]
        public async Task ShouldBeNotAbleToDeleteActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

        }
    }
}