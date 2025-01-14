using Amazon.CloudWatchLogs.Model;
using CloudPanel.WebApi.Dtos.GroupFileDtos;

namespace CloudPanel.WebApi.Repositories.LogRepository
{
    public interface ILogRepository
    {
        Task<List<OutputLogEvent>> GetLogEventsAsync(string logStreamName);
        Task<List<LogStream>> GetLogStreamsAsync();
    }
}
