using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcher.Service.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApplicationWatcher.Service.Api.Controllers
{
    [Route("api/application_watcher")]
    [ApiController]
    public class ApplicationWatcherController : Controller
    {
        private readonly IApplicationWatcherService _applicationWatcherService;
        private readonly ILogger<ApplicationWatcherController> _logger;

        public ApplicationWatcherController(IApplicationWatcherService applicationWatcherService, ILogger<ApplicationWatcherController> logger)
        {
            _applicationWatcherService = applicationWatcherService;
            _logger = logger;
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("reboot")]
        public IActionResult Reboot(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"reboot -->");

            _applicationWatcherService.Reboot();

            _logger.LogInformation($"reboot <--");
            return Ok();
        }

        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("get_logs")]
        public IActionResult GetLogs(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"get_logs -->");

            var logsData = _applicationWatcherService.GetLogs();

            _logger.LogInformation($"get_logs <--");
            if (logsData == null)
                return NotFound();

            return File(logsData, "application/x-zip-compressed");
        }

        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [HttpGet("health_check")]
        public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"health_check -->");

            var result = await _applicationWatcherService.HealthCheckAsync(cancellationToken);

            _logger.LogInformation($"health_check <--");
            return Ok(result);
        }
    }
}
