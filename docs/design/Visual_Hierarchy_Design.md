# FlexToDo 视觉层次设计

## 1. 视觉层次原则

### 1.1 设计理念
- **渐进式披露**: 从最小化到详细状态，信息逐步展示
- **焦点引导**: 通过对比、颜色、尺寸引导用户注意力
- **功能优先**: 核心功能获得最高视觉权重
- **认知负荷最小化**: 避免信息过载，保持界面清爽

### 1.2 层次结构
```
Level 1: 主要操作 (Primary Actions)
Level 2: 重要信息 (Important Info)
Level 3: 次要操作 (Secondary Actions)
Level 4: 辅助信息 (Supporting Info)
Level 5: 背景元素 (Background Elements)
```

## 2. 背景穿透状态视觉层次

### 2.1 层次分析 (280px宽度，0.7透明度)
```
Level 1: 任务列表主内容
Level 2: 紧急程度颜色编码
Level 3: 底层背景和边框
```

### 2.2 视觉权重分配
```css
/* Level 1: 任务列表 (权重: 100%) */
.background-task-list {
  opacity: 0.7;
  width: 280px;
  pointer-events: none; /* 完全穿透 */
  z-index: background;
}

/* Level 2: 紧急任务高亮 (权重: 90%) */
.critical-task-highlight {
  background: rgba(220, 38, 38, 0.1);
  border-left: 4px solid #DC2626;
  font-weight: 600;
}

/* Level 3: 普通任务 (权重: 60%) */
.normal-task {
  opacity: 0.8;
  font-weight: 400;
}
```

## 3. 交互状态视觉层次

### 3.1 信息架构 (280px宽度，0.95透明度)
```
┌─────────────────────────────────┐
│ Level 2: 标题栏和操作按钮        │ ← 次要信息
├─────────────────────────────────┤
│ Level 1: 任务列表主区域         │ ← 最高优先级
│         可点击、编辑、完成      │
├─────────────────────────────────┤
│ Level 2: 快速添加输入框        │ ← 主要操作
└─────────────────────────────────┘
```

### 3.2 视觉对比策略

#### 颜色对比
```css
/* Level 1: 紧急任务 - 最高对比度 */
.task-urgent {
  color: #DC2626;
  font-weight: 600;
  background: rgba(220, 38, 38, 0.1);
  border-left: 3px solid #DC2626;
}

/* Level 1: 今日任务 - 高对比度 */
.task-today {
  color: #D97706;
  font-weight: 500;
  background: rgba(217, 119, 6, 0.08);
}

/* Level 3: 普通任务 - 低对比度 */
.task-normal {
  color: #6B7280;
  font-weight: 400;
  background: transparent;
}
```

#### 尺寸对比
```css
/* 主要信息 */
.primary-text {
  font-size: 13px;
  line-height: 18px;
}

/* 次要信息 */
.secondary-text {
  font-size: 11px;
  line-height: 14px;
}

/* 辅助信息 */
.caption-text {
  font-size: 10px;
  line-height: 12px;
  opacity: 0.7;
}
```

## 4. 状态切换视觉层次

### 4.1 二元状态对比
```
背景穿透模式     vs     交互模式
┌──────────────┐     ┌──────────────┐
│ 不可交互        │     │ 全功能交互   │
│ 0.7透明度       │     │ 0.95透明度  │
│ 鼠标穿透        │     │ 正常鼠标交互 │
│ 信息展示为主    │     │ 操作为主      │
└──────────────┘     └──────────────┘
```

### 4.2 切换动画设计

#### 透明度切换动画
```css
/* 背景穿透模式 */
.background-mode {
  opacity: 0.7;
  pointer-events: none;
  transition: opacity 200ms ease;
}

/* 交互模式 */
.interactive-mode {
  opacity: 0.95;
  pointer-events: auto;
  transition: opacity 200ms ease;
}

/* 状态切换动画 */
.state-transition {
  transition: all 200ms cubic-bezier(0.4, 0.0, 0.2, 1);
}
```

#### 紧急程度层次
```css
/* 紧急 Critical - 最高警示 */
.urgency-critical {
  color: #DC2626;
  background: rgba(220, 38, 38, 0.1);
  border-left: 4px solid #DC2626;
  font-weight: 600;
}

/* 高优先级 High - 中等警示 */
.urgency-high {
  color: #D97706;
  border-left: 4px solid #D97706;
  font-weight: 500;
}

/* 中优先级 Medium - 正常显示 */
.urgency-medium {
  color: #059669;
  border-left: 4px solid #059669;
  font-weight: 400;
}

/* 低优先级 Low - 低调显示 */
.urgency-low {
  color: #6B7280;
  border-left: 4px solid #E5E7EB;
  font-weight: 400;
  opacity: 0.8;
}
```

## 5. 交互可用性层次

### 5.1 交互状态对比
```css
/* 背景穿透模式 - 禁用交互 */
.background-mode {
  pointer-events: none;
  opacity: 0.7;
  cursor: default;
}

/* 交互模式 - 允许交互 */
.interactive-mode {
  pointer-events: auto;
  opacity: 0.95;
  cursor: pointer;
}

/* 任务项悬浮效果 */
.task-item:hover {
  background: rgba(37, 99, 235, 0.05);
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.1);
}
```

### 5.2 状态切换的视觉反馈
```css
/* 切换到交互模式 */
.switching-to-interactive {
  animation: fadeToInteractive 200ms ease-out;
}

@keyframes fadeToInteractive {
  from {
    opacity: 0.7;
    pointer-events: none;
  }
  to {
    opacity: 0.95;
    pointer-events: auto;
  }
}

/* 切换到背景模式 */
.switching-to-background {
  animation: fadeToBackground 200ms ease-in;
}

@keyframes fadeToBackground {
  from {
    opacity: 0.95;
    pointer-events: auto;
  }
  to {
    opacity: 0.7;
    pointer-events: none;
  }
}
```

## 6. 紧急程度色彩系统

### 6.1 四级紧急程度色彩
```css
/* Critical - 紧急 */
.critical-color { 
  color: #DC2626;
  background: rgba(220, 38, 38, 0.1);
  border-color: #DC2626;
}

/* High - 高优先级 */
.high-color { 
  color: #D97706;
  background: rgba(217, 119, 6, 0.1);
  border-color: #D97706;
}

/* Medium - 中优先级 */
.medium-color { 
  color: #059669;
  background: rgba(5, 150, 105, 0.1);
  border-color: #059669;
}

/* Low - 低优先级 */
.low-color { 
  color: #6B7280;
  background: rgba(107, 114, 128, 0.05);
  border-color: #E5E7EB;
}
```

### 6.2 语义色彩层次
```css
/* 危险/紧急 - 最高警示级别 */
.danger-high {
  color: #DC2626;
  background: rgba(220, 38, 38, 0.1);
  border: 1px solid rgba(220, 38, 38, 0.3);
}

/* 警告/重要 - 中等警示级别 */
.warning-medium {
  color: #D97706;
  background: rgba(217, 119, 6, 0.08);
}

/* 成功/完成 - 积极反馈 */
.success-low {
  color: #059669;
  background: rgba(5, 150, 105, 0.05);
}
```

### 6.3 中性色层次
```css
/* 文字层次 */
.text-primary { color: #111827; }      /* Level 1: 主标题 */
.text-secondary { color: #374151; }    /* Level 2: 次标题 */
.text-tertiary { color: #6B7280; }     /* Level 3: 正文 */
.text-quaternary { color: #9CA3AF; }   /* Level 4: 说明文字 */
.text-disabled { color: #D1D5DB; }     /* Level 5: 禁用状态 */
```

## 7. 空间层次设计

### 7.1 间距层次
```css
/* 间距系统 - 基于4px网格 */
.space-xs { margin: 4px; }      /* 紧密相关元素 */
.space-sm { margin: 8px; }      /* 相关元素 */
.space-md { margin: 12px; }     /* 默认间距 */
.space-lg { margin: 16px; }     /* 分组间距 */
.space-xl { margin: 24px; }     /* 区域间距 */
.space-2xl { margin: 32px; }    /* 页面级间距 */
```

### 7.2 Z轴层次
```css
/* Z-index层级系统 */
.z-base { z-index: 1; }         /* 基础元素 */
.z-elevated { z-index: 10; }    /* 提升元素 */
.z-floating { z-index: 100; }   /* 悬浮元素 */
.z-modal { z-index: 1000; }     /* 模态框 */
.z-tooltip { z-index: 9999; }   /* 工具提示 */
.z-top { z-index: 99999; }      /* 最顶层 */
```

## 8. 动态层次效果

### 8.1 进入动画层次
```css
/* 错开动画 - 创建层次感 */
.stagger-item-1 { animation-delay: 0s; }
.stagger-item-2 { animation-delay: 0.1s; }
.stagger-item-3 { animation-delay: 0.2s; }
.stagger-item-4 { animation-delay: 0.3s; }

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

### 8.2 滚动视差层次
```css
/* 滚动时不同层次的移动速度 */
.parallax-bg { transform: translateY(calc(scrollTop * 0.5)); }
.parallax-mid { transform: translateY(calc(scrollTop * 0.8)); }
.parallax-fg { transform: translateY(calc(scrollTop * 1.0)); }
```

## 9. 响应式层次适配

### 9.1 屏幕尺寸层次
```css
/* 大屏幕 - 完整层次 */
@media (min-width: 1200px) {
  .hierarchy-full {
    --spacing-multiplier: 1.2;
    --contrast-multiplier: 1.1;
  }
}

/* 中屏幕 - 标准层次 */
@media (min-width: 768px) and (max-width: 1199px) {
  .hierarchy-standard {
    --spacing-multiplier: 1.0;
    --contrast-multiplier: 1.0;
  }
}

/* 小屏幕 - 简化层次 */
@media (max-width: 767px) {
  .hierarchy-minimal {
    --spacing-multiplier: 0.8;
    --contrast-multiplier: 1.2; /* 提高对比度补偿尺寸 */
  }
}
```

### 9.2 DPI适配层次
```css
/* 高DPI屏幕 */
@media (-webkit-min-device-pixel-ratio: 2) {
  .high-dpi {
    border-width: 0.5px; /* 更细的边框 */
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1); /* 更精细的阴影 */
  }
}
```

## 10. 焦点管理层次

### 10.1 键盘导航层次
```css
/* Tab键焦点顺序 */
.tab-order-1 { tab-index: 1; } /* 主要操作 */
.tab-order-2 { tab-index: 2; } /* 次要操作 */
.tab-order-3 { tab-index: 3; } /* 辅助操作 */

/* 焦点状态层次 */
.focus-primary {
  outline: 2px solid #2563EB;
  outline-offset: 2px;
}

.focus-secondary {
  outline: 1px solid #6B7280;
  outline-offset: 1px;
}
```

### 10.2 屏幕阅读器层次
```css
/* ARIA优先级 */
.aria-level-1 { aria-level: 1; } /* 页面主标题 */
.aria-level-2 { aria-level: 2; } /* 区域标题 */
.aria-level-3 { aria-level: 3; } /* 内容标题 */

/* 重要性标识 */
.aria-important { aria-relevant: "all"; }
.aria-polite { aria-live: "polite"; }
.aria-assertive { aria-live: "assertive"; }
```

## 11. 实际应用示例

### 11.1 任务列表层次实现
```xml
<!-- WPF实现示例 -->
<ListBox x:Name="TaskListBox" Style="{StaticResource TaskListStyle}">
  <ListBox.ItemTemplate>
    <DataTemplate>
      <Border Style="{StaticResource TaskItemBorder}">
        <Grid>
          <!-- Level 1: 任务标题 -->
          <TextBlock Text="{Binding Title}" 
                     Style="{StaticResource PrimaryTextStyle}"/>
          
          <!-- Level 2: 任务时间 -->
          <TextBlock Text="{Binding DueTime}" 
                     Style="{StaticResource SecondaryTextStyle}"/>
          
          <!-- Level 3: 优先级指示 -->
          <Ellipse Fill="{Binding Priority, Converter={StaticResource PriorityColorConverter}}"
                   Style="{StaticResource PriorityIndicatorStyle}"/>
        </Grid>
      </Border>
    </DataTemplate>
  </ListBox.ItemTemplate>
</ListBox>
```

### 11.2 CSS变量层次系统
```css
:root {
  /* 层次权重系统 */
  --weight-primary: 900;
  --weight-secondary: 600;
  --weight-tertiary: 400;
  --weight-quaternary: 300;
  
  /* 透明度层次 */
  --opacity-primary: 1.0;
  --opacity-secondary: 0.8;
  --opacity-tertiary: 0.6;
  --opacity-quaternary: 0.4;
  
  /* 尺寸层次 */
  --size-xl: 1.2em;
  --size-lg: 1.1em;
  --size-md: 1em;
  --size-sm: 0.9em;
  --size-xs: 0.8em;
}
```

---

这个视觉层次设计确保了FlexToDo在每个状态下都能有效引导用户注意力，提供清晰的信息架构和直观的交互体验。