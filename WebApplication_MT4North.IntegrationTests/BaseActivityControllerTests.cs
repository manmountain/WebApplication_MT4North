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
            Email = "admin@localhost",
            Password = "P@ssw0rd1!"
        };

        private ApplicationUser user;
        private ApplicationUser admin;

        private string authUser;
        private string authAdmin;

        private static BaseActivityInfo _newActivity;

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

        private async Task<List<BaseActivityInfo>> GetActivitiesAsync(HttpStatusCode expectedHttpStatusCode)
        {
            var getResponse = await _httpClient.GetAsync("api/BaseActivityInfos");

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<BaseActivityInfo>>(content);
            return result;
        }

        private async Task<BaseActivityInfo> GetActivityAsync(int id, HttpStatusCode expectedHttpStatusCode)
        {
            
            var getResponse = await _httpClient.GetAsync("api/BaseActivityInfos/" + id);

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BaseActivityInfo>(content);
            return result;
        }

        private async Task<List<Theme>> GetThemesAsync(HttpStatusCode expectedHttpStatusCode)
        {
            var getResponse = await _httpClient.GetAsync("api/Themes");

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Theme>>(content);
            return result;
        }

        private async Task<BaseActivityInfo> CreateActivity(HttpStatusCode expectedHttpStatus)
        {
            // Get a theme
            var themes = await GetThemesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(themes);
            Assert.IsTrue(themes.Count > 0);
            var theme = themes[0];
            // Create a new base activity
            var activity = new BaseActivityInfo
            {
                Name = "TestBastActivityInfo",
                Description = "Activity for UnitTest BaseActivityInfosControllerTest",
                Phase = "Test Phase",
                Theme = theme,
                ThemeId = theme.ThemeId
            };

            var response = await _httpClient.PostAsync("api/BaseActivityInfos/",
                new StringContent(JsonSerializer.Serialize(activity), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
            if (expectedHttpStatus == HttpStatusCode.Created)
            {
                // Get created theme from body
                var postResponseBody = await response.Content.ReadAsStringAsync();
                var new_activity = JsonSerializer.Deserialize<BaseActivityInfo>(postResponseBody);
                // Check the result
                Assert.IsNotNull(new_activity);
                Assert.AreEqual(activity.Name, new_activity.Name);
                Assert.AreEqual(activity.Description, new_activity.Description);
                // return the new activity
                return new_activity;
            }
            return null;
        }

        private async Task<BaseActivityInfo> PutActivityAsync(BaseActivityInfo activity, HttpStatusCode expectedHttpStatus)
        {
            var response = await _httpClient.PutAsync("api/BaseActivityInfos/" + activity.BaseActivityInfoId,
                new StringContent(JsonSerializer.Serialize(activity), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
            if (expectedHttpStatus == HttpStatusCode.OK)
            {
                var putResponseBody = await response.Content.ReadAsStringAsync();
                var updated_activity = JsonSerializer.Deserialize<BaseActivityInfo>(putResponseBody);
                return updated_activity;
            }
            return null;
        }

        private async void DelActivityAsync(BaseActivityInfo activity, HttpStatusCode expectedHttpStatus)
        {
            var response = await _httpClient.DeleteAsync("api/BaseActivityInfos/" + activity.BaseActivityInfoId);
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
        }

        [TestMethod]
        public async Task AA_ShouldBeAbleToGetActivities()
        {
            //Both admin and user should be able to get activties
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            // Get for admin
            var activities1 = await GetActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities1);
            Assert.IsTrue(activities1.Count > 0);
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Get for user
            var activities2 = await GetActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities2);
            Assert.IsTrue(activities2.Count > 0);
            // Compare results
            Assert.AreEqual(activities1.Count, activities2.Count);
            //CollectionAssert.AreEqual(activities1, activities2);
        }

        [TestMethod]
        public async Task AB_ShouldBeAbleToGetSingleActivity()
        {
            //Both admin and user should be able to get activties
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            // Get for admin
            var activities1 = await GetActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities1);
            Assert.IsTrue(activities1.Count > 0);
            var activity1 = await GetActivityAsync(activities1[0].BaseActivityInfoId, HttpStatusCode.OK);
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Get for user
            var activities2 = await GetActivitiesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(activities2);
            Assert.IsTrue(activities2.Count > 0);
            var activity2 = await GetActivityAsync(activities2[0].BaseActivityInfoId, HttpStatusCode.OK);
            // Compare results
            Assert.AreEqual(activities1.Count, activities2.Count);
            Assert.AreEqual(activity1.BaseActivityInfoId, activity2.BaseActivityInfoId);
            Assert.AreEqual(activity1.Name, activity2.Name);
            Assert.AreEqual(activity1.Description, activity2.Description);
            Assert.AreEqual(activity1.ThemeId, activity2.ThemeId);
            Assert.IsNotNull(activity1.Theme);
            Assert.IsNotNull(activity2.Theme);
            //Assert.AreEqual(activity1, activity2);
        }

        [TestMethod]
        public async Task AC_ShouldBeAbleToCreateActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            var activity = await CreateActivity(HttpStatusCode.Created);
            Assert.IsNotNull(activity);
            _newActivity = activity;
        }

        [TestMethod]
        public async Task AD_ShouldNotBeAbleToCreateActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            var activity = await CreateActivity(HttpStatusCode.Forbidden);
            Assert.IsNull(activity);
        }

        [TestMethod]
        public async Task AE_ShouldBeAbleToEditActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            Assert.IsNotNull(_newActivity);
            // Make changes to the activity we created earlier
            _newActivity.Name = "New " + _newActivity.Name;
            _newActivity.Description = "New " + _newActivity.Description;
            // Save the changes made 
            var activity = await PutActivityAsync(_newActivity, HttpStatusCode.OK);
            Assert.AreEqual(_newActivity.ThemeId, activity.ThemeId);
            Assert.AreEqual(_newActivity.Name, activity.Name);
            Assert.AreEqual(_newActivity.Description, activity.Description);
        }

        [TestMethod]
        public async Task AF_ShouldNotBeAbleToEditActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            Assert.IsNotNull(_newActivity);
            // Make changes to the activity we created earlier
            _newActivity.Name = "Not Saved Changes " + _newActivity.Name;
            _newActivity.Description = "Not Saved Changes " + _newActivity.Description;
            // Save the changes
            var activity = await PutActivityAsync(_newActivity, HttpStatusCode.Forbidden);
            Assert.IsNull(activity);
        }

        [TestMethod]
        public async Task AG_ShouldNotBeAbleToDeleteActivity()
        {
            // Autherize User
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            Assert.IsNotNull(_newActivity);
            DelActivityAsync(_newActivity, HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task AH_ShouldBeAbleToDeleteActivity()
        {
            // Autherize Admin
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authAdmin);
            Assert.IsNotNull(_newActivity);
            DelActivityAsync(_newActivity, HttpStatusCode.OK);
        }
       
    }
}