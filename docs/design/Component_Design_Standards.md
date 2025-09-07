# FlexToDo 组件设计规范

## 1. 组件设计原则

### 1.1 设计理念
- **一致性**: 所有组件遵循统一的视觉语言
- **可复用性**: 组件设计支持多种场景使用
- **可访问性**: 符合WCAG 2.1 AA标准
- **性能优化**: 轻量级实现，避免过度渲染

### 1.2 组件分类
- **基础组件**: Button, Input, Icon, Badge
- **布局组件**: Container, Card, Divider
- **交互组件**: Dropdown, Tooltip, Modal
- **业务组件**: TaskItem, TaskList, QuickAdd

## 2. 基础组件规范

### 2.1 按钮组件 (Button)

#### 主要按钮 (Primary Button)
```xml
<Button x:Name="PrimaryButton" 
        Style="{StaticResource PrimaryButtonStyle}">
  <TextBlock Text="确认操作"/>
</Button>
```

**视觉规格**:
```css
.btn-primary {
  background: linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%);
  color: #FFFFFF;
  border: none;
  border-radius: 6px;
  padding: 8px 16px;
  font-size: 12px;
  font-weight: 500;
  min-width: 64px;
  height: 32px;
  
  /* 阴影 */
  box-shadow: 0 2px 4px rgba(37, 99, 235, 0.2);
}

.btn-primary:hover {
  background: linear-gradient(135deg, #1D4ED8 0%, #1E40AF 100%);
  box-shadow: 0 4px 8px rgba(37, 99, 235, 0.3);
  transform: translateY(-1px);
}

.btn-primary:active {
  transform: translateY(0);
  box-shadow: 0 1px 2px rgba(37, 99, 235, 0.2);
}

.btn-primary:disabled {
  background: #9CA3AF;
  box-shadow: none;
  cursor: not-allowed;
}
```

#### 次要按钮 (Secondary Button)
```css
.btn-secondary {
  background: rgba(255, 255, 255, 0.8);
  color: #4B5563;
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  padding: 8px 16px;
  font-size: 12px;
  font-weight: 500;
}

.btn-secondary:hover {
  background: rgba(249, 250, 251, 0.9);
  border-color: #9CA3AF;
}
```

#### 图标按钮 (Icon Button)
```css
.btn-icon {
  background: transparent;
  border: none;
  border-radius: 4px;
  padding: 6px;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-icon:hover {
  background: rgba(37, 99, 235, 0.1);
}
```

### 2.2 输入框组件 (Input)

#### 文本输入框
```xml
<TextBox x:Name="StandardInput"
         Style="{StaticResource StandardInputStyle}"
         Watermark="请输入任务标题..."/>
```

**视觉规格**:
```css
.input-text {
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  padding: 8px 12px;
  font-size: 13px;
  line-height: 18px;
  background: rgba(255, 255, 255, 0.9);
  color: #1F2937;
  min-height: 36px;
  
  /* 占位符样式 */
  ::placeholder {
    color: #9CA3AF;
    font-style: italic;
  }
}

.input-text:focus {
  outline: none;
  border-color: #2563EB;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  background: rgba(255, 255, 255, 1);
}

.input-text:error {
  border-color: #DC2626;
  box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1);
}
```

#### 搜索输入框
```css
.input-search {
  position: relative;
  
  /* 左侧图标 */
  &::before {
    content: "🔍";
    position: absolute;
    left: 12px;
    top: 50%;
    transform: translateY(-50%);
    color: #9CA3AF;
  }
  
  input {
    padding-left: 36px;
    border-radius: 20px;
  }
}
```

### 2.3 图标组件 (Icon)

#### 图标规格
```css
.icon {
  width: 16px;
  height: 16px;
  fill: currentColor;
  stroke: currentColor;
  stroke-width: 1.5px;
  stroke-linecap: round;
  stroke-linejoin: round;
}

.icon-small { width: 14px; height: 14px; }
.icon-large { width: 20px; height: 20px; }
.icon-xl { width: 24px; height: 24px; }
```

#### 常用图标清单
```
功能图标:
- plus: 添加 ➕
- minus: 删除 ➖
- edit-3: 编辑 ✏️
- check: 完成 ✅
- x: 关闭 ❌
- search: 搜索 🔍
- settings: 设置 ⚙️
- more-horizontal: 更多 ⋯

状态图标:
- clock: 时间 🕐
- calendar: 日期 📅
- alert-circle: 警告 ⚠️
- info: 信息 ℹ️
- check-circle: 成功 ✅
- x-circle: 错误 ❌

优先级图标:
- arrow-up: 高优先级 ⬆️
- arrow-right: 中优先级 ➡️
- arrow-down: 低优先级 ⬇️
```

### 2.4 徽章组件 (Badge)

```css
.badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 500;
  line-height: 1;
  border-radius: 10px;
  padding: 2px 6px;
  min-width: 18px;
  height: 18px;
}

.badge-primary {
  background: #2563EB;
  color: #FFFFFF;
}

.badge-danger {
  background: #DC2626;
  color: #FFFFFF;
}

.badge-warning {
  background: #D97706;
  color: #FFFFFF;
}

.badge-success {
  background: #059669;
  color: #FFFFFF;
}

.badge-neutral {
  background: #6B7280;
  color: #FFFFFF;
}

/* 点状徽章 */
.badge-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  padding: 0;
  min-width: auto;
}
```

## 3. 布局组件规范

### 3.1 容器组件 (Container)

```css
.container {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
  overflow: hidden;
}

.container-small { padding: 12px; }
.container-medium { padding: 16px; }
.container-large { padding: 24px; }
```

### 3.2 卡片组件 (Card)

```xml
<Border x:Name="TaskCard" Style="{StaticResource TaskCardStyle}">
  <Grid>
    <!-- 卡片内容 -->
  </Grid>
</Border>
```

```css
.card {
  background: rgba(255, 255, 255, 0.8);
  border: 1px solid #E5E7EB;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 8px;
  transition: all 0.2s ease;
}

.card:hover {
  border-color: #2563EB;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.15);
  transform: translateY(-1px);
}

.card.selected {
  border-color: #2563EB;
  border-width: 2px;
  background: rgba(37, 99, 235, 0.05);
}
```

### 3.3 分割线组件 (Divider)

```css
.divider {
  border: none;
  border-top: 1px solid #E5E7EB;
  margin: 12px 0;
}

.divider-vertical {
  border-left: 1px solid #E5E7EB;
  border-top: none;
  height: 100%;
  margin: 0 12px;
}

.divider-text {
  position: relative;
  text-align: center;
  margin: 16px 0;
  
  &::before {
    content: "";
    position: absolute;
    top: 50%;
    left: 0;
    right: 0;
    height: 1px;
    background: #E5E7EB;
    z-index: -1;
  }
  
  span {
    background: rgba(255, 255, 255, 0.9);
    padding: 0 12px;
    font-size: 11px;
    color: #6B7280;
  }
}
```

## 4. 交互组件规范

### 4.1 工具提示 (Tooltip)

```css
.tooltip {
  position: absolute;
  z-index: 9999;
  background: rgba(0, 0, 0, 0.8);
  color: white;
  font-size: 11px;
  padding: 6px 8px;
  border-radius: 4px;
  white-space: nowrap;
  
  /* 箭头 */
  &::after {
    content: "";
    position: absolute;
    top: 100%;
    left: 50%;
    transform: translateX(-50%);
    border: 4px solid transparent;
    border-top-color: rgba(0, 0, 0, 0.8);
  }
}

/* 显示动画 */
.tooltip-enter {
  opacity: 0;
  transform: translateY(-4px);
}

.tooltip-enter-active {
  opacity: 1;
  transform: translateY(0);
  transition: all 0.15s ease-out;
}
```

### 4.2 紧急程度指示器 (UrgencyIndicator)

```css
.urgency-indicator {
  width: 4px;
  height: 100%;
  border-radius: 2px;
  margin-right: 8px;
  transition: all 200ms ease;
}

.urgency-critical {
  background: #DC2626;
}

.urgency-high {
  background: #D97706;
}

.urgency-medium {
  background: #059669;
}

.urgency-low {
  background: #E5E7EB;
}

/* 背景穿透模式下的紧急程度显示 */
.background-mode .urgency-indicator {
  opacity: 0.8;
}

/* 交互模式下的紧急程度显示 */
.interactive-mode .urgency-indicator {
  opacity: 1.0;
}
```

## 5. 业务组件规范

### 5.1 任务项组件 (TaskItem)

```xml
<UserControl x:Class="FlexToDo.Controls.TaskItem">
  <Border Style="{StaticResource TaskItemBorderStyle}">
    <Grid>
      <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>  <!-- 复选框 -->
        <ColumnDefinition Width="*"/>     <!-- 内容 -->
        <ColumnDefinition Width="Auto"/>  <!-- 操作 -->
      </Grid.ColumnDefinitions>
      
      <!-- 内容布局 -->
    </Grid>
  </Border>
</UserControl>
```

**组件结构**:
```
┌─────────────────────────────────────────────┐
│ ☐ [任务标题]              [时间] [优先级] [⋯] │
│   [任务描述 (可选)]                          │
│   [标签1] [标签2] ...                       │
└─────────────────────────────────────────────┘
```

**状态样式**:
```css
/* 默认状态 */
.task-item {
  padding: 12px 16px;
  border: 1px solid transparent;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.6);
  margin-bottom: 4px;
}

/* 悬浮状态 */
.task-item:hover {
  background: rgba(37, 99, 235, 0.05);
  border-color: rgba(37, 99, 235, 0.2);
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.1);
}

/* 选中状态 */
.task-item.selected {
  background: rgba(37, 99, 235, 0.1);
  border-color: #2563EB;
}

/* 完成状态 */
.task-item.completed {
  opacity: 0.6;
  background: rgba(5, 150, 105, 0.05);
  
  .task-title {
    text-decoration: line-through;
    color: #6B7280;
  }
}

/* 过期状态 */
.task-item.overdue {
  border-left: 4px solid #DC2626;
  background: rgba(220, 38, 38, 0.05);
}
```

**优先级样式**:
```css
.priority-indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 8px;
}

.priority-high { background: #DC2626; }
.priority-medium { background: #D97706; }
.priority-low { background: #6B7280; }
.priority-none { background: #E5E7EB; }
```

### 5.2 快速添加组件 (QuickAdd)

```xml
<Border x:Name="QuickAddContainer" Style="{StaticResource QuickAddStyle}">
  <Grid>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="Auto"/>
      <ColumnDefinition Width="*"/>
      <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    
    <TextBlock Grid.Column="0" Text="+" FontSize="16" Margin="8,0"/>
    <TextBox Grid.Column="1" 
             x:Name="QuickAddInput"
             Text="{Binding QuickAddText}"
             PlaceholderText="快速添加任务..."/>
    <Button Grid.Column="2" Content="添加" 
            Command="{Binding AddTaskCommand}"/>
  </Grid>
</Border>
```

```css
.quick-add {
  background: rgba(37, 99, 235, 0.05);
  border: 1px solid rgba(37, 99, 235, 0.2);
  border-radius: 6px;
  padding: 6px;
  margin: 8px 0;
  transition: all 150ms ease;
}

/* 仅在交互模式下可用 */
.interactive-mode .quick-add {
  pointer-events: auto;
}

.interactive-mode .quick-add:hover {
  background: rgba(37, 99, 235, 0.08);
  border-color: rgba(37, 99, 235, 0.4);
}

.interactive-mode .quick-add:focus-within {
  border-color: #2563EB;
  box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.1);
}

.quick-add-input {
  border: none;
  background: transparent;
  font-size: 13px;
  padding: 4px 8px;
}
```

## 6. 状态管理

### 6.1 背景穿透状态
```css
.background-mode {
  opacity: 0.7;
  pointer-events: none;
  cursor: default;
}

/* 背景模式下的任务列表 */
.background-mode .task-list {
  pointer-events: none;
}

/* 所有交互元素在背景模式下禁用 */
.background-mode button,
.background-mode input,
.background-mode textarea {
  pointer-events: none;
  opacity: 0.6;
}
```

### 6.2 交互状态
```css
.interactive-mode {
  opacity: 0.95;
  pointer-events: auto;
  cursor: auto;
}

/* 交互模式下启用所有功能 */
.interactive-mode .task-list {
  pointer-events: auto;
}

.interactive-mode button,
.interactive-mode input,
.interactive-mode textarea {
  pointer-events: auto;
  opacity: 1.0;
}
```

### 6.3 状态切换动画
```css
.state-transition {
  transition: all 200ms cubic-bezier(0.4, 0.0, 0.2, 1);
}

/* 切换到交互模式 */
.transitioning-to-interactive {
  animation: fadeToInteractive 200ms ease-out;
}

@keyframes fadeToInteractive {
  from { opacity: 0.7; }
  to { opacity: 0.95; }
}

/* 切换到背景模式 */
.transitioning-to-background {
  animation: fadeToBackground 200ms ease-in;
}

@keyframes fadeToBackground {
  from { opacity: 0.95; }
  to { opacity: 0.7; }
}
```

## 7. 动画规范

### 7.1 基础过渡
```css
/* 标准过渡 */
.transition-standard {
  transition: all 0.2s cubic-bezier(0.4, 0.0, 0.2, 1);
}

/* 快速过渡 */
.transition-fast {
  transition: all 0.15s ease-out;
}

/* 慢速过渡 */
.transition-slow {
  transition: all 0.3s ease-in-out;
}
```

### 7.2 特殊动画
```css
/* 弹性动画 */
.bounce-in {
  animation: bounceIn 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes bounceIn {
  0% {
    transform: scale(0.8);
    opacity: 0;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

/* 淡入动画 */
.fade-in {
  animation: fadeIn 0.2s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
```

## 8. 实现建议

### 8.1 WPF样式资源
创建统一的资源字典：
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- 颜色资源 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#2563EB"/>
    <SolidColorBrush x:Key="DangerBrush" Color="#DC2626"/>
    
    <!-- 按钮样式 -->
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
        <!-- 样式定义 -->
    </Style>
    
</ResourceDictionary>
```

### 8.2 自定义控件
对于复杂组件，创建自定义UserControl：
```csharp
public partial class TaskItem : UserControl
{
    public static readonly DependencyProperty TaskProperty =
        DependencyProperty.Register("Task", typeof(TodoTask), typeof(TaskItem));
    
    public TodoTask Task
    {
        get { return (TodoTask)GetValue(TaskProperty); }
        set { SetValue(TaskProperty, value); }
    }
}
```

---

这个组件设计规范确保了FlexToDo的所有UI元素都遵循统一的设计语言，同时提供了详细的实现指导。