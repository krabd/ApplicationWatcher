using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using ApplicationWatcher.Service.Utils.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

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
    }
}
