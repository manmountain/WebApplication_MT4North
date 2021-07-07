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
    public class CustomActivityControllerTests
    {
        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        private LoginRequest user1Credentials = new LoginRequest
        {
            Email = "user1@localhost",
            Password = "P@ssw0rd1!"
        };

        private LoginRequest user2Credentials = new LoginRequest
        {
            Email = "user2@localhost",
            Password = "P@ssw0rd1!"
        };

        private LoginRequest user4Credentials = new LoginRequest
        {
            Email = "user4@localhost",
            Password = "P@ssw0rd1!"
        };

        private ApplicationUser user1;
        private ApplicationUser user2;
        private ApplicationUser user4;

        private string authUser1;
        private string authUser2;
        private string authUser4;


        private static Project _newProject;
        private static CustomActivityInfo _newActivity1, _newActivity2, _newActivity3;

        [TestInitialize]
        public async Task SetUpAsync()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;

            // login users
            authUser1 = await loginUserAsync(user1Credentials);
            authUser2 = await loginUserAsync(user2Credentials);
            authUser4 = await loginUserAsync(user4Credentials);

            // fetch user
            user1 = await getUserAsync(authUser1);
            user2 = await getUserAsync(authUser1);
            user4 = await getUserAsync(authUser4);

            // create a project
            _newProject = await CreateProject(authUser1);

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

        private async Task<List<CustomActivityInfo>> GetActivitiesAsync(HttpStatusCode expectedHttpStatusCode, string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

            var getResponse = await _httpClient.GetAsync("api/CustomActivityInfos");

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<CustomActivityInfo>>(content);
            return result;
        }

        private async Task<CustomActivityInfo> GetActivityAsync(int id, HttpStatusCode expectedHttpStatusCode, string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

            var getResponse = await _httpClient.GetAsync("api/CustomActivityInfos/" + id);

            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);

            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomActivityInfo>(content);
            return result;
        }

        private async Task<List<CustomActivityInfo>> GetProjActivityAsync(int id, HttpStatusCode expectedHttpStatusCode, string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Get 
            var getResponse = await _httpClient.GetAsync("api/CustomActivityInfos/Project/" + id);
            Assert.AreEqual(expectedHttpStatusCode, getResponse.StatusCode);
            // Parse
            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<CustomActivityInfo>>(content);
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

        private async Task<CustomActivityInfo> CreateActivity(String name, HttpStatusCode expectedHttpStatus)
        {
            // Get a theme
            var themes = await GetThemesAsync(HttpStatusCode.OK);
            Assert.IsNotNull(themes);
            Assert.IsTrue(themes.Count > 0);
            var theme = themes[0];
            // Create a new base activity
            var activity = new CustomActivityInfo
            {
                Name = "TestCustomActivityInfo "+name,
                Description = "Activity for UnitTest CustomActivityInfosControllerTest",
                Phase = ActivityPhase.DEVELOPMENT,
                Theme = theme,
                ThemeId = theme.ThemeId
            };

            var response = await _httpClient.PostAsync("api/CustomActivityInfos/",
                new StringContent(JsonSerializer.Serialize(activity), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
            if (expectedHttpStatus == HttpStatusCode.Created)
            {
                // Get created theme from body
                var postResponseBody = await response.Content.ReadAsStringAsync();
                var new_activity = JsonSerializer.Deserialize<CustomActivityInfo>(postResponseBody);
                // Check the result
                Assert.IsNotNull(new_activity);
                Assert.AreEqual(activity.Name, new_activity.Name);
                Assert.AreEqual(activity.Description, new_activity.Description);
                // return the new activity
                return new_activity;
            }
            return null;
        }

        private async Task<CustomActivityInfo> PutActivityAsync(CustomActivityInfo activity, HttpStatusCode expectedHttpStatus)
        {
            var response = await _httpClient.PutAsync("api/CustomActivityInfos/" + activity.CustomActivityInfoId,
                new StringContent(JsonSerializer.Serialize(activity), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
            if (expectedHttpStatus == HttpStatusCode.OK)
            {
                var putResponseBody = await response.Content.ReadAsStringAsync();
                var updated_activity = JsonSerializer.Deserialize<CustomActivityInfo>(putResponseBody);
                return updated_activity;
            }
            return null;
        }

        private async void DelActivityAsync(CustomActivityInfo activity, HttpStatusCode expectedHttpStatus)
        {
            var response = await _httpClient.DeleteAsync("api/CustomActivityInfos/" + activity.CustomActivityInfoId);
            Assert.AreEqual(expectedHttpStatus, response.StatusCode);
        }

        public async Task<Project> CreateProject(string authUser)
        {
            Project project = new Project()
            {
                Name = "TestCase Nytt Project",
                Description = "Ett nytt projekt skapat av unit test CustomActivityControllerTests"
            };

            var body = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, MediaTypeNames.Application.Json);

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Call to create new project
            var postResponse = await _httpClient.PostAsync("api/Projects", body);
            // Did we create the project?
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);

            var postProjectContent = await postResponse.Content.ReadAsStringAsync();
            var newProject = JsonSerializer.Deserialize<Project>(postProjectContent);

            Assert.AreEqual(project.Name, newProject.Name);
            Assert.AreEqual(project.Description, newProject.Description);

            return newProject;
        }

        public async Task<List<Project>> GetProjects(string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Call to create new project
            var postResponse = await _httpClient.GetAsync("api/Projects");
            // Did we create the project?
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);

            var responseContent = await postResponse.Content.ReadAsStringAsync();
            var projects = JsonSerializer.Deserialize<List<Project>>(responseContent);

            return projects;
        }

        public async Task<Project> GetProject(int projectId, string authUser)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);
            // Call to create new project
            var postResponse = await _httpClient.GetAsync("api/Projects/"+projectId);
            // Did we create the project?
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);

            var responseContent = await postResponse.Content.ReadAsStringAsync();
            var project = JsonSerializer.Deserialize<Project>(responseContent);

            return project;
        }

        [TestMethod]
        public async Task AA_ShouldBeAbleToCreateActivities()
        {
            // Autherized users should be able to create a custom activity
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser1);
            var newActivity1 = await CreateActivity("Custom1", HttpStatusCode.OK);
            Assert.IsNotNull(newActivity1);

            var newActivity2 = await CreateActivity("Custom2", HttpStatusCode.OK);
            Assert.IsNotNull(newActivity1);

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser2);

            var newActivity3 = await CreateActivity("Custom3", HttpStatusCode.OK);
            Assert.IsNotNull(newActivity3);

            _newActivity1 = newActivity1;
            _newActivity2 = newActivity2;
            _newActivity3 = newActivity3;
        }

        [TestMethod]
        public async Task AB_ShouldNotBeAbleToCreateActivities()
        {
            var newActivity = await CreateActivity("ShouldFail", HttpStatusCode.OK);
            Assert.IsNull(newActivity);

        }








        [TestMethod]
        public async Task AA_ShouldBeAbleToGetActivities()
        {
            // Get a project for user 1
            var projects = await GetProjects(authUser1);
            Assert.IsTrue(projects.Count > 0);
            var project = await GetProject(projects[0].ProjectId, authUser1);
            Assert.IsNotNull(project);

            // get the activites for the project
            var activitiesUser = await GetActivitiesAsync(HttpStatusCode.OK, authUser1);
            Assert.IsTrue(activitiesUser.Count > 0);
            var activitiesProj = await GetProjActivityAsync(project.ProjectId, HttpStatusCode.OK, authUser1);
            Assert.IsTrue(activitiesProj.Count > 0);
        }

        [TestMethod]
        public async Task AB_ShouldNotBeAbleToGetActivitiesAsNonMember()
        {
            // Get a project for user 1
            var projects = await GetProjects(authUser1);
            Assert.IsTrue(projects.Count > 0);
            var project = await GetProject(projects[0].ProjectId, authUser1);
            Assert.IsNotNull(project);

            // Try to get the activity for user1 project as user4. It should fail with forbidden
            // user4 is NOT a member of project 
            var activitiesProj = await GetProjActivityAsync(project.ProjectId, HttpStatusCode.Forbidden, authUser4);
            Assert.IsNull(activitiesProj);
 
        }

        [TestMethod]
        public async Task AA_ShouldBeAbleToGetActivitiesAsMember()
        {
            // Get a project for user 1
            var projects = await GetProjects(authUser1);
            Assert.IsTrue(projects.Count > 0);
            var project = await GetProject(projects[0].ProjectId, authUser1);
            Assert.IsNotNull(project);

            // Try to get the activity for user1 project as user3.
            // user2 is a member of project with atleast reading permissions
            var activitiesProj = await GetProjActivityAsync(project.ProjectId, HttpStatusCode.OK, authUser2);
            Assert.IsNull(activitiesProj);
        }





    }
}