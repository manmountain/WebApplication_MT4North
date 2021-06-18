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
        private readonly TestHostFixture _testHostFixture = new TestHostFixture();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public async Task SetUpAsync()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;

        }
    }
}