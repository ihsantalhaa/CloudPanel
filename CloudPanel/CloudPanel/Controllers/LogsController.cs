using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using CloudPanel.Controllers;
using CloudPanel.WebApi.Repositories.AuthRepository;
using CloudPanel.WebApi.Repositories.LogRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CloudPanel.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : Controller
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LogsController> _logger;
        private readonly IAmazonCloudWatchLogs _cloudWatchLogsClient;

        public LogsController(ILogRepository logRepository, ILogger<LogsController> logger, IAmazonCloudWatchLogs cloudWatchLogsClient)
        {
            _logRepository = logRepository;
            _logger = logger;
            _cloudWatchLogsClient = cloudWatchLogsClient;
        }

        [HttpGet("GetLogEventsAsync/{logStreamName}")]
        public async Task<IActionResult> GetLogEventsAsync(string logStreamName)
        {
            var result = await _logRepository.GetLogEventsAsync(logStreamName);
            _logger.LogInformation("All log events are listed");
            return Ok(result);
        }

        [HttpGet("GetLogStreamsAsync")]
        public async Task<IActionResult> GetLogStreamsAsync()
        {
            var result = await _logRepository.GetLogStreamsAsync();
            _logger.LogInformation("All log streams are listed");
            return Ok(result);
        }   

    }
}
