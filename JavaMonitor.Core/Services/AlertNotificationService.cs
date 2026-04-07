using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using JavaMonitor.Core.Entities;

namespace JavaMonitor.Core.Services;

/// <summary>
/// SignalR 实时通知服务接口
/// </summary>
public interface IAlertNotificationService
{
    /// <summary>
    /// 发送告警通知
    /// </summary>
    Task SendAlertAsync(AlertRecord alert);

    /// <summary>
    /// 发送状态更新通知
    /// </summary>
    Task SendStatusUpdateAsync(MonitorRecord record);
}

/// <summary>
/// 空实现（当 SignalR Hub 不可用时使用）
/// </summary>
public class NullAlertNotificationService : IAlertNotificationService
{
    private readonly ILogger<NullAlertNotificationService> _logger;

    public NullAlertNotificationService(ILogger<NullAlertNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendAlertAsync(AlertRecord alert)
    {
        _logger.LogWarning("告警通知: [{Type}] {Target} - {Details}",
            alert.AlertType, alert.Target?.Name, alert.Details);
        return Task.CompletedTask;
    }

    public Task SendStatusUpdateAsync(MonitorRecord record)
    {
        return Task.CompletedTask;
    }
}

// DTOs
public class AlertDto
{
    public int Id { get; set; }
    public int TargetId { get; set; }
    public string TargetName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public DateTime AlertAt { get; set; }
    public string? Details { get; set; }
    public bool IsAcknowledged { get; set; }
}

public class StatusUpdateDto
{
    public int TargetId { get; set; }
    public bool IsOnline { get; set; }
    public int ResponseTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}

// SignalR Hub 接口（仅接口定义在 Core 中）
public interface IMonitorHubClient
{
    Task ReceiveAlert(AlertDto alert);
    Task ReceiveStatusUpdate(StatusUpdateDto status);
}
