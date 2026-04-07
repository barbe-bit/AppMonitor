using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JavaMonitor.Core.Services;

/// <summary>
/// 后台监控服务 - 定时巡检
/// </summary>
public class MonitorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MonitorBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

    public MonitorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<MonitorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("监控后台服务已启动");

        // 等待应用完全启动
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCheckAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行监控检查时发生错误");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("监控后台服务已停止");
    }

    private async Task PerformCheckAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var monitorService = scope.ServiceProvider.GetRequiredService<MonitorService>();
        var notificationService = scope.ServiceProvider.GetService<IAlertNotificationService>();

        _logger.LogDebug("开始执行监控检查...");

        var records = await monitorService.CheckAllEnabledTargetsAsync();

        // 发送状态更新通知
        if (notificationService != null)
        {
            foreach (var record in records)
            {
                await notificationService.SendStatusUpdateAsync(record);
            }
        }

        // 检查是否有新的未确认告警，发送通知
        if (notificationService != null)
        {
            var alerts = await monitorService.GetUnacknowledgedAlertsAsync();
            foreach (var alert in alerts.Where(a => a.AlertAt > DateTime.Now.AddMinutes(-1)))
            {
                await notificationService.SendAlertAsync(alert);
            }
        }

        var onlineCount = records.Count(r => r.IsOnline);
        var totalCount = records.Count;

        _logger.LogInformation(
            "监控检查完成: {Online}/{Total} 在线",
            onlineCount, totalCount);
    }
}
