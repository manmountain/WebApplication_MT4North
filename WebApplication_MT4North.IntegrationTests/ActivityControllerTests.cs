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
    public class ActivityControllerTests
    {

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
        private static Activity _newActivity1, _newActivity2, _newActivity3, _newActivity4, _newActivity5;
        private static CustomActivityInfo _customActivity1, _customActivity2, _customActivity3;
        private static BaseActivityInfo _baseActivity1, _baseActivity2;

        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public async Task SetUpAsync()
        {
            //_httpClient = _testHostFixture.Client;
            //_serviceProvider = _testHostFixture.ServiceProvider;

        }




    }
}