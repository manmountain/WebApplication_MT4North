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
    public class AccountControllerTests
    {
        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;
        }

        [TestMethod]
        public async Task ShouldExpect401WhenLoginWithInvalidCredentials()
        {
            var credentials = new LoginRequest
            {
                Email = "admin@localhost",
                Password = "invalidPassword"
            };
            var response = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task ShouldExpect401WhenAccesingProtectedRoutesUnauthorized()
        {
            var response = await _httpClient.GetAsync("api/account/user"); // protected route
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToRegistrateWithWeakPassword()
        {

            var credentials = new RegisterRequest
            {
                UserName = "weak@test.io",
                Email = "weak@test.io",
                Password = "weakpasswassword",
                FirstName = "Weak",
                LastName = "Password"
            };

            // 1. Try to register a user 
            var registerResponse = await _httpClient.PostAsync("api/account/register",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));

            var registerResponseBody= await registerResponse.Content.ReadAsStringAsync();
            var registerResult = JsonSerializer.Deserialize<ErrorResult>(registerResponseBody);

            // Make sure that we failed
            Assert.AreEqual(HttpStatusCode.BadRequest, registerResponse.StatusCode);
            Assert.AreEqual("Error creating user with email: weak@test.io and username: weak@test.io", registerResult.Message);
            var errors = new List<string>(new string[] { "Passwords must have at least one non alphanumeric character.", "Passwords must have at least one digit ('0'-'9').", "Passwords must have at least one uppercase ('A'-'Z')." });
            CollectionAssert.AreEqual(errors, registerResult.Errors);
        }

        [TestMethod]
        public async Task ShouldBeAbleToRegistrateUpdateAndDeleteUser()
        {
            var credentials = new RegisterRequest
            {
                UserName = "testuser@mt4north.io",
                Email = "testuser@mt4north.io",
                Password = "T3st#P4ssw0rd",
                FirstName = "Test",
                LastName = "Testson"
            };
            
            // 1. Create a User
            var registerResponse = await _httpClient.PostAsync("api/account/register",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, registerResponse.StatusCode);

            // 2. Loggin
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            //Assert.AreEqual(credentials.Email, loginResult.Email);
            var roles = new List<string>(new string[] { "BasicUser" });
            CollectionAssert.AreEqual(roles, loginResult.Roles);
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.RefreshToken));

            // 3. Change password
            var passwordUpdate = new UpdatePasswordRequest
            {
                CurrentPassword = credentials.Password,
                NewPassword = "N3w#S3cr3t#P4ssw0rd"
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var updatePasswordResponse = await _httpClient.PutAsync("api/account/user/password",
                new StringContent(JsonSerializer.Serialize(passwordUpdate), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, updatePasswordResponse.StatusCode);

            // 4. Logout and login with new credentials
            var newCredentials = new RegisterRequest
            {
                Email = credentials.Email,
                Password = passwordUpdate.NewPassword
            };

            var logoutResponse = await _httpClient.PostAsync("api/account/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var newLoginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(newCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            var newLoginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var newLoginResult = JsonSerializer.Deserialize<LoginResult>(newLoginResponseContent);
            Assert.AreEqual(HttpStatusCode.OK, newLoginResponse.StatusCode);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, newLoginResult.AccessToken);

            // 5. Update User info
            var userUpdate = new UserRequest
            {
                FirstName = "Upd",
                LastName = "Ated"
            };
            var updateUserResponse = await _httpClient.PutAsync("api/account/user",
                new StringContent(JsonSerializer.Serialize(userUpdate), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, updatePasswordResponse.StatusCode);

            var updatedUserResponseContent = await updateUserResponse.Content.ReadAsStringAsync();
            var updatedUserResult = JsonSerializer.Deserialize<ApplicationUser>(updatedUserResponseContent);

            Assert.AreEqual(updatedUserResult.FirstName, "Upd");
            Assert.AreEqual(updatedUserResult.LastName,  "Ated");

            /*logoutResponse = await _httpClient.PostAsync("api/account/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            _httpClient.DefaultRequestHeaders.Authorization = null;*/

            // 6. Delete User "api/account/user/testUser@mt4north.io" (must be admin user). TODO: Implement "api/account/user/" for deleting your own user.. 
            /*var adminCredentials = new RegisterRequest
            {
                Email = "admin@mt4north.io",
                Password = "S3cr3t#P4ssw0rd"
            };

            var adminLoginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(adminCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, adminLoginResponse.StatusCode);

            var adminloginResponseContent = await adminLoginResponse.Content.ReadAsStringAsync();
            var adminLoginResult = JsonSerializer.Deserialize<LoginResult>(adminloginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.RefreshToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, adminLoginResult.AccessToken);
            var deleteResponse = await _httpClient.DeleteAsync("api/account/user/"+ credentials.Email);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);

            logoutResponse = await _httpClient.PostAsync("api/account/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            _httpClient.DefaultRequestHeaders.Authorization = null;*/

            // 6. Delete User
            var deleteResponse = await _httpClient.DeleteAsync("api/account/user/");
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToUpdateEmailToExisting()
        {
            var adminCredentials = new RegisterRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };
            var userCredentials = new RegisterRequest
            {
                Email = "user1@localhost",
                Password = "P@ssw0rd1!"
            };
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(userCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);

            var userUpdate = new UserRequest
            {
                Email = adminCredentials.Email
            };
            var updateUserResponse = await _httpClient.PutAsync("api/account/user",
                new StringContent(JsonSerializer.Serialize(userUpdate), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.StatusCode);

            //var updatedUserResponseContent = await updateUserResponse.Content.ReadAsStringAsync();
            //var updatedUserResult = JsonSerializer.Deserialize<UserResult>(updatedUserResponseContent);
        }

        //TODO
        [TestMethod]
        public async Task ShouldBeAbleToDeleteUserAsAdmin()
        {
            var credentials = new RegisterRequest
            {
                UserName = "user4test@mt4north.io",
                Email = "user4test@mt4north.io",
                Password = "T3st#P4ssw0rd",
                FirstName = "Test",
                LastName = "User"
            };

            // 1. Create a User
            var registerResponse = await _httpClient.PostAsync("api/account/register",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, registerResponse.StatusCode);

            var adminCredentials = new RegisterRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };

            // 2. Login as admin and delete it
            var adminLoginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(adminCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, adminLoginResponse.StatusCode);

            var adminloginResponseContent = await adminLoginResponse.Content.ReadAsStringAsync();
            var adminLoginResult = JsonSerializer.Deserialize<LoginResult>(adminloginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.RefreshToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, adminLoginResult.AccessToken);
            var deleteResponse = await _httpClient.DeleteAsync("api/account/user/" + credentials.Email);
            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.StatusCode);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToDeleteNonExistingUserAsAdmin()
        {
            var adminCredentials = new RegisterRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };

            // 1. Login as admin and delete a user that does not exist
            var adminLoginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(adminCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, adminLoginResponse.StatusCode);

            var adminloginResponseContent = await adminLoginResponse.Content.ReadAsStringAsync();
            var adminLoginResult = JsonSerializer.Deserialize<LoginResult>(adminloginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(adminLoginResult.RefreshToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, adminLoginResult.AccessToken);
            var deleteResponse = await _httpClient.DeleteAsync("api/account/user/idontexist@mt4north.io");
            Assert.AreEqual(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToDeleteUserAsBasicUser()
        {
            
            var basicCredentials = new RegisterRequest
            {
                Email = "user1@localhost",
                Password = "P@ssw0rd1!"
            };

            // Login as a basic user and try to delete a user (admin user in this case)
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(basicCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));

            var responseBody = await loginResponse.Content.ReadAsStringAsync();
            var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBody);

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.RefreshToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var deleteResponse = await _httpClient.DeleteAsync("api/account/user/admin@localhost");
            Assert.AreEqual(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        //TODO
        [TestMethod]
        public async Task ShouldBeAbleToManageRolesAsAdminUser()
        {
            var adminCredentials = new RegisterRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };
            var newRoleName = "TestUser";
            // 1. Login as admin
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(adminCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.RefreshToken));

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            
            // Get all roles
            var roleResponse1 = await _httpClient.GetAsync("api/account/roles");
            Assert.AreEqual(HttpStatusCode.OK, roleResponse1.StatusCode);

            var roleResponseContent1 = await roleResponse1.Content.ReadAsStringAsync();
            var roleResult1 = JsonSerializer.Deserialize<RolesResult>(roleResponseContent1);

            // Create a new Role
            /*var newRole = new RoleRequest
            {
                RoleName = newRoleName
            };*/
            var newRoleResponse = await _httpClient.PostAsync("api/account/roles/" + newRoleName, null);
            Assert.AreEqual(HttpStatusCode.OK, newRoleResponse.StatusCode);

            var newRoleResponseContent = await newRoleResponse.Content.ReadAsStringAsync();
            var newRoleResult = JsonSerializer.Deserialize<StatusResult>(newRoleResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(newRoleResult.Message));
            Assert.AreEqual("Role " + newRoleName + " created", newRoleResult.Message);

            // Get all roles
            var roleResponse2 = await _httpClient.GetAsync("api/account/roles");
            Assert.AreEqual(HttpStatusCode.OK, roleResponse2.StatusCode);

            var roleResponseContent2 = await roleResponse2.Content.ReadAsStringAsync();
            var roleResult2 = JsonSerializer.Deserialize<RolesResult>(roleResponseContent2);

            // Compare with previous list of roles (roleResult2 == roleResult1.insert('newRoleName') ) 
            var roles0 = new List<string>(roleResult1.Roles.ToArray());
            roles0.Insert(0, newRoleName);
            roles0.Sort();
            roleResult2.Roles.Sort();
            CollectionAssert.AreEqual(roles0, roleResult2.Roles);
            
            // Delete new role
            var delRoleResponse = await _httpClient.DeleteAsync("api/account/roles/" + newRoleName);
            Assert.AreEqual(HttpStatusCode.OK, delRoleResponse.StatusCode);

            var delRoleResponseContent = await delRoleResponse.Content.ReadAsStringAsync();
            var delRoleResult = JsonSerializer.Deserialize<StatusResult>(delRoleResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(delRoleResult.Message));
            Assert.AreEqual("Role " + newRoleName + " deleted", delRoleResult.Message);

            // Get all roles
            var roleResponse3 = await _httpClient.GetAsync("api/account/roles");
            Assert.AreEqual(HttpStatusCode.OK, roleResponse3.StatusCode);

            var roleResponseContent3 = await roleResponse3.Content.ReadAsStringAsync();
            var roleResult3 = JsonSerializer.Deserialize<RolesResult>(roleResponseContent3);

            // Compare with previous two list, should be equal with the first  (roleResult3 == roleResult1)
            CollectionAssert.AreEqual(roleResult1.Roles, roleResult3.Roles);
        }

        [TestMethod]
        public async Task ShouldNotBeAbleToManageRolesAsBasicUser()
        {
            var basicCredentials = new RegisterRequest
            {
                Email = "user1@localhost",
                Password = "P@ssw0rd1!"
            };
            var newRoleName = "TestUser";
            // 1. Login as admin
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(basicCredentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.RefreshToken));

            // Autherize
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);

            // Get all roles
            var roleResponse = await _httpClient.GetAsync("api/account/roles");
            Assert.AreEqual(HttpStatusCode.OK, roleResponse.StatusCode); // Can read roles, 200 OK

            // Create a new Role
            /*var newRole = new RoleRequest
            {
                RoleName = newRoleName
            };*/
            var newRoleResponse = await _httpClient.PostAsync("api/account/roles/"+newRoleName, null);
                //new StringContent(JsonSerializer.Serialize(newRole), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.Forbidden, newRoleResponse.StatusCode); // Cant create role, 403 Forbidden

            // Delete new role
            var delRoleResponse = await _httpClient.DeleteAsync("api/account/roles/" + newRoleName);
            Assert.AreEqual(HttpStatusCode.Forbidden, delRoleResponse.StatusCode); // Cant delete role, 403 Forbidden

        }

        [TestMethod]
        public async Task ShouldReturnCorrectResponseForSuccessLogin()
        {
            var credentials = new LoginRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            //Assert.AreEqual(credentials.Email, loginResult.Email);
            //Assert.IsNull(loginResult.OriginalUserName); //REMOVEME:
            var roles = new List<string>(new string[] { /*"BasicUser",*/ "AdminUser" });
            CollectionAssert.AreEqual(roles, loginResult.Roles); 
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.RefreshToken));

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(loginResult.AccessToken);
            Assert.AreEqual(credentials.Email, principal.Identity.Name);
            var claimRoles = principal.FindAll(ClaimTypes.Role); //FindFirst(ClaimTypes.Role);
            var v = claimRoles.ElementAt(0);
            Assert.AreEqual("AdminUser", principal.FindFirst(ClaimTypes.Role).Value);
            //Assert.AreEqual("AdminUser", claimRoles.ElementAt(0).Value);
            Assert.IsNotNull(jwtSecurityToken);
        }

        [TestMethod]
        public async Task ShouldBeAbleToLogout()
        {
            var credentials = new LoginRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            Assert.IsTrue(jwtAuthManager.UsersRefreshTokensReadOnlyDictionary.ContainsKey(loginResult.RefreshToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var logoutResponse = await _httpClient.PostAsync("api/account/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            Assert.IsFalse(jwtAuthManager.UsersRefreshTokensReadOnlyDictionary.ContainsKey(loginResult.RefreshToken));
        }

        [TestMethod]
        public async Task ShouldCorrectlyRefreshToken()
        {
            const string userName = "admin@localhost";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
            var response = await _httpClient.PostAsync("api/account/refresh-token",
                new StringContent(JsonSerializer.Serialize(refreshRequest), Encoding.UTF8, MediaTypeNames.Application.Json));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResult>(responseContent);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var refreshToken2 = jwtAuthManager.UsersRefreshTokensReadOnlyDictionary.GetValueOrDefault(result.RefreshToken);
            Assert.AreEqual(refreshToken2.TokenString, result.RefreshToken);
            Assert.AreNotEqual(refreshToken2.TokenString, jwtResult.RefreshToken.TokenString);
            Assert.AreNotEqual(jwtResult.AccessToken, result.AccessToken);
        }


        [TestMethod]
        public async Task ShouldNotAllowToRefreshTokenWhenRefreshTokenIsExpired()
        {
            const string userName = "admin@localhost";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtTokenConfig = _serviceProvider.GetRequiredService<JwtTokenConfig>();
            var jwtResult1 = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-jwtTokenConfig.RefreshTokenExpiration - 1));
            var jwtResult2 = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult2.AccessToken); // valid JWT token
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = jwtResult1.RefreshToken.TokenString
            };
            var response = await _httpClient.PostAsync("api/account/refresh-token",
                new StringContent(JsonSerializer.Serialize(refreshRequest), Encoding.UTF8, MediaTypeNames.Application.Json)); // expired Refresh token
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Invalid token", responseContent);
        }

        [TestMethod]
        public async Task ShouldNotAllowToRefreshTokenAfterLogOut()
        {
            var credentials = new LoginRequest
            {
                Email = "admin@localhost",
                Password = "P@ssw0rd1!"
            };
            // login
            var loginResponse = await _httpClient.PostAsync("api/account/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode); // login OK

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            Assert.IsTrue(jwtAuthManager.UsersRefreshTokensReadOnlyDictionary.ContainsKey(loginResult.RefreshToken));

            // logout
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var logoutResponse = await _httpClient.PostAsync("api/account/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode); // logout ok
            Assert.IsFalse(jwtAuthManager.UsersRefreshTokensReadOnlyDictionary.ContainsKey(loginResult.RefreshToken)); //

            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = loginResult.RefreshToken // our old refresh token
            };
            var response = await _httpClient.PostAsync("api/account/refresh-token",
                new StringContent(JsonSerializer.Serialize(refreshRequest), Encoding.UTF8, MediaTypeNames.Application.Json)); // expired Refresh token
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.AreEqual("Invalid token", responseContent);
        }

        /*
        [TestMethod]
        public async Task ShouldAllowAdminImpersonateOthers()
        {
            const string userName = "admin";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            var request = new ImpersonationRequest { UserName = "test1" };
            var response = await _httpClient.PostAsync("api/account/impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResult>(responseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(request.UserName, result.UserName);
            Assert.AreEqual(userName, result.OriginalUserName);
            Assert.AreEqual(UserRoles.BasicUser, result.Role);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.RefreshToken));

            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(result.AccessToken);
            Assert.AreEqual(request.UserName, principal.Identity.Name);
            Assert.AreEqual(UserRoles.BasicUser, principal.FindFirst(ClaimTypes.Role).Value);
            Assert.AreEqual(userName, principal.FindFirst("OriginalUserName").Value);
            Assert.IsNotNull(jwtSecurityToken);
        }

        [TestMethod]
        public async Task ShouldForbidNonAdminToImpersonate()
        {
            const string userName = "test1";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.BasicUser)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            var request = new ImpersonationRequest { UserName = "test2" };
            var response = await _httpClient.PostAsync("api/account/impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task ShouldAllowAdminToStopImpersonation()
        {
            const string userName = "test1";
            const string originalUserName = "admin";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.BasicUser),
                new Claim("OriginalUserName", originalUserName)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            var response = await _httpClient.PostAsync("api/account/stop-impersonation", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResult>(responseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(originalUserName, result.UserName);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.OriginalUserName));
            Assert.AreEqual(UserRoles.Admin, result.Role);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.RefreshToken));

            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(result.AccessToken);
            Assert.AreEqual(originalUserName, principal.Identity.Name);
            Assert.AreEqual(UserRoles.Admin, principal.FindFirst(ClaimTypes.Role).Value);
            Assert.IsTrue(string.IsNullOrWhiteSpace(principal.FindFirst("OriginalUserName")?.Value));
            Assert.IsNotNull(jwtSecurityToken);
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestIfStopImpersonationWhenNotImpersonating()
        {
            const string userName = "test1";
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.BasicUser)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var jwtResult = jwtAuthManager.GenerateTokens(userName, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtResult.AccessToken);
            var request = new ImpersonationRequest { UserName = "test2" };
            var response = await _httpClient.PostAsync("api/account/stop-impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }*/
    }
}
