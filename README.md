# AppMonitor

🔔 基于 .NET 8 + Blazor Server 的轻量级应用/端口监控系统

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![License](https://img.shields.io/badge/License-Apache%202.0-green)

## 功能特性

- 🌐 **多协议支持**：支持 TCP 端口检测、HTTP/HTTPS 健康检查
- 🔄 **实时推送**：基于 SignalR 的实时状态更新
- 📊 **历史记录**：完整的检测历史记录存储
- 🚨 **灵活告警**：前端实时告警提示，支持自定义阈值
- 📱 **响应式界面**：适配桌面和移动设备
- 🐳 **Docker 部署**：一行命令即可部署

## 界面预览

```
┌─────────────────────────────────────────────────────────┐
│  🚀 AppMonitor                    [+ 添加监控目标]       │
├─────────────────────────────────────────────────────────┤
│  名称     地址                端口  状态   最后检测     │
│  ─────────────────────────────────────────────────────  │
│  我的博客  blog.example.com    443  ✅     17:25:30     │
│  API服务   api.example.com     8080 ✅     17:25:28     │
│  游戏服    game.example.com   25565 ❌   17:25:25     │
└─────────────────────────────────────────────────────────┘
```

## 快速开始

### 方式一：Docker 部署（推荐）

```bash
# 克隆项目
git clone https://github.com/barbe-bit/AppMonitor.git
cd AppMonitor

# 构建并运行
docker build -t appmonitor .
docker run -d --name appmonitor -p 8080:8080 \
  -v ./data:/app/data \
  --restart unless-stopped \
  appmonitor:latest
```

访问 http://localhost:8080 即可使用。

### 方式二：本地运行

```bash
# 克隆项目
git clone https://github.com/barbe-bit/AppMonitor.git
cd AppMonitor

# 安装 .NET 8 SDK
# 运行项目
dotnet run --project JavaMonitor.Web --urls "http://localhost:8080"
```

### 方式三：Linux 服务器部署

```bash
# 克隆项目
git clone https://github.com/barbe-bit/AppMonitor.git
cd AppMonitor

# 发布
dotnet publish JavaMonitor.Web -c Release -r linux-x64 --self-contained

# 使用 systemd 管理服务
sudo cp deploy/appmonitor.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable appmonitor
sudo systemctl start appmonitor
```

## 项目结构

```
AppMonitor/
├── JavaMonitor.Core/          # 核心业务逻辑
│   ├── Models/                 # 数据模型
│   ├── Services/               # 服务层（监控逻辑）
│   └── Data/                  # 数据库上下文
├── JavaMonitor.Web/            # Blazor Web 应用
│   ├── Pages/                  # 页面组件
│   ├── Shared/                 # 共享组件
│   └── Hubs/                   # SignalR Hub
├── deploy/                     # 部署脚本
│   ├── Dockerfile
│   └── appmonitor.service
└── JavaMonitor.sln             # 解决方案文件
```

## 配置说明

### 监控配置

在添加监控目标时：
- **地址**：支持 IP 地址或域名
- **端口**：
  - HTTP 协议默认端口 80
  - HTTPS 协议默认端口 443
  - 其他协议填写实际端口号
- **检测间隔**：默认 60 秒

### 数据库

默认使用 SQLite 数据库，存储在 `monitor.db` 文件中。

### SignalR 配置

实时推送间隔可在 `Program.cs` 中调整：

```csharp
// JavaMonitor.Web/Program.cs
services.AddHostedService<MonitorBackgroundService>(sp =>
    new MonitorBackgroundService(
        hubContext,
        monitorService,
        TimeSpan.FromSeconds(60)  // 推送间隔
    ));
```

## 技术栈

- **后端框架**：.NET 8.0
- **前端框架**：Blazor Server
- **数据库**：SQLite + Entity Framework Core
- **实时通信**：ASP.NET Core SignalR
- **容器化**：Docker

## API 参考

### 添加监控目标

```
POST /api/monitor/targets
Content-Type: application/json

{
  "name": "我的网站",
  "host": "example.com",
  "port": 443,
  "protocol": "https"
}
```

### 获取监控目标列表

```
GET /api/monitor/targets
```

### 删除监控目标

```
DELETE /api/monitor/targets/{id}
```

## 开发指南

### 环境要求

- .NET 8.0 SDK
- SQLite（内置，无需安装）

### 构建

```bash
# 编译
dotnet build JavaMonitor.sln

# 测试
dotnet test

# 发布
dotnet publish JavaMonitor.Web -c Release
```

## 许可证

本项目基于 [Apache License 2.0](LICENSE) 开源。

## 贡献指南

欢迎提交 Issue 和 Pull Request！

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 致谢

- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - 现代 Web UI 框架
- [Entity Framework Core](https://docs.microsoft.com/ef/core/) - ORM 框架
- [SignalR](https://docs.microsoft.com/aspnet/core/signalr/) - 实时通信库
