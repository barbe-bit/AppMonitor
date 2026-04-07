using Microsoft.EntityFrameworkCore;
using JavaMonitor.Core.Data;
using JavaMonitor.Core.Services;
using JavaMonitor.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库
builder.Services.AddDbContext<MonitorDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(builder.Environment.ContentRootPath, "monitor.db")}"));

// 添加核心服务
builder.Services.AddScoped<PortCheckerService>();
builder.Services.AddScoped<MonitorService>();

// 添加 SignalR 通知服务（使用 SignalR 实现）
builder.Services.AddScoped<IAlertNotificationService, SignalRAlertNotificationService>();

// 添加后台监控服务
builder.Services.AddHostedService<MonitorBackgroundService>();

// 添加 SignalR
builder.Services.AddSignalR();

// 添加 Razor 页面
builder.Services.AddRazorPages();

// 添加 Blazor Server
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MonitorDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapHub<MonitorHub>("/monitorHub");
app.MapFallbackToPage("/_Host");

app.Run();
