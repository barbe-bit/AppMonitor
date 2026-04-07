using System;

namespace JavaMonitor.Core.Entities;

/// <summary>
/// 告警记录实体
/// </summary>
public class AlertRecord
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
    /// 告警类型: Down(宕机), Recovered(恢复)
    /// </summary>
    public string AlertType { get; set; } = "Down";

    /// <summary>
    /// 告警时间
    /// </summary>
    public DateTime AlertAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 是否已确认
    /// </summary>
    public bool IsAcknowledged { get; set; }

    /// <summary>
    /// 确认时间
    /// </summary>
    public DateTime? AcknowledgedAt { get; set; }

    /// <summary>
    /// 告警详情
    /// </summary>
    public string? Details { get; set; }
}
