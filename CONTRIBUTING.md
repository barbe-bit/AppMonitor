# 贡献指南

感谢您对 AppMonitor 的兴趣！我们欢迎各种形式的贡献。

## 如何贡献

### 报告问题

如果您发现 bug 或有功能建议，请创建 [Issue](https://github.com/yourusername/AppMonitor/issues)。

报告时请包含：
- 清晰的标题和描述
- 复现步骤
- 预期行为 vs 实际行为
- 环境信息（操作系统、.NET 版本等）

### 提交代码

1. Fork 本仓库
2. 创建特性分支：
   ```bash
   git checkout -b feature/your-feature-name
   # 或
   git checkout -b fix/your-bug-fix
   ```
3. 进行开发，确保代码符合项目规范
4. 编写测试（如果有）
5. 提交更改：
   ```bash
   git commit -m "Add: 添加新功能描述"
   git commit -m "Fix: 修复问题描述"
   ```
6. 推送到您的 Fork：
   ```bash
   git push origin feature/your-feature-name
   ```
7. 创建 Pull Request

### 代码规范

- 遵循 .NET 编码规范
- 使用有意义的变量和方法名
- 添加必要的注释
- 确保代码可以通过编译

### 提交信息规范

```
<type>(<scope>): <subject>

可选的正文

可选的页脚
```

类型 (type):
- `Add`: 新功能
- `Fix`: 修复 bug
- `Update`: 更新现有功能
- `Refactor`: 代码重构
- `Docs`: 文档更新
- `Style`: 代码格式调整
- `Test`: 测试相关
- `Chore`: 构建/工具相关

## 开发环境

```bash
# 克隆项目
git clone https://github.com/barbe-bit/AppMonitor.git
cd AppMonitor

# 安装依赖
dotnet restore

# 开发模式运行
dotnet watch run --project JavaMonitor.Web

# 运行测试
dotnet test

# 构建
dotnet build
```

## 许可证

通过提交代码，您同意您的贡献将在 [Apache 2.0](LICENSE) 许可证下发布。
