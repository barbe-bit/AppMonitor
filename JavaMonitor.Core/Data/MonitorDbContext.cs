using Microsoft.EntityFrameworkCore;
using JavaMonitor.Core.Entities;

namespace JavaMonitor.Core.Data;

/// <summary>
/// 数据库上下文
/// </summary>
public class MonitorDbContext : DbContext
{
    public MonitorDbContext(DbContextOptions<MonitorDbContext> options) : base(options)
    {
    }

    public DbSet<MonitorTarget> Targets { get; set; }
    public DbSet<MonitorRecord> Records { get; set; }
    public DbSet<AlertRecord> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 监控目标配置
        modelBuilder.Entity<MonitorTarget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Host).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Host);
        });

        // 监控记录配置
        modelBuilder.Entity<MonitorRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Target)
                  .WithMany()
                  .HasForeignKey(e => e.TargetId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.CheckedAt);
        });

        // 告警记录配置
        modelBuilder.Entity<AlertRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Target)
                  .WithMany()
                  .HasForeignKey(e => e.TargetId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.AlertAt);
        });
    }
}
