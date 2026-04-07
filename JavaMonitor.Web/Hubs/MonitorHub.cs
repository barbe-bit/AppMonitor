using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using JavaMonitor.Core.Services;
using JavaMonitor.Core.Entities;

namespace JavaMonitor.Web.Hubs;

/// <summary>
/// SignalR Hub 实现
/// </summary>
public class MonitorHub : Hub<IMonitorHubClient>
{
    private readonly ILogger<MonitorHub> _logger;

    public MonitorHub(ILogger<MonitorHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("客户端已连接: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("客户端已断开: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// SignalR 通知服务实现
/// </summary>
public class SignalRAlertNotificationService : IAlertNotificationService
{
    private readonly IHubContext<MonitorHub, IMonitorHubClient> _hubContext;
    private readonly ILogger<SignalRAlertNotificationService> _logger;

    public SignalRAlertNotificationService(
        IHubContext<MonitorHub, IMonitorHubClient> hubContext,
        ILogger<SignalRAlertNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendAlertAsync(AlertRecord alert)
    {
        _logger.LogInformation("发送告警通知: {AlertType} - {TargetName}",
            alert.AlertType, alert.Target?.Name);

        await _hubContext.Clients.All.ReceiveAlert(new AlertDto
        {
            Id = alert.Id,
            TargetId = alert.TargetId,
            TargetName = alert.Target?.Name ?? "Unknown",
            AlertType = alert.AlertType,
            AlertAt = alert.AlertAt,
            Details = alert.Details,
            IsAcknowledged = alert.IsAcknowledged
        });
    }

    public async Task SendStatusUpdateAsync(MonitorRecord record)
    {
        await _hubContext.Clients.All.ReceiveStatusUpdate(new StatusUpdateDto
        {
            TargetId = record.TargetId,
            IsOnline = record.IsOnline,
            ResponseTimeMs = record.ResponseTimeMs,
            ErrorMessage = record.ErrorMessage,
            CheckedAt = record.CheckedAt
        });
    }
}
