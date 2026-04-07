# Java应用监控中心 - 使用说明

## 项目概述
基于 .NET 8 + Blazor 的 Java 应用端口监控平台

## 技术架构
- **前端**: Blazor Server
- **后端**: ASP.NET Core 8.0
- **数据库**: SQLite
- **实时通信**: SignalR

## 项目结构
```
JavaMonitor/
├── JavaMonitor.Core/           # 核心业务逻辑
│   ├── Entities/                # 数据模型
│   │   ├── MonitorTarget.cs     # 监控目标
│   │   ├── MonitorRecord.cs     # 检测记录
│   │   └── AlertRecord.cs       # 告警记录
│   ├── Services/                # 业务服务
│   │   ├── MonitorService.cs    # 监控核心服务
│   │   ├── PortCheckerService.cs # 端口检测
│   │   └── MonitorBackgroundService.cs # 后台巡检
│   └── Data/
│       └── MonitorDbContext.cs # EF Core 数据库上下文
│
└── JavaMonitor.Web/             # Web 管理后台
    ├── Pages/
    │   ├── Index.razor          # 监控面板主页
    │   └── Monitor/
    │       ├── Add.razor        # 添加监控目标
    │       └── Edit.razor       # 编辑监控目标
    └── Hubs/
        └── MonitorHub.cs       # SignalR 实时通知
```

## 功能特性

### 1. 监控目标管理
- 手动录入 Java 应用地址和端口
- 支持配置超时时间和检测间隔
- 启用/禁用监控目标

### 2. 端口检测
- TCP 连接检测
- 记录响应时间
- 支持超时配置

### 3. 告警功能
- 服务宕机自动告警
- 服务恢复通知
- 告警确认功能
- Web界面实时提示

### 4. 后台巡检
- 每30秒自动巡检
- 记录检测历史
- SignalR 实时推送状态

## 启动方式

```bash
cd JavaMonitor.Web
dotnet run --urls "http://localhost:5000"
```

然后访问 http://localhost:5000

## 使用流程

1. **添加监控目标**: 点击"添加监控"按钮
2. **填写配置**:
   - 应用名称（如：用户服务）
   - 主机地址（localhost 或 IP）
   - 端口号
3. **查看状态**: 返回首页查看监控状态
4. **处理告警**: 发现告警时在面板上会高亮显示，点击"确认"处理

## 注意事项

- 数据库文件会自动创建在 `JavaMonitor.Web/monitor.db`
- 首次启动会自动初始化数据库
- 监控服务每30秒自动执行检测
