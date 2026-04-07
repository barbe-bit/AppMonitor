using System;

namespace JavaMonitor.Core.Entities;

/// <summary>
/// 监控目标实体
/// </summary>
public class MonitorTarget
{
    public int Id { get; set; }

    /// <summary>
    /// 应用名称（显示用）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址 (IP 或域名)
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// 端口号
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 检测超时时间（秒）
    /// </summary>
    public int TimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// 检测间隔（秒）
    /// </summary>
    public int IntervalSeconds { get; set; } = 30;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 最后检测时间
    /// </summary>
    public DateTime? LastCheckAt { get; set; }
}
