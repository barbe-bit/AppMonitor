#!/bin/bash
# AppMonitor 部署脚本

set -e

# 配置
APP_NAME="appmonitor"
APP_DIR="/opt/appmonitor"
DATA_DIR="/opt/appmonitor/data"
PORT=8080

echo "🚀 开始部署 AppMonitor..."

# 检查是否为 root 用户
if [ "$EUID" -ne 0 ]; then
    echo "⚠️  请使用 root 用户运行此脚本"
    exit 1
fi

# 创建目录
echo "📁 创建目录..."
mkdir -p $APP_DIR
mkdir -p $DATA_DIR

# 下载或构建
if [ -f "appmonitor_deploy.tar.gz" ]; then
    echo "📦 使用本地包部署..."
    tar -xzf appmonitor_deploy.tar.gz -C $APP_DIR
else
    echo "📦 需要打包文件: appmonitor_deploy.tar.gz"
    echo "请先运行: dotnet publish JavaMonitor.Web -c Release -r linux-x64 --self-contained"
    exit 1
fi

# 设置权限
echo "🔒 设置权限..."
chown -R www-data:www-data $APP_DIR
chmod -R 755 $APP_DIR

# 创建 systemd 服务文件
echo "📝 创建 systemd 服务..."
cat > /etc/systemd/system/${APP_NAME}.service <<EOF
[Unit]
Description=AppMonitor - Application Monitoring Service
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=$APP_DIR
ExecStart=$APP_DIR/JavaMonitor.Web --urls http://+:${PORT}
Restart=always
RestartSec=5
Environment=TZ=Asia/Shanghai

[Install]
WantedBy=multi-user.target
EOF

# 重新加载 systemd
echo "🔄 重载 systemd..."
systemctl daemon-reload
systemctl enable ${APP_NAME}

# 启动服务
echo "▶️  启动服务..."
systemctl restart ${APP_NAME}

# 检查状态
sleep 3
if systemctl is-active --quiet ${APP_NAME}; then
    echo ""
    echo "✅ 部署成功！"
    echo "   访问地址: http://localhost:${PORT}"
    echo "   状态查看: systemctl status ${APP_NAME}"
    echo "   日志查看: journalctl -u ${APP_NAME} -f"
else
    echo ""
    echo "❌ 部署失败，请检查日志:"
    journalctl -u ${APP_NAME} --no-pager -n 20
    exit 1
fi
