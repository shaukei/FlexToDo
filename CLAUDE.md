# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
FlexToDo - 轻量级悬浮待办事项工具

FlexToDo是一个现代化的Windows桌面待办事项应用，采用半透明悬浮窗设计，支持背景穿透模式和交互模式的无缝切换。项目目标是打造一个不干扰工作流程的"背景水印式"待办提醒工具。

## Current Status (v1.0 里程碑版本)
✅ **已完成的核心功能**:
- 二元状态切换系统（背景穿透模式 ↔ 交互模式）
- 280px固定宽度半透明悬浮窗口（透明度0.7/0.95）
- Win32点击穿透技术实现
- 热键系统（Alt+Ctrl+Space激活窗口交互）
- 4级紧急程度待办事项管理（Critical/High/Medium/Low）
- 完整的任务列表界面始终可见
- 系统托盘集成和通知功能
- 10秒自动返回背景穿透机制

## Development Environment
- **Platform**: Windows
- **Framework**: WPF (.NET 8)
- **Language**: C# 12
- **Architecture**: MVVM模式 + 依赖注入
- **Git Repository**: 已初始化

## Project Structure
```
FlexToDo/
├── src/
│   ├── FlexToDo.App/           # 应用程序入口
│   ├── FlexToDo.UI/            # 用户界面层
│   ├── FlexToDo.Core/          # 核心业务逻辑
│   └── FlexToDo.Infrastructure/ # 基础设施层
├── docs/                       # 项目文档
│   ├── design/                 # UI/UX设计规范
│   ├── technical/              # 技术实现指南  
│   └── archive/                # 历史开发文档
├── TODO.md                     # 功能开发计划
└── README.md                   # 项目说明
```

## Commands

### Build and Run
```bash
# 构建项目
dotnet build

# 运行应用
dotnet run --project src/FlexToDo.App

# 发布单文件版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Development Tools
```bash
# 代码格式化
dotnet format

# 运行测试（如果有）
dotnet test
```

## Architecture

### Core Components
- **WindowStateController**: 窗口状态管理（背景穿透↔交互模式）
- **GlobalHotkeyManager**: 全局热键管理
- **NotifyIconService**: 系统托盘服务
- **TodoService**: 待办事项业务逻辑

### Key Technologies
- **Win32 API**: 点击穿透和窗口属性控制
- **WPF动画**: 透明度切换动画（200ms渐变）
- **MVVM绑定**: 数据驱动的界面更新
- **依赖注入**: 模块化组件管理

## Development Guidelines

### Code Style
- 优先使用英文注释和变量名
- 遵循C#编码规范
- 保持代码整洁和可维护性
- 使用异步编程模式（async/await）

### UI/UX Principles
- **简洁优先**: 避免复杂的界面元素
- **不干扰工作流**: 背景穿透模式下完全透明操作
- **快速交互**: 热键激活，10秒自动返回
- **视觉层次**: 4级紧急程度颜色编码

### Technical Constraints
- Windows平台专用（Win32 API依赖）
- 固定280px宽度设计
- 内存占用目标：<10MB
- 启动时间目标：<3秒

## Current Priority Tasks
参考 `TODO.md` 文件获取最新的功能开发计划。

高优先级功能包括：
1. 鼠标悬停突显效果 + 点击穿透
2. 穿透状态界面简化
3. 界面名称更新
4. 界面用户体验优化（输入框聚焦bug修复）
5. 事项操作功能完善

## Notes
- **语言**: 使用中文进行沟通（用户偏好设置）
- **平台**: 确保所有命令和脚本支持Windows
- **Git提交**: 避免在提交信息中包含AI辅助相关信息
- **测试**: 在不同Windows版本上验证兼容性
- **性能**: 保持轻量级，避免资源占用过多