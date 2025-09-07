# FlexToDo 永远可见悬浮UI设计方案

## 设计理念
**简化的二元状态切换设计** - 始终可见的半透明待办列表，可在背景穿透和交互模式之间切换

## 1. 核心设计方案

### 固定位置待办列表设计
- **位置**：屏幕右侧边缘，垂直居中
- **尺寸**：280px固定宽度，始终保持不变
- **交互模式**：默认点击穿透，热键激活后可正常交互
- **视觉反馈**：通过事项颜色标记传达紧急程度，透明度变化表示交互状态

## 2. 界面状态设计

### 背景穿透模式（默认）
```
尺寸：280px固定宽度 × 自适应高度
位置：屏幕右侧边缘
透明度：70%（半透明）
交互性：完全点击穿透（Win32 WS_EX_TRANSPARENT）
显示内容：完整待办列表界面，始终可见
特色：列表内容始终展示，但不会干扰后面应用的操作
```

### 交互激活模式（热键触发）
```
尺寸：280px固定宽度 × 自适应高度
位置：屏幕右侧边缘（位置不变）
透明度：95%（高不透明度）
交互性：正常窗口操作（移除Win32穿透样式）
显示内容：完整待办列表和操作界面
自动返回：10秒无操作后自动返回背景穿透模式
```

## 3. 紧急程度视觉编码

### 紧急程度颜色编码
```javascript
const urgencyLevels = {
  critical: {
    color: '#ef4444',        // 红色 - 紧急（已过期或即将到期）
    textColor: '#ffffff',
    backgroundColor: 'rgba(239, 68, 68, 0.15)'
  },
  high: {
    color: '#f97316',        // 橙色 - 重要（24小时内）
    textColor: '#ffffff',
    backgroundColor: 'rgba(249, 115, 22, 0.15)'
  },
  medium: {
    color: '#eab308',        // 黄色 - 普通（3天内）
    textColor: '#ffffff',
    backgroundColor: 'rgba(234, 179, 8, 0.15)'
  },
  low: {
    color: '#22c55e',        // 绿色 - 低优先级（3天以上）
    textColor: '#ffffff',
    backgroundColor: 'rgba(34, 197, 94, 0.15)'
  }
}
```

### 事项显示逻辑
- 在列表中直接显示所有事项
- 使用相应颜色标记不同紧急程度
- 紧急事项排在前面，颜色更加显眼
- 已完成事项使用删除线和淡化显示

## 4. 动态提醒效果

### 即将到期动画（24小时内）
```css
@keyframes urgentPulse {
  0%, 100% { 
    opacity: 0.3; 
    transform: scale(1); 
  }
  50% { 
    opacity: 0.8; 
    transform: scale(1.02); 
  }
}
```

### 已过期闪烁
```css
@keyframes criticalBlink {
  0%, 50% { opacity: 0.9; }
  51%, 100% { opacity: 0.1; }
}
```

## 5. 激活后界面布局

### 主界面结构
```
┌─────────────────────────────┐
│ 🔥 紧急事项 (2)             │ ← 分组标题
├─────────────────────────────┤
│ • 完成项目提案              │ ← 事项内容
│   📅 2小时后到期 [延期] [完成] │ ← 操作按钮
├─────────────────────────────┤  
│ ⚡ 重要事项 (3) [折叠]      │ ← 可折叠分组
├─────────────────────────────┤
│ + 快速添加新事项...         │ ← 快速输入
└─────────────────────────────┘
```

### 信息优先级显示
1. **紧急事项**：永远显示，红色高亮
2. **重要事项**：优先显示，橙色标记  
3. **普通事项**：按时间排序，黄色标记
4. **低优先级**：折叠显示，绿色标记

## 6. 技术规格

### WPF窗口属性
```csharp
// 基本窗口设置
WindowStyle = WindowStyle.None;
AllowsTransparency = true;
Topmost = true;
ShowInTaskbar = false;
WindowState = WindowState.Normal;
ResizeMode = ResizeMode.NoResize;
Width = 280; // 固定宽度

// 背景穿透模式设置
Opacity = 0.7;
IsHitTestVisible = false;
var style = Win32Api.GetWindowLong(hwnd, Win32Api.GWL_EXSTYLE);
Win32Api.SetWindowLong(hwnd, Win32Api.GWL_EXSTYLE, 
    style | Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_TOOLWINDOW | Win32Api.WS_EX_NOACTIVATE);

// 交互模式设置
Opacity = 0.95;
IsHitTestVisible = true;
var style = Win32Api.GetWindowLong(hwnd, Win32Api.GWL_EXSTYLE);
Win32Api.SetWindowLong(hwnd, Win32Api.GWL_EXSTYLE, 
    (style & ~(Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_NOACTIVATE)) | Win32Api.WS_EX_TOOLWINDOW);
```

### 状态切换动画
```csharp
// 激活动画（透明度变化）
var activateAnimation = new DoubleAnimation
{
    From = 0.7,  // 从背景穿透模式的透明度
    To = 0.95,   // 到交互模式的透明度
    Duration = TimeSpan.FromMilliseconds(150),
    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
};

// 返回动画（透明度变化）
var collapseAnimation = new DoubleAnimation
{
    From = 0.95, // 从交互模式的透明度
    To = 0.7,    // 到背景穿透模式的透明度
    Duration = TimeSpan.FromMilliseconds(150),
    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
};
```

## 7. 热键系统设计

### 热键映射
- **Ctrl+Alt+Space**：激活/关闭主界面
- **Ctrl+Alt+N**：快速添加新事项
- **Ctrl+Alt+V**：快速查看待办
- **Esc**：退出交互模式

### 热键响应流程
1. 检测全局热键
2. 切换窗口属性（取消穿透）
3. 播放展开动画
4. 获取焦点开始交互
5. 用户操作完成后自动返回背景模式

## 8. 视觉设计细节

### 毛玻璃效果
```css
backdrop-filter: blur(12px);
background: rgba(17, 24, 39, 0.85);
border: 1px solid rgba(75, 85, 99, 0.3);
```

### 字体系统
```css
.title { font: 600 14px/20px 'Microsoft YaHei', sans-serif; }
.item { font: 400 13px/18px 'Microsoft YaHei', sans-serif; }
.time { font: 400 11px/16px 'Microsoft YaHei', sans-serif; }
.label { font: 500 10px/14px 'Microsoft YaHei', sans-serif; }
```

### 圆角和阴影
```css
border-radius: 12px 0 0 12px;
box-shadow: 
  0 20px 25px -5px rgba(0, 0, 0, 0.3),
  0 10px 10px -5px rgba(0, 0, 0, 0.1);
```

## 9. 响应式适配

### DPI感知
- 支持100%、125%、150%、200%缩放
- 自动调整字体大小和间距
- 保持4px边缘条在所有分辨率下的视觉一致性

### 多显示器支持
- 自动检测主显示器
- 支持显示器配置变化时的位置调整
- 记住用户最后的位置偏好

## 10. 无障碍设计

### 高对比度支持
- 支持Windows高对比度主题
- 提供颜色盲友好的替代方案
- 键盘导航完全支持

这个UI设计完美实现了"永远可见但不干扰"的核心需求，让用户能够随时了解待办事项的紧急程度，同时不会影响正常的工作流程。