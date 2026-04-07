using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JavaMonitor.Core.Data;
using JavaMonitor.Core.Entities;

namespace JavaMonitor.Core.Services;

/// <summary>
/// 监控服务 - 核心业务逻辑
/// </summary>
public class MonitorService
{
    private readonly MonitorDbContext _db;
    private readonly PortCheckerService _portChecker;
    private readonly ILogger<MonitorService> _logger;

    public MonitorService(
        MonitorDbContext db,
        PortCheckerService portChecker,
        ILogger<MonitorService> logger)
    {
        _db = db;
        _portChecker = portChecker;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有监控目标
    /// </summary>
    public async Task<List<MonitorTarget>> GetAllTargetsAsync()
    {
        return await _db.Targets.OrderBy(t => t.Name).ToListAsync();
    }

    /// <summary>
    /// 获取单个监控目标
    /// </summary>
    public async Task<MonitorTarget?> GetTargetByIdAsync(int id)
    {
        return await _db.Targets.FindAsync(id);
    }

    /// <summary>
    /// 添加监控目标
    /// </summary>
    public async Task<MonitorTarget> AddTargetAsync(MonitorTarget target)
    {
        target.CreatedAt = DateTime.Now;
        _db.Targets.Add(target);
        await _db.SaveChangesAsync();
        _logger.LogInformation("添加监控目标: {Name} ({Host}:{Port})", target.Name, target.Host, target.Port);
        return target;
    }

    /// <summary>
    /// 更新监控目标
    /// </summary>
    public async Task<MonitorTarget?> UpdateTargetAsync(MonitorTarget target)
    {
        var existing = await _db.Targets.FindAsync(target.Id);
        if (existing == null) return null;

        existing.Name = target.Name;
        existing.Host = target.Host;
        existing.Port = target.Port;
        existing.TimeoutSeconds = target.TimeoutSeconds;
        existing.IntervalSeconds = target.IntervalSeconds;
        existing.IsEnabled = target.IsEnabled;
        existing.Description = target.Description;

        await _db.SaveChangesAsync();
        _logger.LogInformation("更新监控目标: {Name} ({Host}:{Port})", target.Name, target.Host, target.Port);
        return existing;
    }

    /// <summary>
    /// 删除监控目标
    /// </summary>
    public async Task<bool> DeleteTargetAsync(int id)
    {
        var target = await _db.Targets.FindAsync(id);
        if (target == null) return false;

        _db.Targets.Remove(target);
        await _db.SaveChangesAsync();
        _logger.LogInformation("删除监控目标: {Name}", target.Name);
        return true;
    }

    /// <summary>
    /// 对单个目标进行检测
    /// </summary>
    public async Task<MonitorRecord> CheckTargetAsync(MonitorTarget target)
    {
        var record = new MonitorRecord
        {
            TargetId = target.Id,
            CheckedAt = DateTime.Now
        };

        var (isOnline, responseTime, error) = await _portChecker.CheckPortAsync(
            target.Host, target.Port, target.TimeoutSeconds);

        record.IsOnline = isOnline;
        record.ResponseTimeMs = responseTime;
        record.ErrorMessage = error;

        // 更新目标最后检测时间
        // 注意：target 已经是 EF Core 跟踪的实体，直接修改属性即可保存
        // 使用 Attach 确保持久化（非跟踪状态时）
        if (_db.Entry(target).State == EntityState.Detached)
        {
            _db.Targets.Attach(target);
        }
        target.LastCheckAt = record.CheckedAt;

        _db.Records.Add(record);
        await _db.SaveChangesAsync();

        // 检查是否需要创建告警
        await CheckAndCreateAlertAsync(target, isOnline, error);

        return record;
    }

    /// <summary>
    /// 检测所有启用的目标
    /// </summary>
    public async Task<List<MonitorRecord>> CheckAllEnabledTargetsAsync()
    {
        var targets = await _db.Targets
            .Where(t => t.IsEnabled)
            .ToListAsync();

        var records = new List<MonitorRecord>();

        foreach (var target in targets)
        {
            try
            {
                var record = await CheckTargetAsync(target);
                records.Add(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检测目标 {Name} 时发生错误", target.Name);
            }
        }

        return records;
    }

    /// <summary>
    /// 获取最新检测记录
    /// </summary>
    public async Task<Dictionary<int, MonitorRecord>> GetLatestRecordsAsync()
    {
        return await _db.Records
            .GroupBy(r => r.TargetId)
            .Select(g => g.OrderByDescending(r => r.CheckedAt).First())
            .ToDictionaryAsync(r => r.TargetId);
    }

    /// <summary>
    /// 获取未确认的告警
    /// </summary>
    public async Task<List<AlertRecord>> GetUnacknowledgedAlertsAsync()
    {
        return await _db.Alerts
            .Include(a => a.Target)
            .Where(a => !a.IsAcknowledged)
            .OrderByDescending(a => a.AlertAt)
            .ToListAsync();
    }

    /// <summary>
    /// 确认告警
    /// </summary>
    public async Task<bool> AcknowledgeAlertAsync(int alertId)
    {
        var alert = await _db.Alerts.FindAsync(alertId);
        if (alert == null) return false;

        alert.IsAcknowledged = true;
        alert.AcknowledgedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 获取目标的状态统计
    /// </summary>
    public async Task<Dictionary<int, (int OnlineCount, int TotalCount)>> GetTargetStatisticsAsync(int hours = 24)
    {
        var since = DateTime.Now.AddHours(-hours);
        return await _db.Records
            .Where(r => r.CheckedAt >= since)
            .GroupBy(r => r.TargetId)
            .Select(g => new {
                TargetId = g.Key,
                OnlineCount = g.Count(r => r.IsOnline),
                TotalCount = g.Count()
            })
            .ToDictionaryAsync(
                x => x.TargetId,
                x => (x.OnlineCount, x.TotalCount));
    }

    /// <summary>
    /// 检查并创建告警
    /// </summary>
    private async Task CheckAndCreateAlertAsync(MonitorTarget target, bool isOnline, string? error)
    {
        // 获取最近的告警
        var latestAlert = await _db.Alerts
            .Where(a => a.TargetId == target.Id)
            .OrderByDescending(a => a.AlertAt)
            .FirstOrDefaultAsync();

        if (isOnline)
        {
            // 在线状态
            if (latestAlert?.AlertType == "Down" && !latestAlert.IsAcknowledged)
            {
                // 之前是宕机状态，现在恢复了
                var recoveredAlert = new AlertRecord
                {
                    TargetId = target.Id,
                    AlertType = "Recovered",
                    AlertAt = DateTime.Now,
                    Details = $"服务已恢复: {target.Host}:{target.Port}"
                };
                _db.Alerts.Add(recoveredAlert);
                await _db.SaveChangesAsync();
                _logger.LogInformation("服务恢复: {Name}", target.Name);
            }
        }
        else
        {
            // 宕机状态
            if (latestAlert == null || latestAlert.AlertType == "Recovered")
            {
                // 没有未恢复的告警，创建新的宕机告警
                var downAlert = new AlertRecord
                {
                    TargetId = target.Id,
                    AlertType = "Down",
                    AlertAt = DateTime.Now,
                    Details = $"服务不可达: {target.Host}:{target.Port}, 错误: {error}"
                };
                _db.Alerts.Add(downAlert);
                await _db.SaveChangesAsync();
                _logger.LogWarning("服务宕机: {Name}, 错误: {Error}", target.Name, error);
            }
        }
    }
}
