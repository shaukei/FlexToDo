# FlexToDo 交互动画规范

## 1. 动画设计原则

### 1.1 设计理念
- **有意义的动作**: 每个动画都有明确的功能目的
- **自然流畅**: 遵循现实物理规律，提供直观体验
- **性能优先**: 保持60FPS流畅度，避免影响应用性能
- **可访问性**: 考虑用户偏好，支持减少动画选项

### 1.2 动画分类
- **状态转换**: 背景穿透模式 ↔ 交互模式切换
- **微交互**: 按钮点击、任务项悬浮反馈
- **通知动画**: 托盘通知、成功/失败提示
- **紧急程度显示**: 颜色编码和边框动画

### 1.3 动画时长标准
```css
/* 微交互 */
--duration-instant: 100ms;    /* 按钮点击 */
--duration-fast: 150ms;       /* 悬浮效果 */
--duration-standard: 250ms;   /* 标准过渡 */
--duration-slow: 350ms;       /* 复杂状态切换 */
--duration-deliberate: 500ms; /* 重要提示 */

/* 缓动函数 */
--easing-standard: cubic-bezier(0.4, 0.0, 0.2, 1);
--easing-accelerate: cubic-bezier(0.4, 0.0, 1, 1);
--easing-decelerate: cubic-bezier(0.0, 0.0, 0.2, 1);
--easing-bounce: cubic-bezier(0.34, 1.56, 0.64, 1);
```

## 2. 状态转换动画

### 2.1 背景穿透模式 ↔ 交互模式

#### 切换到交互模式 (背景穿透 → 交互)
```css
/* 透明度切换动画 */
.transition-to-interactive {
  animation: fadeToInteractive 200ms cubic-bezier(0.4, 0.0, 0.2, 1);
}

@keyframes fadeToInteractive {
  0% {
    opacity: 0.7;
    pointer-events: none;
  }
  100% {
    opacity: 0.95;
    pointer-events: auto;
  }
}

/* 任务项恢复交互能力 */
.task-item {
  transition: all 200ms ease;
}

.interactive-mode .task-item {
  cursor: pointer;
}

.interactive-mode .task-item:hover {
  background: rgba(37, 99, 235, 0.05);
  transform: translateY(-1px);
}
```

#### 切换到背景穿透模式 (交互 → 背景穿透)
```css
.transition-to-background {
  animation: fadeToBackground 200ms cubic-bezier(0.4, 0.0, 1, 1);
}

@keyframes fadeToBackground {
  0% {
    opacity: 0.95;
    pointer-events: auto;
  }
  100% {
    opacity: 0.7;
    pointer-events: none;
  }
}

/* 任务项禁用交互 */
.background-mode .task-item {
  cursor: default;
  pointer-events: none;
}

.background-mode .task-item:hover {
  background: transparent;
  transform: none;
}
```

### 2.2 状态指示动画

#### 系统托盘状态变化
```css
/* 托盘图标正常状态 */
.tray-icon-normal {
  background: #2563EB;
  opacity: 0.8;
}

/* 有紧急任务时的托盘图标 */
.tray-icon-urgent {
  background: #DC2626;
  animation: urgentPulse 1.5s ease-in-out infinite;
}

@keyframes urgentPulse {
  0%, 100% {
    opacity: 0.8;
    transform: scale(1);
  }
  50% {
    opacity: 1.0;
    transform: scale(1.1);
  }
}
```

#### 紧急程度边框动画
```css
/* 紧急任务边框动画 */
.task-item.critical {
  border-left: 4px solid #DC2626;
  animation: criticalBorder 2s ease-in-out infinite;
}

@keyframes criticalBorder {
  0%, 100% {
    border-left-color: #DC2626;
    box-shadow: none;
  }
  50% {
    border-left-color: #EF4444;
    box-shadow: -4px 0 8px rgba(220, 38, 38, 0.2);
  }
}

/* 高优先级任务轻微高亮 */
.task-item.high {
  border-left: 4px solid #D97706;
  animation: highBorder 3s ease-in-out infinite;
}

@keyframes highBorder {
  0%, 100% {
    border-left-color: #D97706;
  }
  50% {
    border-left-color: #F59E0B;
  }
}
```

## 3. 微交互动画

### 3.1 按钮交互

#### 主要按钮动画
```css
.btn-primary {
  transition: all 150ms ease;
  background: #2563EB;
}

/* 仅在交互模式下可交互 */
.interactive-mode .btn-primary:hover {
  background: #1D4ED8;
  transform: translateY(-1px);
}

.interactive-mode .btn-primary:active {
  transform: translateY(0);
  background: #1E40AF;
  transition-duration: 100ms;
}

/* 背景模式下按钮不可交互 */
.background-mode .btn-primary {
  pointer-events: none;
  opacity: 0.6;
}
```

#### 状态切换按钮动画
```css
/* ESC键切换提示 */
.state-toggle-hint {
  opacity: 0;
  transform: translateY(4px);
  transition: all 200ms ease;
}

.interactive-mode .state-toggle-hint {
  opacity: 0.7;
  transform: translateY(0);
}

/* 热键反馈动画 */
.hotkey-feedback {
  animation: hotkeyPressed 300ms ease-out;
}

@keyframes hotkeyPressed {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.02);
    opacity: 0.9;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}
```

### 3.2 任务项交互

#### 复选框动画
```css
.checkbox {
  width: 16px;
  height: 16px;
  border: 2px solid #D1D5DB;
  border-radius: 3px;
  transition: all 200ms ease;
}

/* 仅在交互模式下可点击 */
.interactive-mode .checkbox {
  cursor: pointer;
}

.interactive-mode .checkbox:hover {
  border-color: #059669;
}

.checkbox.checked {
  border-color: #059669;
  background: #059669;
  animation: checkboxComplete 200ms ease-out;
}

@keyframes checkboxComplete {
  0% {
    transform: scale(0.9);
    opacity: 0.8;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

/* 对勾显示 */
.checkbox::after {
  content: '\2713';
  position: absolute;
  top: -1px;
  left: 2px;
  color: white;
  font-size: 12px;
  opacity: 0;
  transition: opacity 150ms ease;
}

.checkbox.checked::after {
  opacity: 1;
}
```

#### 任务完成动画
```css
.task-item.completing {
  animation: taskComplete 300ms ease-out;
}

@keyframes taskComplete {
  0% {
    opacity: 1;
    transform: scale(1);
  }
  50% {
    opacity: 0.8;
    transform: scale(1.01);
    background: rgba(5, 150, 105, 0.05);
  }
  100% {
    opacity: 0.6;
    transform: scale(1);
  }
}

/* 完成后的删除线效果 */
.task-item.completed .task-title {
  text-decoration: line-through;
  color: #9CA3AF;
  transition: all 200ms ease;
}

/* 完成任务的边框变化 */
.task-item.completed {
  border-left-color: #10B981;
  background: rgba(16, 185, 129, 0.02);
}
```

### 3.3 悬浮效果

#### 卡片悬浮
```css
.card {
  transition: all 200ms cubic-bezier(0.4, 0.0, 0.2, 1);
  transform: translateY(0);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(37, 99, 235, 0.12);
}

/* 内容微移动 */
.card:hover .card-content {
  transform: translateY(-1px);
}
```

#### 提示工具动画
```css
.tooltip {
  opacity: 0;
  transform: translateY(-8px) scale(0.9);
  transition: all 150ms cubic-bezier(0.0, 0.0, 0.2, 1);
  pointer-events: none;
}

.tooltip.show {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* 延迟显示避免误触 */
.tooltip-trigger:hover .tooltip {
  animation: tooltipDelay 150ms ease-out 500ms both;
}

@keyframes tooltipDelay {
  0% { opacity: 0; visibility: hidden; }
  100% { opacity: 1; visibility: visible; }
}
```

## 4. 加载动画

### 4.1 加载指示器

#### 旋转加载
```css
.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #E5E7EB;
  border-top: 2px solid #2563EB;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
```

#### 脉冲加载
```css
.loading-pulse {
  width: 40px;
  height: 40px;
  background: #2563EB;
  border-radius: 50%;
  animation: pulse 1.5s ease-in-out infinite;
}

@keyframes pulse {
  0% {
    transform: scale(0.8);
    opacity: 1;
  }
  50% {
    transform: scale(1.2);
    opacity: 0.5;
  }
  100% {
    transform: scale(0.8);
    opacity: 1;
  }
}
```

#### 骨架屏动画
```css
.skeleton {
  background: linear-gradient(
    90deg,
    #f0f0f0 25%,
    #e0e0e0 50%,
    #f0f0f0 75%
  );
  background-size: 200% 100%;
  animation: shimmer 2s ease-in-out infinite;
}

@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
```

### 4.2 数据加载状态

#### 列表项依次加载
```css
.list-item {
  opacity: 0;
  transform: translateY(20px);
}

.list-item.loaded {
  animation: slideInStagger 300ms ease-out both;
}

/* 错开动画 */
.list-item:nth-child(1) { animation-delay: 0ms; }
.list-item:nth-child(2) { animation-delay: 100ms; }
.list-item:nth-child(3) { animation-delay: 200ms; }
.list-item:nth-child(4) { animation-delay: 300ms; }
.list-item:nth-child(5) { animation-delay: 400ms; }

@keyframes slideInStagger {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

## 5. 通知动画

### 5.1 成功消息
```css
.notification-success {
  background: rgba(5, 150, 105, 0.1);
  border-left: 4px solid #059669;
  animation: slideInRight 300ms ease-out, 
             fadeOut 300ms ease-in 2700ms both;
}

@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

/* 成功图标动画 */
.success-icon {
  animation: successPop 400ms cubic-bezier(0.34, 1.56, 0.64, 1) 100ms both;
}

@keyframes successPop {
  0% { transform: scale(0); }
  50% { transform: scale(1.2); }
  100% { transform: scale(1); }
}
```

### 5.2 错误消息
```css
.notification-error {
  background: rgba(220, 38, 38, 0.1);
  border-left: 4px solid #DC2626;
  animation: shake 400ms ease-in-out;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  10%, 30%, 50%, 70%, 90% { transform: translateX(-4px); }
  20%, 40%, 60%, 80% { transform: translateX(4px); }
}
```

## 6. 手势动画

### 6.1 滑动操作
```css
.swipe-action {
  transition: transform 150ms ease-out;
}

.swipe-action.swipe-left {
  animation: swipeReveal 250ms ease-out;
}

@keyframes swipeReveal {
  0% { transform: translateX(0); }
  100% { transform: translateX(-80px); }
}

/* 显示操作按钮 */
.swipe-actions {
  transform: translateX(100%);
  transition: transform 250ms ease-out;
}

.swipe-action.swipe-left .swipe-actions {
  transform: translateX(0);
}
```

### 6.2 长按动画
```css
.long-press {
  position: relative;
}

.long-press::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(37, 99, 235, 0.1);
  border-radius: inherit;
  transform: scale(0);
  animation: longPressGrow 800ms ease-out;
}

@keyframes longPressGrow {
  to { transform: scale(1); }
}
```

## 7. WPF动画实现

### 7.1 Storyboard示例
```xml
<UserControl.Resources>
  <!-- 展开动画 -->
  <Storyboard x:Key="ExpandAnimation">
    <DoubleAnimation
      Storyboard.TargetProperty="Width"
      From="30" To="200" Duration="0:0:0.2"
      EasingFunction="{StaticResource QuadraticEase}"/>
    
    <DoubleAnimation
      Storyboard.TargetProperty="Height"
      From="30" To="100" Duration="0:0:0.2"
      EasingFunction="{StaticResource QuadraticEase}"/>
    
    <DoubleAnimation
      Storyboard.TargetProperty="Opacity"
      From="0.7" To="1" Duration="0:0:0.15"
      BeginTime="0:0:0.05"/>
  </Storyboard>
</UserControl.Resources>
```

### 7.2 代码触发动画
```csharp
private void AnimateToPreview()
{
    var sb = new Storyboard();
    
    // 宽度动画
    var widthAnimation = new DoubleAnimation
    {
        From = 30,
        To = 200,
        Duration = TimeSpan.FromMilliseconds(200),
        EasingFunction = new QuadraticEase()
    };
    Storyboard.SetTarget(widthAnimation, this);
    Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
    
    // 高度动画
    var heightAnimation = new DoubleAnimation
    {
        From = 30,
        To = 100,
        Duration = TimeSpan.FromMilliseconds(200),
        EasingFunction = new QuadraticEase()
    };
    Storyboard.SetTarget(heightAnimation, this);
    Storyboard.SetTargetProperty(heightAnimation, new PropertyPath("Height"));
    
    sb.Children.Add(widthAnimation);
    sb.Children.Add(heightAnimation);
    sb.Begin();
}
```

### 7.3 自定义缓动函数
```csharp
public class BounceEase : EasingFunctionBase
{
    protected override double EaseInCore(double normalizedTime)
    {
        if (normalizedTime < 1 / 2.75)
        {
            return 7.5625 * normalizedTime * normalizedTime;
        }
        else if (normalizedTime < 2 / 2.75)
        {
            return 7.5625 * (normalizedTime -= 1.5 / 2.75) * normalizedTime + 0.75;
        }
        else if (normalizedTime < 2.5 / 2.75)
        {
            return 7.5625 * (normalizedTime -= 2.25 / 2.75) * normalizedTime + 0.9375;
        }
        else
        {
            return 7.5625 * (normalizedTime -= 2.625 / 2.75) * normalizedTime + 0.984375;
        }
    }
}
```

## 8. 性能优化

### 8.1 GPU加速
```css
.gpu-accelerated {
  will-change: transform, opacity;
  backface-visibility: hidden;
  transform: translateZ(0); /* 强制GPU渲染 */
}
```

### 8.2 动画性能监控
```javascript
// 监控帧率
let lastTime = performance.now();
let frameCount = 0;

function measureFPS() {
  frameCount++;
  const currentTime = performance.now();
  
  if (currentTime - lastTime >= 1000) {
    const fps = Math.round((frameCount * 1000) / (currentTime - lastTime));
    console.log(`FPS: ${fps}`);
    
    frameCount = 0;
    lastTime = currentTime;
  }
  
  requestAnimationFrame(measureFPS);
}
```

### 8.3 减少动画选项
```css
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

## 9. 调试和测试

### 9.1 动画调试工具
```css
/* 开发模式显示边界 */
.debug-animations * {
  outline: 1px solid rgba(255, 0, 0, 0.3) !important;
  animation-play-state: paused !important;
}

/* 慢动作模式 */
.slow-motion * {
  animation-duration: calc(var(--original-duration, 250ms) * 5) !important;
  transition-duration: calc(var(--original-duration, 250ms) * 5) !important;
}
```

### 8.2 A/B测试动画效果
```csharp
public enum AnimationVariant
{
    Standard,
    Bouncy,
    Smooth,
    Instant
}

private void ApplyAnimationVariant(AnimationVariant variant)
{
    switch (variant)
    {
        case AnimationVariant.Bouncy:
            // 使用弹性缓动
            break;
        case AnimationVariant.Smooth:
            // 使用平滑过渡
            break;
        // ...
    }
}
```

---

这个动画规范确保了FlexToDo具有流畅、直观、性能优化的交互体验，每个动画都有明确的目的和技术实现方案。