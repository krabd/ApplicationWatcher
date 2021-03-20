using System;
using System.Diagnostics;
using ApplicationWatcher.Service.Utils.Interfaces.Wrappers;
using ApplicationWatcher.Service.Utils.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApplicationWatcher.Service.Utils.Tests.Services
{
    public class ProcessServiceTests
    {
        private ProcessService CreateProcessService(IPathWrapperService pathWrapperService, IProcessWrapperService processWrapperService)
        {
            var logger = Mock.Of<ILogger<ProcessService>>();
            return new ProcessService(pathWrapperService, processWrapperService, logger);
        }

        [Fact]
        public void GetProcessByExePath_WhenExePathNull_ThenNull()
        {
            string exePath = null;
            var processService = CreateProcessService(Mock.Of<IPathWrapperService>(), Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            
        }

        [Fact]
        public void GetProcessByExePath_WhenExePathEmpty_ThenNull()
        {
            var exePath = string.Empty;
            var processService = CreateProcessService(Mock.Of<IPathWrapperService>(), Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
        }

        [Fact]
        public void GetProcessByExePath_WhenFileNameNull_ThenNull()
        {
            var exePath = "exePath";
            var pathWrapperServiceMock = new Mock<IPathWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetFileNameWithoutExtension(exePath)).Returns((string?)null);
            var processService = CreateProcessService(pathWrapperServiceMock.Object, Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            pathWrapperServiceMock.Verify(i => i.GetFileNameWithoutExtension(exePath), Times.Exactly(1));
        }

        [Fact]
        public void GetProcessByExePath_WhenFileNameEmpty_ThenNull()
        {
            var exePath = "exePath";
            var pathWrapperServiceMock = new Mock<IPathWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetFileNameWithoutExtension(exePath)).Returns(string.Empty);
            var processService = CreateProcessService(pathWrapperServiceMock.Object, Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            pathWrapperServiceMock.Verify(i => i.GetFileNameWithoutExtension(exePath), Times.Exactly(1));
        }

        [Fact]
        public void GetProcessByExePath_WhenDirectoryNull_ThenNull()
        {
            var exePath = "exePath";
            var pathWrapperServiceMock = new Mock<IPathWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetFileNameWithoutExtension(exePath)).Returns("fileName");
            pathWrapperServiceMock.Setup(x => x.GetDirectoryName(exePath)).Returns((string?)null);
            var processService = CreateProcessService(pathWrapperServiceMock.Object, Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            pathWrapperServiceMock.Verify(i => i.GetFileNameWithoutExtension(exePath), Times.Exactly(1));
            pathWrapperServiceMock.Verify(i => i.GetDirectoryName(exePath), Times.Exactly(1));
        }

        [Fact]
        public void GetProcessByExePath_WhenDirectoryEmpty_ThenNull()
        {
            var exePath = "exePath";
            var pathWrapperServiceMock = new Mock<IPathWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetFileNameWithoutExtension(exePath)).Returns("fileName");
            pathWrapperServiceMock.Setup(x => x.GetDirectoryName(exePath)).Returns(string.Empty);
            var processWrapperServiceMock = new Mock<IProcessWrapperService>();
            processWrapperServiceMock.Setup(x => x.GetProcessesByName(It.IsAny<string>())).Returns(Array.Empty<Process>());
            var processService = CreateProcessService(pathWrapperServiceMock.Object, Mock.Of<IProcessWrapperService>());

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            pathWrapperServiceMock.Verify(i => i.GetFileNameWithoutExtension(exePath), Times.Exactly(1));
            pathWrapperServiceMock.Verify(i => i.GetDirectoryName(exePath), Times.Exactly(1));
            processWrapperServiceMock.Verify(i => i.GetProcessesByName(exePath), Times.Exactly(0));
        }

        [Fact]
        public void GetProcessByExePath_WhenProcessesByNameNotFound_ThenNull()
        {
            var exePath = "exePath";
            var pathWrapperServiceMock = new Mock<IPathWrapperService>();
            pathWrapperServiceMock.Setup(x => x.GetFileNameWithoutExtension(exePath)).Returns("fileName");
            pathWrapperServiceMock.Setup(x => x.GetDirectoryName(exePath)).Returns("directoryName");
            var processWrapperServiceMock = new Mock<IProcessWrapperService>();
            processWrapperServiceMock.Setup(x => x.GetProcessesByName(It.IsAny<string>())).Returns(Array.Empty<Process>());
            var processService = CreateProcessService(pathWrapperServiceMock.Object, processWrapperServiceMock.Object);

            var process = processService.GetProcessByExePath(exePath);

            Assert.Null(process);
            pathWrapperServiceMock.Verify(i => i.GetFileNameWithoutExtension(exePath), Times.Exactly(1));
            pathWrapperServiceMock.Verify(i => i.GetDirectoryName(exePath), Times.Exactly(1));
            processWrapperServiceMock.Verify(i => i.GetProcessesByName(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
