
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using System.Net;

namespace CloudPanel.WebApi.Repositories.LogRepository
{
    public class LogRepository : ILogRepository
    {
        private readonly IAmazonCloudWatchLogs _cloudWatchLogsClient;
        private readonly IConfiguration _configuration;

        public LogRepository(IAmazonCloudWatchLogs cloudWatchLogsClient, IConfiguration configuration)
        {
            _cloudWatchLogsClient = cloudWatchLogsClient;
            _configuration = configuration;
        }

        public async Task<List<OutputLogEvent>> GetLogEventsAsync(string logStreamName)
        {
            try
            {
                var logGroupName = "cloudpanel_applogs";
                var logStreamNameClean = WebUtility.UrlDecode(logStreamName);
                var request = new GetLogEventsRequest
                {
                    LogGroupName = logGroupName,
                    LogStreamName = logStreamNameClean,
                };
                var response = await _cloudWatchLogsClient.GetLogEventsAsync(request);
                var cc = response.ToString();
                return response.Events;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting log events: {ex.Message}");
            }
        }

        public async Task<List<LogStream>> GetLogStreamsAsync()
        {
            try
            {
                var logGroupName = "cloudpanel_applogs";
                var request = new DescribeLogStreamsRequest { LogGroupName = logGroupName };
                var response = await _cloudWatchLogsClient.DescribeLogStreamsAsync(request);
                return response.LogStreams;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while getting log streams: {ex.Message}");
            }
        }
    }
}
