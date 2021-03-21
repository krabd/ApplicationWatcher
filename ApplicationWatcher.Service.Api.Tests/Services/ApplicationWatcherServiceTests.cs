using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcher.Grpc.Client.Interfaces;
using ApplicationWatcher.Service.Api.Models.Options;
using ApplicationWatcher.Service.Api.Services;
using ApplicationWatcher.Service.Utils.Interfaces;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ApplicationWatcher.Service.Api.Tests.Services
{
    public class ApplicationWatcherServiceTests
    {
        private ApplicationWatcherService CreateApplicationWatcherServiceForReboot(IRegistryService registryService, IProcessService processService, IOptions<RegistryOptions> registryOptions)
        {
            var logger = Mock.Of<ILogger<ApplicationWatcherService>>();
            return new ApplicationWatcherService(registryService, processService,
                Mock.Of<IFileWrapperService>(), Mock.Of<IGrpcClientService>(),
                registryOptions, Mock.Of<IOptions<HealthCheckOptions>>(), Mock.Of<IOptions<GrpcOptions>>(),
                logger);
        }

        private ApplicationWatcherService CreateApplicationWatcherServiceForGetLogs(IRegistryService registryService,
            IFileWrapperService fileWrapperService, IOptions<RegistryOptions> registryOptions)
        {
            var logger = Mock.Of<ILogger<ApplicationWatcherService>>();
            return new ApplicationWatcherService(registryService, Mock.Of<IProcessService>(),
                fileWrapperService, Mock.Of<IGrpcClientService>(),
                registryOptions, Mock.Of<IOptions<HealthCheckOptions>>(), Mock.Of<IOptions<GrpcOptions>>(),
                logger);
        }

        private ApplicationWatcherService CreateApplicationWatcherServiceForHealthCheck(IOptions<GrpcOptions> grpcOptions, IOptions<HealthCheckOptions> healthCheckOptions, IGrpcClientService grpcClientService)
        {
            return new ApplicationWatcherService(Mock.Of<IRegistryService>(), Mock.Of<IProcessService>(), Mock.Of<IFileWrapperService>(),
                grpcClientService,
                Mock.Of<IOptions<RegistryOptions>>(), healthCheckOptions, grpcOptions,
                Mock.Of<ILogger<ApplicationWatcherService>>());
        }

        [Fact]
        public void Reboot_WhenGetExePathNull_ThenNoReboot()
        {
            var basePath = "basePath";
            var exeValue = "exeValue";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, exeValue)).Returns((string) null);
            var processServiceMock = new Mock<IProcessService>();
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.ExeValue = exeValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForReboot(registryServiceMock.Object, processServiceMock.Object, registryOptionsObjMock.Object);

            applicationWatcherService.Reboot();

            processServiceMock.Verify(i => i.GetProcessByExePath(It.IsAny<string>()), Times.Exactly(0));
        }

        [Fact]
        public void Reboot_WhenGetExePathExePath_ThenReboot()
        {
            var basePath = "basePath";
            var exeValue = "exeValue";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, exeValue)).Returns("exePath");
            var processServiceMock = new Mock<IProcessService>();
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.ExeValue = exeValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForReboot(registryServiceMock.Object, processServiceMock.Object, registryOptionsObjMock.Object);

            applicationWatcherService.Reboot();

            processServiceMock.Verify(i => i.GetProcessByExePath(It.IsAny<string>()), Times.Exactly(1));
            processServiceMock.Verify(i => i.StartProcessFromWindowService(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetLogs_WhenGetLogsPathNull_ThenNull()
        {
            var basePath = "basePath";
            var logsValue = "logsValue";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, logsValue)).Returns((string)null);
            var fileWrapperServiceMock = new Mock<IFileWrapperService>();
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.LogsValue = logsValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForGetLogs(registryServiceMock.Object, fileWrapperServiceMock.Object, registryOptionsObjMock.Object);

            var logsData = applicationWatcherService.GetLogs();

            Assert.Null(logsData);
            fileWrapperServiceMock.Verify(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [Fact]
        public void GetLogs_WhenCreateZipException_ThenNull()
        {
            var basePath = "basePath";
            var logsValue = "logsValue";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, logsValue)).Returns("logsPath");
            var fileWrapperServiceMock = new Mock<IFileWrapperService>();
            fileWrapperServiceMock.Setup(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.LogsValue = logsValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForGetLogs(registryServiceMock.Object, fileWrapperServiceMock.Object, registryOptionsObjMock.Object);

            var logsData = applicationWatcherService.GetLogs();

            Assert.Null(logsData);
            fileWrapperServiceMock.Verify(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetLogs_WhenReadAllBytesException_ThenNull()
        {
            var basePath = "basePath";
            var logsValue = "logsValue";
            var logsPath = "logsPath";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, logsValue)).Returns(logsPath);
            var fileWrapperServiceMock = new Mock<IFileWrapperService>();
            fileWrapperServiceMock.Setup(i => i.ReadAllBytes(It.IsAny<string>())).Throws(new Exception());
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.LogsValue = logsValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForGetLogs(registryServiceMock.Object, fileWrapperServiceMock.Object, registryOptionsObjMock.Object);

            var logsData = applicationWatcherService.GetLogs();

            Assert.Null(logsData);
            fileWrapperServiceMock.Verify(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            fileWrapperServiceMock.Verify(i => i.ReadAllBytes(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetLogs_WhenReadAllBytesOk_ThenOk()
        {
            var expected = new[] {(byte) 1};
            var basePath = "basePath";
            var logsValue = "logsValue";
            var logsPath = "logsPath";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, logsValue)).Returns(logsPath);
            var fileWrapperServiceMock = new Mock<IFileWrapperService>();
            fileWrapperServiceMock.Setup(i => i.ReadAllBytes(It.IsAny<string>())).Returns(expected);
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.LogsValue = logsValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForGetLogs(registryServiceMock.Object, fileWrapperServiceMock.Object, registryOptionsObjMock.Object);

            var actual = applicationWatcherService.GetLogs();

            Assert.Equal(expected, actual);
            fileWrapperServiceMock.Verify(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            fileWrapperServiceMock.Verify(i => i.ReadAllBytes(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public void GetLogs_WhenDeleteFileException_ThenOk()
        {
            var expected = new[] { (byte)1 };
            var basePath = "basePath";
            var logsValue = "logsValue";
            var logsPath = "logsPath";
            var registryServiceMock = new Mock<IRegistryService>();
            registryServiceMock.Setup(i => i.GetRegistryValue(basePath, logsValue)).Returns(logsPath);
            var fileWrapperServiceMock = new Mock<IFileWrapperService>();
            fileWrapperServiceMock.Setup(i => i.ReadAllBytes(It.IsAny<string>())).Returns(expected);
            fileWrapperServiceMock.Setup(i => i.Delete(It.IsAny<string>())).Throws(new Exception());
            var registryOptionsMock = new Mock<RegistryOptions>();
            registryOptionsMock.Object.BasePath = basePath;
            registryOptionsMock.Object.LogsValue = logsValue;
            registryOptionsMock.SetupAllProperties();
            var registryOptionsObjMock = new Mock<IOptions<RegistryOptions>>();
            registryOptionsObjMock.Setup(i => i.Value).Returns(registryOptionsMock.Object);
            var applicationWatcherService = CreateApplicationWatcherServiceForGetLogs(registryServiceMock.Object, fileWrapperServiceMock.Object, registryOptionsObjMock.Object);

            var actual = applicationWatcherService.GetLogs();

            Assert.Equal(expected, actual);
            fileWrapperServiceMock.Verify(i => i.CreateZip(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
            fileWrapperServiceMock.Verify(i => i.ReadAllBytes(It.IsAny<string>()), Times.Exactly(1));
            fileWrapperServiceMock.Verify(i => i.Delete(It.IsAny<string>()), Times.Exactly(1));
        }

        [Fact]
        public async void HealthCheck_WhenGrpcHealthCheckTrue_ThenTrue()
        {
            var grpcOptionsMock = new Mock<GrpcOptions>();
            grpcOptionsMock.Object.Host = "localhost";
            grpcOptionsMock.Object.Port = 5000;
            grpcOptionsMock.SetupAllProperties();
            var grpcOptionsObjMock = new Mock<IOptions<GrpcOptions>>();
            grpcOptionsObjMock.Setup(i => i.Value).Returns(grpcOptionsMock.Object);
            var healthCheckOptionsMock = new Mock<HealthCheckOptions>();
            healthCheckOptionsMock.Object.TimeoutSeconds = 5;
            healthCheckOptionsMock.SetupAllProperties();
            var healthCheckOptionsObjMock = new Mock<IOptions<HealthCheckOptions>>();
            healthCheckOptionsObjMock.Setup(i => i.Value).Returns(healthCheckOptionsMock.Object);
            var grpcClientServiceMock = new Mock<IGrpcClientService>();
            grpcClientServiceMock
                .Setup(x => x.HealthCheck(grpcOptionsMock.Object.GetUri(), healthCheckOptionsMock.Object.TimeoutSeconds, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            var applicationWatcherService = CreateApplicationWatcherServiceForHealthCheck(grpcOptionsObjMock.Object, healthCheckOptionsObjMock.Object, grpcClientServiceMock.Object);

            var actual = await applicationWatcherService.HealthCheckAsync(CancellationToken.None);

            Assert.True(actual);
        }

        [Fact]
        public async void HealthCheck_WhenGrpcHealthCheckFalse_ThenFalse()
        {
            var grpcOptionsMock = new Mock<GrpcOptions>();
            grpcOptionsMock.Object.Host = "localhost";
            grpcOptionsMock.Object.Port = 5000;
            grpcOptionsMock.SetupAllProperties();
            var grpcOptionsObjMock = new Mock<IOptions<GrpcOptions>>();
            grpcOptionsObjMock.Setup(i => i.Value).Returns(grpcOptionsMock.Object);
            var healthCheckOptionsMock = new Mock<HealthCheckOptions>();
            healthCheckOptionsMock.Object.TimeoutSeconds = 5;
            healthCheckOptionsMock.SetupAllProperties();
            var healthCheckOptionsObjMock = new Mock<IOptions<HealthCheckOptions>>();
            healthCheckOptionsObjMock.Setup(i => i.Value).Returns(healthCheckOptionsMock.Object);
            var grpcClientServiceMock = new Mock<IGrpcClientService>();
            grpcClientServiceMock
                .Setup(x => x.HealthCheck(grpcOptionsMock.Object.GetUri(), healthCheckOptionsMock.Object.TimeoutSeconds, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
            var applicationWatcherService = CreateApplicationWatcherServiceForHealthCheck(grpcOptionsObjMock.Object, healthCheckOptionsObjMock.Object, grpcClientServiceMock.Object);

            var actual = await applicationWatcherService.HealthCheckAsync(CancellationToken.None);

            Assert.False(actual);
        }

        [Fact]
        public async void HealthCheck_WhenGrpcHealthCheckException_ThenFalse()
        {
            var grpcOptionsMock = new Mock<GrpcOptions>();
            grpcOptionsMock.Object.Host = "localhost";
            grpcOptionsMock.Object.Port = 5000;
            grpcOptionsMock.SetupAllProperties();
            var grpcOptionsObjMock = new Mock<IOptions<GrpcOptions>>();
            grpcOptionsObjMock.Setup(i => i.Value).Returns(grpcOptionsMock.Object);
            var healthCheckOptionsMock = new Mock<HealthCheckOptions>();
            healthCheckOptionsMock.Object.TimeoutSeconds = 5;
            healthCheckOptionsMock.SetupAllProperties();
            var healthCheckOptionsObjMock = new Mock<IOptions<HealthCheckOptions>>();
            healthCheckOptionsObjMock.Setup(i => i.Value).Returns(healthCheckOptionsMock.Object);
            var grpcClientServiceMock = new Mock<IGrpcClientService>();
            grpcClientServiceMock
                .Setup(x => x.HealthCheck(grpcOptionsMock.Object.GetUri(), healthCheckOptionsMock.Object.TimeoutSeconds, It.IsAny<CancellationToken>()))
                .Throws(new Exception());
            var applicationWatcherService = CreateApplicationWatcherServiceForHealthCheck(grpcOptionsObjMock.Object, healthCheckOptionsObjMock.Object, grpcClientServiceMock.Object);

            var actual = await applicationWatcherService.HealthCheckAsync(CancellationToken.None);

            Assert.False(actual);
        }
    }
}
