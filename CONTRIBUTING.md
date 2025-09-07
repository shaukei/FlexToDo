# 贡献指南 🤝

感谢您对 FlexToDo 项目的关注！我们欢迎所有形式的贡献，无论是代码、文档、设计还是反馈。

## 🌟 贡献类型

### 代码贡献
- 🐛 **Bug 修复**：修复现有功能的问题
- ✨ **新功能开发**：实现 [TODO.md](TODO.md) 中规划的功能  
- ⚡ **性能优化**：提升应用性能和资源使用效率
- 🧪 **测试覆盖**：添加单元测试和集成测试

### 非代码贡献
- 📚 **文档改进**：完善使用指南、API文档
- 🎨 **UI/UX 优化**：界面设计和用户体验改进
- 🌍 **国际化**：多语言支持
- 💡 **功能建议**：通过 Issues 提出新的功能想法

## 🚀 快速开始

### 1. Fork 和 Clone

```bash
# Fork 本项目到您的 GitHub 账户
# 然后 clone 您的 fork

git clone https://github.com/YOUR-USERNAME/FlexToDo.git
cd FlexToDo
```

### 2. 设置开发环境

**环境要求**：
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio 2022 或 VS Code

**构建项目**：
```bash
# 恢复 NuGet 包
dotnet restore

# 构建解决方案
dotnet build

# 运行应用
dotnet run --project src/FlexToDo.App
```

### 3. 创建分支

```bash
# 创建功能分支
git checkout -b feature/your-feature-name

# 或修复分支
git checkout -b fix/issue-number-description
```

## 📝 开发规范

### 代码风格

1. **C# 编码规范**
   - 遵循 Microsoft C# 编码约定
   - 使用 PascalCase 命名类、方法、属性
   - 使用 camelCase 命名局部变量、字段
   - 优先使用英文命名和注释

2. **MVVM 模式**
   - 视图层 (Views) 只包含 UI 逻辑
   - 业务逻辑放在 ViewModels 和 Services 中
   - 使用 ICommand 处理用户交互

3. **异步编程**
   - 优先使用 async/await 模式
   - 避免 Task.Result 和 .Wait() 调用
   - 使用 ConfigureAwait(false) 优化性能

### 项目结构

遵循现有的分层架构：
```
src/
├── FlexToDo.Core/        # 业务逻辑层
├── FlexToDo.UI/          # 用户界面层
├── FlexToDo.Infrastructure/  # 基础设施层
└── FlexToDo.App/         # 应用程序入口
```

### 提交规范

使用清晰的提交信息：
```bash
# 功能添加
feat: 添加多行事项输入支持

# Bug 修复  
fix: 修复输入框聚焦时占位符不消失的问题

# 文档更新
docs: 更新安装指南

# 性能优化
perf: 优化窗口状态切换动画性能

# 代码重构
refactor: 重构热键管理器类结构
```

## 🧪 测试要求

### 单元测试
- 为新功能编写单元测试
- 确保测试覆盖率不低于 80%
- 使用 xUnit 测试框架

### 手动测试
在提交 PR 前，请确保：
- [ ] 应用正常启动和关闭
- [ ] 背景穿透模式工作正常
- [ ] 热键功能响应正确
- [ ] UI 在不同 DPI 下显示正常
- [ ] 内存占用保持在合理范围内

## 📋 提交 Pull Request

### PR 检查清单

提交前请确认：
- [ ] 代码通过所有现有测试
- [ ] 新功能包含相应测试
- [ ] 文档已更新（如有必要）
- [ ] 提交信息遵循规范
- [ ] 代码风格符合项目标准

### PR 描述模板

```markdown
## 📝 变更说明
简要描述此 PR 的目的和实现的功能。

## 🔧 技术实现
- 主要变更的文件和模块
- 技术方案说明

## ✅ 测试验证
- [ ] 单元测试通过
- [ ] 手动测试验证
- [ ] 性能测试验证（如适用）

## 📸 截图或演示
（如涉及 UI 变更，请提供截图或 GIF）

## 🔗 相关 Issue
Closes #issue-number
```

## 🐛 报告问题

### Issue 模板

**Bug 报告**：
```markdown
## 🐛 问题描述
清晰简洁地描述遇到的问题。

## 📋 复现步骤
1. 执行操作 A
2. 执行操作 B
3. 观察到错误

## 💻 环境信息
- 操作系统：Windows 10/11
- .NET 版本：8.0
- FlexToDo 版本：v1.0

## 📸 截图或日志
（如果适用）
```

**功能请求**：
```markdown
## ✨ 功能描述
描述希望添加的功能。

## 🎯 使用场景
说明此功能的使用场景和价值。

## 💡 实现建议
（可选）您认为如何实现这个功能。
```

## 🤖 人机协作开发

FlexToDo 采用人机协作开发模式。如果您对这种开发方式感兴趣：

1. **参与讨论**：在 [Discussions](../../discussions) 中分享您的想法
2. **流程改进**：提出协作开发流程的改进建议  
3. **经验交流**：分享您在 AI 辅助编程方面的经验

查看 [AI_COLLABORATION.md](AI_COLLABORATION.md) 了解协作过程记录。

## 📞 获得帮助

如果您在贡献过程中遇到问题：

- 📋 **GitHub Issues**：技术问题和 Bug 报告
- 💬 **GitHub Discussions**：开放式讨论和经验分享
- 📖 **文档**：查看项目文档了解更多信息

## 🎉 致谢

感谢每一位贡献者的参与。

感谢：
- 提交代码的贡献者
- 报告问题和提供反馈的用户
- 参与讨论和分享经验的社区成员

## 📄 许可证

通过贡献代码，您同意您的贡献将在 [MIT 许可证](LICENSE) 下发布。

---

感谢您的贡献。