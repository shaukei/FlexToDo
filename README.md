# FlexToDo - 轻量级悬浮待办事项工具

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)

## 🎯 项目概述

FlexToDo 是一个基于 C# WPF 开发的悬浮待办事项应用，支持背景穿透模式和交互模式切换。

> 💡 **开发方式**：本项目采用人机协作开发模式，由开发者与 Claude Code Assistant 协作完成。开发过程包括需求分析、架构设计、代码实现和文档编写的完整协作流程。

### ✨ 核心特性

- **🌊 背景穿透模式**：280px固定宽度界面，0.7透明度，完全鼠标穿透，始终可见
- **⚡ 热键激活**：通过全局热键瞬间切换到交互模式(0.95透明度)，无需中断工作流
- **🔥 紧急提醒**：4级紧急程度颜色编码，托盘通知系统
- **💾 本地存储**：安全的JSON文件存储，支持自动备份和数据恢复
- **⚙️ 轻量运行**：内存占用<10MB，后台CPU占用接近0%

## 🎹 快捷键

| 快捷键 | 功能 |
|--------|------|
| `Ctrl+Alt+Space` | 切换主界面显示/隐藏 |
| `Ctrl+Alt+N` | 快速添加新待办事项 |
| `Ctrl+Alt+V` | 快速查看待办列表 |
| `Ctrl+Alt+C` | 清除已完成事项 |
| `Esc` | 退出交互模式 |

## 🏗️ 项目架构

### 模块划分

```
FlexToDo/
├── FlexToDo.Core/           # 核心业务逻辑
│   ├── Models/              # 数据模型 (TodoItem, UrgencyLevel)
│   ├── Services/            # 业务服务 (TodoService)
│   └── Interfaces/          # 服务接口
├── FlexToDo.Infrastructure/ # 基础设施层
│   ├── Win32/               # Win32 API封装
│   └── Storage/             # 本地存储服务
├── FlexToDo.UI/            # 用户界面层
│   ├── Views/               # XAML视图
│   ├── ViewModels/          # MVVM视图模型
│   └── Controls/            # 自定义控件
└── FlexToDo.App/           # 应用程序入口
    └── Services/            # 应用级服务 (托盘图标)
```

### 设计模式

- **MVVM模式**：视图与业务逻辑分离
- **依赖注入**：使用 Microsoft.Extensions.DependencyInjection
- **观察者模式**：状态变化和UI更新
- **命令模式**：热键和UI操作统一处理

## 🎨 界面设计

### 背景穿透模式
- 280px × 完整高度界面，0.7透明度，完全鼠标穿透
- Win32点击穿透机制，不干扰正常工作
- 完整待办列表始终可见，紧急程度颜色编码

### 交互模式
- 280px × 完整高度界面，0.95透明度
- 恢复正常鼠标交互，支持所有操作
- 任务列表按紧急程度排序显示
- 快速添加、编辑、完成任务功能

## ⚡ 技术实现

### 核心技术
- **框架**：.NET 8.0 + WPF
- **状态管理**：WindowStateController (背景模式 ↔ 交互模式)
- **热键系统**：GlobalHotkeyManager (Win32 API)
- **数据存储**：JSON本地文件 + 自动备份
- **UI效果**：WPF动画 + 透明窗口

### Win32 集成
- 窗口透明度和鼠标穿透
- 全局热键注册
- DPI感知支持
- 单实例应用

## 🚀 编译和运行

### 环境要求
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio 2022 (可选)

### 编译命令
```bash
# 编译项目
dotnet build

# 运行项目
dotnet run --project src/FlexToDo.App

# 发布单文件版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 📋 待办事项管理

### 自动紧急程度计算
- **紧急 (Critical)**：已过期或6小时内到期 (红色)
- **高优先级 (High)**：24小时内到期 (橙色)  
- **中优先级 (Medium)**：3天内到期 (黄色)
- **低优先级 (Low)**：3天以上 (绿色)

### 智能提醒
- 任务项颜色编码显示紧急程度
- 系统托盘图标状态指示
- 过期任务弹出通知提醒

## 🛡️ 数据安全

- 本地JSON文件存储，无需网络连接
- 自动备份机制，保留最近20个备份
- 数据完整性验证
- 损坏文件自动恢复

## 📚 项目文档

### 核心文档
- [功能开发计划](TODO.md) - 详细的功能规划和优先级
- [开发者指南](CLAUDE.md) - 项目架构和开发规范
- [快速开始](QUICKSTART.md) - 安装和使用指南

### 设计文档
- [UX分析报告](.docs/01_UX_Analysis_Report.md) - 用户体验设计分析
- [UI设计规范](.docs/02_UI_Design_Specification.md) - 界面设计标准
- [技术实现指南](.docs/03_Technical_Implementation_Guide.md) - 技术架构详解

## 🤖 人机协作开发

### 开发过程记录
本项目采用AI辅助编程方式开发，包含以下阶段：

- **🧠 需求分析阶段**：通过对话确定功能需求和技术约束
- **🏗️架构设计阶段**：确定技术栈选择和模块结构
- **💻 代码实现阶段**：协作编写应用代码
- **📋 项目管理阶段**：维护功能规划和文档
- **🔍 质量保证阶段**：代码审查和规范检查

### 协作方式说明
- **对话式开发**：通过交互对话明确需求和实现方案
- **文档同步维护**：代码变更时同步更新相关文档
- **技术方案讨论**：对不同技术选项进行分析和选择
- **代码质量检查**：遵循编码规范和设计模式

## 📈 开发进度

当前状态：**v1.0 里程碑版本已完成** ✅

核心功能全部实现，正在根据用户反馈持续优化。查看 [TODO.md](TODO.md) 了解后续开发计划。

## 🤝 贡献指南

### 如何贡献
1. **Fork** 本项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 **Pull Request**

### 贡献类型
- 🐛 Bug 修复
- ✨ 新功能开发  
- 📚 文档改进
- 🎨 UI/UX 优化
- ⚡ 性能优化
- 🧪 测试覆盖

欢迎任何形式的贡献，包括对人机协作开发方式的改进建议。

## 📋 开发计划

### 当前重点
- 完善 Windows 版本功能
- 收集用户反馈和需求
- 持续优化性能和体验

### 后续计划
- 考虑跨平台实现方案 (macOS, Linux)
- 记录和分享AI辅助编程经验
- 完善人机协作开发流程

## 📞 联系方式

- **Issues**: [GitHub Issues](../../issues) - 问题报告和功能建议
- **Discussions**: [GitHub Discussions](../../discussions) - 开放讨论和经验分享

## 📄 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件

---

### 🎨 设计理念

待办事项管理工具应当简洁实用，不干扰正常工作流程。FlexToDo 通过背景穿透机制实现始终可见但不阻碍操作的设计目标。

### 🤖 开发说明

本项目采用人机协作方式开发，开发过程中 Claude Code Assistant 参与了需求分析、代码编写和文档维护工作。

感谢所有关注和贡献本项目的开发者。