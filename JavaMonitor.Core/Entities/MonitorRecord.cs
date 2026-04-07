using System;

namespace JavaMonitor.Core.Entities;

/// <summary>
/// 监控记录实体
/// </summary>
public class MonitorRecord
{
    public int Id { get; set; }

    /// <summary>
    /// 关联的监控目标ID
    /// </summary>
    public int TargetId { get; set; }

    /// <summary>
    /// 监控目标（导航属性）
    /// </summary>
    public MonitorTarget? Target { get; set; }

    /// <summary>
    /// 检测时间
    /// </summary>
    public DateTime CheckedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 是否在线
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// 响应时间（毫秒）
    /// </summary>
    public int ResponseTimeMs { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
}
