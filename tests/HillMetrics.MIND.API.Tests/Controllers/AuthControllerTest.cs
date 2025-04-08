using HillMetrics.MIND.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.Core.API.Contracts;
using HillMetrics.MIND.Infrastructure;
using HillMetrics.Core.API.Configs;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.API.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTest
    {
        private IAuthenticationService _authenticationService;
        private IRedirectUrlValidator _redirectUrlValidator;
        private CorsConfig _corsConfig;

        private AuthController _controller;
        private readonly string [] _validRedirectUrls = ["http://localhost:123"];
        private readonly string _redirectAuthenticationUrl = "http://localhost/redirect-login";
        private ILogger<AuthController> _logger;
        private ITokenExchangeService _tokenExchangeService;
        private ICookieService _cookieService;

        [SetUp]
        public void Setup()
        {
            _authenticationService = Substitute.For<IAuthenticationService>();

            _authenticationService.GetAuthenticationUrl(Arg.Any<string>()).Returns(_redirectAuthenticationUrl);

            _logger = Substitute.For<ILogger<AuthController>>();
            _tokenExchangeService = Substitute.For<ITokenExchangeService>();

            _cookieService = Substitute.For<ICookieService>();

            _corsConfig = new CorsConfig()
            {
                AllowedOrigins = _validRedirectUrls
            };

            _redirectUrlValidator = new RedirectUrlValidator(Options.Create(_corsConfig));

            _controller = new AuthController(_authenticationService, _redirectUrlValidator, _logger, _tokenExchangeService, _cookieService);
        }

        [Test]
        public void Login_RedirectUrlAllowed_Return_RedirectResult()
        {
            IActionResult result = _controller.Login(_validRedirectUrls[0]);
            
            Assert.That(result, Is.InstanceOf<RedirectResult>());
            
            RedirectResult redirectResult = (RedirectResult)result;
            Assert.That(redirectResult.Url, Is.EqualTo(_redirectAuthenticationUrl));
        }

        [Test]
        public void Login_RedirectUrlNotAValidUrl_Return_ForbidenResult()
        {
            IActionResult result = _controller.Login("invalid url");

            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public void Login_RedirectUrlNotAllowedUrl_Return_ForbidenResult()
        {
            IActionResult result = _controller.Login("http://localhost/redirect-login-not-whitelisted");
    
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }
    }
}
