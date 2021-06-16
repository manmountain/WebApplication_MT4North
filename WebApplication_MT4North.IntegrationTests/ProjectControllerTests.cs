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
    public class ProjectControllerTests
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

        private LoginRequest user3Credentials = new LoginRequest
        {
            Email = "user3@localhost",
            Password = "P@ssw0rd1!"
        };

        private LoginRequest user4Credentials = new LoginRequest
        {
            Email = "user4@localhost",
            Password = "P@ssw0rd1!"
        };

        private ApplicationUser user1;
        private ApplicationUser user2;
        private ApplicationUser user3;
        private ApplicationUser user4;

        private string authUser1;
        private string authUser2;
        private string authUser3;
        private string authUser4;

        private static Project _newProject;

        [TestInitialize]
        public async Task SetUpAsync()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;

            // login users
            authUser1 = await loginUserAsync(user1Credentials);
            authUser2 = await loginUserAsync(user2Credentials);
            authUser3 = await loginUserAsync(user3Credentials);
            authUser4 = await loginUserAsync(user4Credentials);

            // fetch user
            user1 = await getUserAsync(authUser1);
            user2 = await getUserAsync(authUser2);
            user3 = await getUserAsync(authUser3);
            user4 = await getUserAsync(authUser4);
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

        [TestMethod]
        public async Task AAA_ShouldBeAbleToCreateProject()
        {
            Project project = new Project()
            {
                Name = "TestCase Nytt Project",
                Description = "Ett nytt projekt skapat av unit test _ShouldBeAbleToCreateProject"
            };

            var body = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, MediaTypeNames.Application.Json);

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser1);
            // Call to create new project
            var postResponse = await _httpClient.PostAsync("api/Projects", body);
            // Did we create the project?
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);

            var postProjectContent = await postResponse.Content.ReadAsStringAsync();
            var newProject = JsonSerializer.Deserialize<Project>(postProjectContent);

            Assert.AreEqual(project.Name, newProject.Name);
            Assert.AreEqual(project.Description, newProject.Description);

            _newProject = newProject;
        }

        [TestMethod]
        public async Task AAB_CreatorShouldBeMemberOfNewProject()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser1);
            // Call to userProjects for the new project
            var getResponse = await _httpClient.GetAsync("/api/UserProjects/Project/" + _newProject.ProjectId);
            // Did we create the project?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getUserProjectContent = await getResponse.Content.ReadAsStringAsync();
            var userProjects = JsonSerializer.Deserialize<List<UserProject>>(getUserProjectContent);

            Assert.AreEqual(userProjects.Count, 1);

            var userproject = userProjects[0];

            Assert.AreEqual(userproject.UserId, user1.Id);
            Assert.AreEqual(userproject.ProjectId, _newProject.ProjectId);
            Assert.AreEqual(userproject.Role, "Projektägare");
            Assert.AreEqual(userproject.Rights, "RW");
            Assert.AreEqual(userproject.Status, UserProjectStatus.Accepted);
        }

        [TestMethod]
        public async Task AAC_CreatorShouldBeAbleToInvite()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser1);

            // Invite user2 with RW permissions
            await inviteUser(user2Credentials.Email, _newProject.ProjectId, "TestDeltagare", "RW", HttpStatusCode.OK);
            // Invite user3 with R  permissions
            await inviteUser(user3Credentials.Email, _newProject.ProjectId, "TestDeltagare", "R", HttpStatusCode.OK);
        }

        private async Task inviteUser(string email, int projectId, string role, string permissions, HttpStatusCode expectedHttpStatus)
        {
            // Invite user
            //var url = String.Format("/api/UserProjects/{userEmail}/{projectId}/{role}/{permissions}", email, projectId, role, permissions);

            //api/UserProjects/Invites/{userEmail}/{projectId}/{role}/{permissions}
            var url = "/api/UserProjects/"+email+"/"+projectId+"/"+role+"/"+permissions;
            var body = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json);
            // Call to create new UserProject with status pending
            var postResponse = await _httpClient.PostAsync(url, body);
            // Did we create invite user?
            Assert.AreEqual(expectedHttpStatus, postResponse.StatusCode);
        }

        [TestMethod]
        public async Task AAD_UsersShouldBeAbleToAcceptAndReject()
        {
            await acceptInvite(user2, authUser2, "TestDeltagare", "RW");
            await rejectInvite(user3, authUser3, "TestDeltagare", "R");
        }

        private async Task acceptInvite(ApplicationUser user, string authUser, string role, string permissions)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

            // Get user project-invitations
            var getUrl = "api/UserProjects/Invites";
            // Call to get the invitations (UserProjects with status pending) for the logged in user
            var getResponse = await _httpClient.GetAsync(getUrl);
            // Did we get the invites?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getUserProjectContent = await getResponse.Content.ReadAsStringAsync();
            var userProjects = JsonSerializer.Deserialize<List<UserProject>>(getUserProjectContent);

            Assert.AreEqual(userProjects.Count, 1);
            var userproject = userProjects[0];

            Assert.AreEqual(userproject.ProjectId, _newProject.ProjectId);
            Assert.AreEqual(userproject.UserId, user.Id);
            Assert.AreEqual(userproject.Role, role);
            Assert.AreEqual(userproject.Rights, permissions);
            Assert.AreEqual(userproject.Status, UserProjectStatus.Pending);

            // Accept invite
            var url = "api/UserProjects/Invites/Accept/" + userproject.UserProjectId;
            var body = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json);
            // Call to accept
            var postResponse = await _httpClient.PostAsync(url, body);
            // Did we accept?
            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);

            // Get user invites a second time
            var getResponse2 = await _httpClient.GetAsync(getUrl);
            // Did we get the invites?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getUserProjectContent2 = await getResponse2.Content.ReadAsStringAsync();
            var userProjects2 = JsonSerializer.Deserialize<List<UserProject>>(getUserProjectContent2);

            // We should not have any invites left
            Assert.AreEqual(userProjects2.Count, 0);
        }

        private async Task rejectInvite(ApplicationUser user, string authUser, string role, string permissions)
        {
            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser);

            // Get user project-invitations
            var getUrl = "api/UserProjects/Invites";
            // Call to get the invitations (UserProjects with status pending) for the logged in user
            var getResponse = await _httpClient.GetAsync(getUrl);
            // Did we get the invites?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getUserProjectContent = await getResponse.Content.ReadAsStringAsync();
            var userProjects = JsonSerializer.Deserialize<List<UserProject>>(getUserProjectContent);

            Assert.AreEqual(userProjects.Count, 1);
            var userproject = userProjects[0];

            Assert.AreEqual(userproject.ProjectId, _newProject.ProjectId);
            Assert.AreEqual(userproject.UserId, user.Id);
            Assert.AreEqual(userproject.Role, role);
            Assert.AreEqual(userproject.Rights, permissions); 
            Assert.AreEqual(userproject.Status, UserProjectStatus.Pending);

            // Reject invite
            var url = "api/UserProjects/Invites/Reject/"+userproject.UserProjectId;
            var body = new StringContent("", Encoding.UTF8, MediaTypeNames.Application.Json);
            // Call to reject
            var postResponse = await _httpClient.PostAsync(url, body);
            // Did we reject?
            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);

            // Get user invites a second time
            var getResponse2 = await _httpClient.GetAsync(getUrl);
            // Did we get the invites?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getUserProjectContent2 = await getResponse2.Content.ReadAsStringAsync();
            var userProjects2 = JsonSerializer.Deserialize<List<UserProject>>(getUserProjectContent2);

            // We should have the rejected invite
            Assert.AreEqual(userProjects2.Count, 0);
        }

        [TestMethod]
        public async Task AAE_UserShouldBeAbleToInvite()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize user2
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser2);

            // Invite user4 with R permissions
            await inviteUser(user4Credentials.Email, _newProject.ProjectId, "TestDeltagare", "R", HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task AAF_UserShouldNotBeAbleToInviteTwice()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize user2
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser2);

            // Invite user4 again (should Fail)
            await inviteUser(user4Credentials.Email, _newProject.ProjectId, "TestDeltagare", "RW", HttpStatusCode.BadRequest);

            // Accept the previus invitation for user4
            await acceptInvite(user4, authUser4, "TestDeltagare", "R");
        }

        [TestMethod]
        public async Task AAG_UserShouldNotBeAbleToInvite()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize user4
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser4);

            // Invite a user (should Fail) because missing W permission of user4
            await inviteUser(user2.Email, _newProject.ProjectId, "TestDeltagare", "R", HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public async Task AAH_ShouldBeAbleToEdit()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize user2
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser2);

            // get the project 
            var getUrl = "api/Projects/"+_newProject.ProjectId;
            // Call to get the project with id
            var getResponse = await _httpClient.GetAsync(getUrl);
            // Did we get the project?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getProjectContent = await getResponse.Content.ReadAsStringAsync();
            var project = JsonSerializer.Deserialize<Project>(getProjectContent);

            // Edit the project decription
            project.Description = "New Decription";

            // Save the change
            var putUrl = String.Format("/api/Projects/{id}", project.ProjectId);
            var body = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, MediaTypeNames.Application.Json);
            // Call to save
            var putResponse = await _httpClient.PutAsync(putUrl, body);
            // Did we succed?
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);

            // Call to get the project with id
            var getResponse2 = await _httpClient.GetAsync(getUrl);
            // Did we get the project?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getProjectContent2 = await getResponse.Content.ReadAsStringAsync();
            var project2 = JsonSerializer.Deserialize<Project>(getProjectContent2);

            Assert.AreEqual(project.ProjectId, project2.ProjectId);
            Assert.AreEqual(project.Name, project2.Name);
            Assert.AreEqual(project.Description, project2.Description);
        }

        [TestMethod]
        public async Task AAI_ShouldNotBeAbleToEdit()
        {
            // Make sure that we have a newProject
            Assert.IsNotNull(_newProject);

            // Autherize user4
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authUser4);

            // get the project 
            var getUrl = "api/Projects/" + _newProject.ProjectId;
            // Call to get the project with id
            var getResponse = await _httpClient.GetAsync(getUrl);
            // Did we get the project?
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var getProjectContent = await getResponse.Content.ReadAsStringAsync();
            var project = JsonSerializer.Deserialize<Project>(getProjectContent);

            // Edit the project decription
            project.Description = "Should not be saved";

            // Save the change
            var putUrl = String.Format("/api/Projects/{id}", project.ProjectId);
            var body = new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8, MediaTypeNames.Application.Json);
            // Call to save
            var putResponse = await _httpClient.PutAsync(putUrl, body);
            // Did we succed?
            Assert.AreEqual(HttpStatusCode.Forbidden, putResponse.StatusCode);
        }

        public async Task AAJ_ShouldNotBeAbleToDelete()
        {


        }

        public async Task AAK_ShouldBeAbleToDelete()
        {


        }
    }
}