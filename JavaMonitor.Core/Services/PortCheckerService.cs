using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JavaMonitor.Core.Data;
using JavaMonitor.Core.Entities;

namespace JavaMonitor.Core.Services;

/// <summary>
/// 端口检测服务
/// </summary>
public class PortCheckerService
{
    private readonly ILogger<PortCheckerService> _logger;

    public PortCheckerService(ILogger<PortCheckerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 检测端口是否可达
    /// </summary>
    public async Task<(bool IsOnline, int ResponseTimeMs, string? Error)> CheckPortAsync(
        string host, int port, int timeoutSeconds = 5)
    {
        var startTime = DateTime.Now;

        try
        {
            using var client = new TcpClient();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            await client.ConnectAsync(host, port, cts.Token);

            var responseTime = (int)(DateTime.Now - startTime).TotalMilliseconds;
            _logger.LogDebug("端口检测成功: {Host}:{Port}, 响应时间: {Time}ms", host, port, responseTime);

            return (true, responseTime, null);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("端口检测超时: {Host}:{Port}", host, port);
            return (false, (int)(DateTime.Now - startTime).TotalMilliseconds, "连接超时");
        }
        catch (SocketException ex)
        {
            _logger.LogWarning("端口检测失败: {Host}:{Port}, 错误: {Error}", host, port, ex.Message);
            return (false, (int)(DateTime.Now - startTime).TotalMilliseconds, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "端口检测异常: {Host}:{Port}", host, port);
            return (false, 0, ex.Message);
        }
    }
}
