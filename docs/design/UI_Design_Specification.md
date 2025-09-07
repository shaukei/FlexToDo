# FlexToDo UI设计规格说明书

## 1. 项目概述

**产品名称**: FlexToDo 悬浮待办工具  
**平台**: Windows WPF应用  
**设计目标**: 轻量级、非干扰性、高效的桌面待办管理工具  

## 2. 设计原则

- **轻量化**: 最小化视觉干扰，保持工作区域清洁
- **高效性**: 快速访问和操作，支持热键交互
- **视觉层次**: 清晰的优先级和状态展示
- **渐进式**: 三层界面状态，按需展现信息

## 3. 界面状态设计

### 3.1 最小化状态 (30x30px)
```
尺寸: 30x30px
形状: 圆形
透明度: 30%
阴影: 0 2px 8px rgba(37, 99, 235, 0.3)
```

**设计元素**:
- 背景: 主色调#2563EB的径向渐变
- 图标: 白色待办事项图标 (16x16px)
- 悬浮动画: 鼠标悬停时轻微放大1.1倍

### 3.2 预览状态 (200x100px)
```
尺寸: 200x100px
圆角: 12px
透明度: 80%
阴影: 0 4px 16px rgba(0, 0, 0, 0.2)
```

**布局结构**:
```
┌─────────────────────────────┐
│ 今日待办 (3)        [详细] │ ← 顶部栏 (20px高)
├─────────────────────────────┤
│ ● 紧急任务1            14:00 │ ← 任务项 (20px高)
│ ● 重要任务2            16:30 │
│ ● 普通任务3            待定   │
├─────────────────────────────┤
│ [+] 添加任务              │ ← 底部操作栏 (20px高)
└─────────────────────────────┘
```

### 3.3 详细状态 (300x400px)
```
尺寸: 300x400px
圆角: 16px
透明度: 90%
阴影: 0 8px 32px rgba(0, 0, 0, 0.3)
```

**完整布局**:
```
┌───────────────────────────────┐
│ FlexToDo        [-] [□] [×]   │ ← 标题栏 (32px)
├───────────────────────────────┤
│ 搜索框                  [🔍]  │ ← 搜索区域 (36px)
├───────────────────────────────┤
│ [全部][今日][重要][已完成]     │ ← 筛选标签 (32px)
├───────────────────────────────┤
│                               │
│ 任务列表区域                   │ ← 主内容区域 (240px)
│ (可滚动)                      │
│                               │
├───────────────────────────────┤
│ [+] 添加新任务                │ ← 添加按钮 (40px)
├───────────────────────────────┤
│ 统计: 完成 3/8 | 今日剩余 5个  │ ← 状态栏 (24px)
└───────────────────────────────┘
```

## 4. 颜色系统

### 4.1 主要颜色
```css
/* 品牌色 */
--primary-blue: #2563EB;
--primary-blue-light: #3B82F6;
--primary-blue-dark: #1D4ED8;

/* 语义色彩 */
--danger-red: #DC2626;
--danger-red-light: #EF4444;
--success-green: #059669;
--warning-yellow: #D97706;

/* 中性色 */
--gray-50: #F9FAFB;
--gray-100: #F3F4F6;
--gray-200: #E5E7EB;
--gray-300: #D1D5DB;
--gray-400: #9CA3AF;
--gray-500: #6B7280;
--gray-600: #4B5563;
--gray-700: #374151;
--gray-800: #1F2937;
--gray-900: #111827;
```

### 4.2 透明度变体
```css
/* 背景透明度 */
--bg-minimized: rgba(37, 99, 235, 0.3);
--bg-preview: rgba(255, 255, 255, 0.8);
--bg-detailed: rgba(255, 255, 255, 0.9);

/* 悬浮效果 */
--hover-overlay: rgba(37, 99, 235, 0.1);
--active-overlay: rgba(37, 99, 235, 0.2);
```

## 5. 字体规范

### 5.1 字体族
```css
font-family: "Microsoft YaHei UI", "Segoe UI", system-ui, sans-serif;
```

### 5.2 字体尺寸层级
```css
/* 标题 */
--text-title: 14px/20px, font-weight: 600;
--text-subtitle: 12px/16px, font-weight: 500;

/* 正文 */
--text-body: 13px/18px, font-weight: 400;
--text-small: 11px/14px, font-weight: 400;
--text-tiny: 10px/12px, font-weight: 400;

/* 特殊 */
--text-button: 12px/16px, font-weight: 500;
--text-caption: 10px/14px, font-weight: 400, opacity: 0.7;
```

## 6. 组件设计规范

### 6.1 任务项组件
```
高度: 40px (固定高度)
内边距: 8px 12px
圆角: 6px
紧急程度条: 左侧 4px 宽度垂直边条
```

**状态样式**:
- **背景模式**: 无交互，仅显示
- **交互模式悬浮**: 背景 rgba(37, 99, 235, 0.05), 微小上移
- **完成**: 文字删除线，透明度0.6
- **紧急程度颜色**: 红色(Critical) > 橙色(High) > 黄色(Medium) > 绿色(Low)

**紧急程度指示器**:
```css
.urgency-critical { border-left: 4px solid #DC2626; }
.urgency-high { border-left: 4px solid #D97706; }
.urgency-medium { border-left: 4px solid #059669; }
.urgency-low { border-left: 4px solid #E5E7EB; }
```

### 6.2 按钮组件
```css
/* 主要按钮 */
.btn-primary {
  background: #2563EB;
  color: white;
  border-radius: 6px;
  padding: 8px 16px;
  border: none;
  font-weight: 500;
}

.btn-primary:hover {
  background: #1D4ED8;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

/* 次要按钮 */
.btn-secondary {
  background: transparent;
  color: #4B5563;
  border: 1px solid #D1D5DB;
  border-radius: 6px;
  padding: 8px 16px;
}
```

### 6.3 输入框组件
```css
.input-field {
  border: 1px solid #D1D5DB;
  border-radius: 8px;
  padding: 10px 12px;
  font-size: 13px;
  background: rgba(255, 255, 255, 0.8);
  transition: all 0.2s ease;
}

.input-field:focus {
  outline: none;
  border-color: #2563EB;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}
```

## 7. 图标系统

### 7.1 图标库
建议使用 **Feather Icons** 或 **Heroicons** 作为主要图标库

**核心图标**:
- 添加: plus-circle (16px/20px)
- 删除: trash-2 (16px)
- 编辑: edit-3 (16px)
- 完成: check-circle (16px)
- 设置: settings (16px)
- 搜索: search (16px)
- 最小化: minus (12px)
- 关闭: x (12px)

### 7.2 图标规范
```css
.icon {
  width: 16px;
  height: 16px;
  stroke-width: 1.5px;
  color: #6B7280;
}

.icon-small {
  width: 14px;
  height: 14px;
}

.icon-large {
  width: 20px;
  height: 20px;
}
```

## 8. 动画规范

### 8.1 过渡动画
```css
/* 基础过渡 */
transition: all 0.2s cubic-bezier(0.4, 0.0, 0.2, 1);

/* 弹性效果 */
transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);

/* 淡入淡出 */
transition: opacity 0.15s ease-in-out;
```

### 8.2 状态切换动画
- **最小化→预览**: 0.3s ease-out, 从圆形展开为矩形
- **预览→详细**: 0.25s ease-out, 垂直展开
- **任务完成**: 0.2s ease-in, 复选框填充动画

### 8.3 微交互动画
- **悬浮提升**: `transform: translateY(-2px)`
- **点击反馈**: `transform: scale(0.95)` → `scale(1)`
- **加载状态**: 脉冲动画或旋转动画

## 9. 响应式设计

### 9.1 缩放适配
支持Windows系统DPI缩放 (100%, 125%, 150%, 200%)

### 9.2 最小尺寸限制
- 详细状态最小宽度: 280px
- 详细状态最小高度: 320px

## 10. 无障碍设计

### 10.1 颜色对比度
- 正文文字与背景对比度 ≥ 4.5:1
- 大文字与背景对比度 ≥ 3:1
- 交互元素对比度 ≥ 3:1

### 10.2 键盘导航
- Tab键顺序逻辑清晰
- 焦点状态视觉明确
- 支持空格键和回车键操作

## 11. WPF实现建议

### 11.1 窗口设置
```xml
<Window 
    WindowStyle="None" 
    AllowsTransparency="True" 
    Background="Transparent"
    Topmost="True"
    ShowInTaskbar="False">
```

### 11.2 样式资源
创建统一的资源字典文件:
- Colors.xaml: 颜色定义
- Typography.xaml: 字体样式
- Controls.xaml: 控件样式

### 11.3 数据绑定结构
```csharp
public class TaskItem
{
    public string Title { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
}

public enum Priority
{
    Low,
    Medium, 
    High
}
```

## 12. 性能优化建议

- 使用虚拟化ListView显示大量任务
- 实现窗口缓存避免重复创建
- 优化透明度渲染性能
- 使用合成层避免重绘

---

**设计版本**: v1.0  
**创建日期**: 2025-09-05  
**设计师**: Claude UI Designer  