using ClanManager.Core.Exceptions;
using ClanManager.WEB.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClanManager.Tests.Unit.Middleware
{
    public class GlobalExceptionFilterUnitTests
    {
        private readonly Mock<ILogger<GlobalExceptionFilter>> _mockLogger;
        private readonly Mock<ITempDataDictionaryFactory> _mockTempDataFactory;
        private readonly Mock<ITempDataDictionary> _mockTempData;
        private readonly GlobalExceptionFilter _filter;

        public GlobalExceptionFilterUnitTests()
        {
            _mockLogger = new Mock<ILogger<GlobalExceptionFilter>>();
            _mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();
            _mockTempData = new Mock<ITempDataDictionary>();
            _mockTempDataFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(_mockTempData.Object);

            _filter = new GlobalExceptionFilter(_mockLogger.Object, _mockTempDataFactory.Object);
        }

        private ExceptionContext CreateExceptionContext(Exception ex)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };
            actionContext.RouteData.Values["action"] = "TestInvoke";

            return new ExceptionContext(actionContext, new List<IFilterMetadata>()) { Exception = ex };
        }

        [Fact]
        public void OnException_WhenBusinessException_LogsWarningAndRedirects()
        {
            var exception = new UserNotFoundException();
            var context = CreateExceptionContext(exception);

            _filter.OnException(context);

            Assert.True(context.ExceptionHandled);
            var redirectResult = Assert.IsType<RedirectToActionResult>(context.Result);
            Assert.Equal("List", redirectResult.ActionName);
            Assert.Equal("Clan", redirectResult.ControllerName);

            _mockTempData.VerifySet(t => t["ErrorMessage"] = exception.Message);

            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Business logic exception")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void OnException_WhenTechnicalException_LogsError()
        {
            var exception = new Exception("Crash fatal");
            var context = CreateExceptionContext(exception);

            _filter.OnException(context);

            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled technical exception")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
