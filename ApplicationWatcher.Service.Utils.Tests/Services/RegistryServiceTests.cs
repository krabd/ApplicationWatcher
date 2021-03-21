using System;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using ApplicationWatcher.Service.Utils.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace ApplicationWatcher.Service.Utils.Tests.Services
{
    public class RegistryServiceTests
    {
        private RegistryService CreateRegistryService(IRegistryWrapperService registryWrapperService)
        {
            var logger = Mock.Of<ILogger<RegistryService>>();
            return new RegistryService(registryWrapperService, logger);
        }

        [Fact]
        public void GetRegistryValue_WhenBasePathNull_ThenNull()
        {
            string basePath = null;
            var valueName = "valueName";
            var registryService = CreateRegistryService(Mock.Of<IRegistryWrapperService>());

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenValueNameNull_ThenNull()
        {
            var basePath = "basePath";
            string valueName = null;
            var registryService = CreateRegistryService(Mock.Of<IRegistryWrapperService>());

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenBasePathEmpty_ThenNull()
        {
            var basePath = string.Empty;
            var valueName = "valueName";
            var registryService = CreateRegistryService(Mock.Of<IRegistryWrapperService>());

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenValueNameEmpty_ThenNull()
        {
            var basePath = "basePath";
            var valueName = string.Empty;
            var registryService = CreateRegistryService(Mock.Of<IRegistryWrapperService>());

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenValueNull_ThenNull()
        {
            var basePath = "basePath";
            var valueName = "valueName";
            var pathWrapperServiceMock = new Mock<IRegistryWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetRegistryValue<string>(basePath, valueName, RegistryHive.LocalMachine)).Returns((string)null);
            var registryService = CreateRegistryService(pathWrapperServiceMock.Object);

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenValueValue_ThenValue()
        {
            var basePath = "basePath";
            var valueName = "valueName";
            var pathWrapperServiceMock = new Mock<IRegistryWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetRegistryValue<string>(basePath, valueName, RegistryHive.LocalMachine)).Returns("value");
            var registryService = CreateRegistryService(pathWrapperServiceMock.Object);

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.NotNull(registryValue);
            Assert.Equal("value", registryValue);
        }

        [Fact]
        public void GetRegistryValue_WhenGetValueException_ThenNull()
        {
            var basePath = "basePath";
            var valueName = "valueName";
            var pathWrapperServiceMock = new Mock<IRegistryWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetRegistryValue<string>(basePath, valueName, RegistryHive.LocalMachine)).Throws(new Exception());
            var registryService = CreateRegistryService(pathWrapperServiceMock.Object);

            var registryValue = registryService.GetRegistryValue(basePath, valueName);

            Assert.Null(registryValue);
        }
    }
}
