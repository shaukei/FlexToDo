# FlexToDo 图标设计要求

## 1. 图标设计原则

### 1.1 设计理念
- **极简主义**: 去除多余元素，保留核心特征
- **一致性**: 统一的视觉语言和风格
- **可识别性**: 在小尺寸下依然清晰可辨
- **功能性**: 直观传达功能含义

### 1.2 视觉特征
- **线条风格**: 1.5px描边，圆润端点
- **圆角处理**: 2px圆角，避免尖锐边缘
- **填充方式**: 线性图标为主，关键状态使用填充
- **几何规范**: 基于16×16px网格系统

### 1.3 设计约束
- **兼容性**: 支持Windows 10/11系统规范
- **可缩放**: 矢量格式，支持高DPI显示
- **颜色适配**: 支持明暗主题切换
- **无障碍**: 满足WCAG对比度要求

## 2. 图标尺寸规范

### 2.1 尺寸体系
```
特小号 (XS): 12×12px - 内联文字图标
小号 (S):   14×14px - 辅助功能图标
标准 (M):   16×16px - 主要功能图标 (默认)
大号 (L):   20×20px - 重要操作图标
特大号 (XL): 24×24px - 主导航图标
巨大号 (XXL): 32×32px - 应用主图标
```

### 2.2 网格系统
```
基础网格: 16×16px
安全区域: 2px (边缘留白)
绘制区域: 12×12px
视觉对齐: 14×14px (视觉权重)
```

### 2.3 DPI缩放支持
```css
/* 标准DPI (96dpi) */
.icon { width: 16px; height: 16px; }

/* 高DPI (120dpi) */
@media (min-resolution: 120dpi) {
  .icon { width: 20px; height: 20px; }
}

/* 超高DPI (144dpi) */
@media (min-resolution: 144dpi) {
  .icon { width: 24px; height: 24px; }
}
```

## 3. 图标分类与清单

### 3.1 核心功能图标

#### 任务管理
```
📝 plus-circle (添加任务)
- 描述: 圆形边框，内部加号
- 尺寸: 16×16px
- 描边: 1.5px
- 颜色: #2563EB
- 填充: 透明
- 用途: 新建任务按钮

☑️ check-square (完成任务)  
- 描述: 方形复选框，内部对勾
- 尺寸: 16×16px
- 描边: 1.5px
- 颜色: #059669 (完成), #D1D5DB (未完成)
- 用途: 任务状态切换

✏️ edit-3 (编辑任务)
- 描述: 铅笔图标，斜向右上
- 尺寸: 14×14px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 编辑任务内容

🗑️ trash-2 (删除任务)
- 描述: 垃圾桶，带盖子
- 尺寸: 14×14px  
- 描边: 1.5px
- 颜色: #DC2626
- 用途: 删除任务

📌 pin (置顶任务)
- 描述: 图钉图标
- 尺寸: 14×14px
- 描边: 1.5px
- 颜色: #D97706
- 用途: 重要任务标记
```

#### 界面控制
```
➖ minus (最小化)
- 描述: 水平直线
- 尺寸: 12×12px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 最小化窗口

❌ x (关闭)
- 描述: 叉号，两条对角线
- 尺寸: 12×12px
- 描边: 1.5px
- 颜色: #6B7280
- 悬浮: #DC2626
- 用途: 关闭窗口

⚙️ settings (设置)
- 描述: 齿轮图标
- 尺寸: 16×16px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 打开设置面板

🔍 search (搜索)
- 描述: 放大镜
- 尺寸: 16×16px
- 描边: 1.5px
- 颜色: #9CA3AF
- 用途: 搜索输入框
```

#### 时间与优先级
```
🕐 clock (时间)
- 描述: 圆形时钟，时针分针
- 尺寸: 14×14px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 时间显示

📅 calendar (日期)
- 描述: 日历图标，上方两个小孔
- 尺寸: 14×14px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 日期选择

⬆️ arrow-up (高优先级)
- 描述: 向上箭头
- 尺寸: 12×12px
- 描边: 2px
- 颜色: #DC2626
- 用途: 高优先级标识

➡️ arrow-right (中优先级)
- 描述: 向右箭头
- 尺寸: 12×12px
- 描边: 1.5px
- 颜色: #D97706
- 用途: 中优先级标识

⬇️ arrow-down (低优先级)
- 描述: 向下箭头
- 尺寸: 12×12px
- 描边: 1.5px
- 颜色: #6B7280
- 用途: 低优先级标识
```

### 3.2 状态指示图标

#### 任务状态
```
✅ check-circle (已完成)
- 描述: 圆形，内部对勾
- 尺寸: 16×16px
- 填充: #059669
- 用途: 完成状态

⏳ clock-alert (进行中)
- 描述: 时钟加感叹号
- 尺寸: 16×16px
- 颜色: #D97706
- 用途: 正在进行

⚠️ alert-circle (即将到期)
- 描述: 圆形感叹号
- 尺寸: 16×16px
- 颜色: #DC2626
- 用途: 紧急提醒

❌ x-circle (已过期)
- 描述: 圆形叉号
- 尺寸: 16×16px
- 颜色: #DC2626
- 填充: rgba(220, 38, 38, 0.1)
- 用途: 过期状态
```

#### 加载状态
```
🔄 refresh-cw (刷新/同步)
- 描述: 圆形箭头，顺时针
- 尺寸: 16×16px
- 描边: 1.5px
- 颜色: #2563EB
- 动画: 旋转
- 用途: 数据同步

⚡ zap (快速操作)
- 描述: 闪电图标
- 尺寸: 14×14px
- 颜色: #2563EB
- 用途: 快捷功能

💾 save (保存)
- 描述: 软盘图标
- 尺寸: 14×14px
- 描边: 1.5px
- 颜色: #059669
- 用途: 保存操作
```

### 3.3 分类标签图标

```
🏢 briefcase (工作)
- 颜色: #2563EB

🏠 home (个人)
- 颜色: #059669

🎓 book-open (学习)
- 颜色: #7C3AED

❤️ heart (生活)
- 颜色: #DC2626

🛒 shopping-cart (购物)
- 颜色: #D97706

💪 activity (健身)
- 颜色: #059669
```

## 4. 图标设计规格

### 4.1 几何构建规则

#### 基础形状
```css
/* 圆形图标 */
.icon-circle {
  border-radius: 50%;
  aspect-ratio: 1;
}

/* 方形图标 */
.icon-square {
  border-radius: 2px;
  aspect-ratio: 1;
}

/* 矩形图标 */
.icon-rect {
  border-radius: 2px;
  aspect-ratio: 4/3;
}
```

#### 描边规范
```css
.icon-stroke {
  stroke-width: 1.5px;
  stroke-linecap: round;
  stroke-linejoin: round;
  fill: none;
}

.icon-stroke-thick {
  stroke-width: 2px;
}

.icon-stroke-thin {
  stroke-width: 1px;
}
```

#### 填充规范
```css
.icon-filled {
  fill: currentColor;
  stroke: none;
}

.icon-dual {
  fill: currentColor;
  stroke: currentColor;
  stroke-width: 1px;
}
```

### 4.2 颜色系统

#### 主色调图标
```css
.icon-primary { color: #2563EB; }
.icon-primary-light { color: #3B82F6; }
.icon-primary-dark { color: #1D4ED8; }
```

#### 语义色彩
```css
.icon-success { color: #059669; }
.icon-warning { color: #D97706; }
.icon-danger { color: #DC2626; }
.icon-info { color: #0EA5E9; }
```

#### 中性色调
```css
.icon-gray-900 { color: #111827; } /* 主要内容 */
.icon-gray-700 { color: #374151; } /* 次要内容 */
.icon-gray-500 { color: #6B7280; } /* 辅助内容 */
.icon-gray-400 { color: #9CA3AF; } /* 占位符 */
.icon-gray-300 { color: #D1D5DB; } /* 禁用状态 */
```

#### 透明度变体
```css
.icon-opacity-100 { opacity: 1; }
.icon-opacity-80 { opacity: 0.8; }
.icon-opacity-60 { opacity: 0.6; }
.icon-opacity-40 { opacity: 0.4; }
.icon-opacity-20 { opacity: 0.2; }
```

### 4.3 状态变化

#### 交互状态
```css
.icon-interactive {
  transition: all 150ms ease-out;
  cursor: pointer;
}

.icon-interactive:hover {
  transform: scale(1.1);
  opacity: 0.8;
}

.icon-interactive:active {
  transform: scale(0.95);
}
```

#### 禁用状态
```css
.icon-disabled {
  opacity: 0.4;
  cursor: not-allowed;
  pointer-events: none;
}
```

## 5. 动画图标

### 5.1 加载动画
```css
@keyframes spin {
  to { transform: rotate(360deg); }
}

.icon-loading {
  animation: spin 1s linear infinite;
}
```

### 5.2 成功动画
```css
@keyframes checkmark {
  0% {
    stroke-dasharray: 0 50;
    stroke-dashoffset: 0;
  }
  100% {
    stroke-dasharray: 50 0;
    stroke-dashoffset: -50;
  }
}

.icon-success-animated {
  animation: checkmark 0.3s ease-in-out;
}
```

### 5.3 注意动画
```css
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.icon-alert {
  animation: pulse 2s ease-in-out infinite;
}
```

## 6. 技术实现

### 6.1 SVG图标模板
```xml
<svg width="16" height="16" viewBox="0 0 16 16" 
     fill="none" xmlns="http://www.w3.org/2000/svg">
  <path d="M8 2V14M2 8H14" 
        stroke="currentColor" 
        stroke-width="1.5" 
        stroke-linecap="round"/>
</svg>
```

### 6.2 WPF图标实现
```xml
<!-- 资源字典中定义 -->
<ResourceDictionary>
  <DrawingImage x:Key="PlusIcon">
    <DrawingImage.Drawing>
      <GeometryDrawing>
        <GeometryDrawing.Geometry>
          <PathGeometry Data="M8 2V14M2 8H14"/>
        </GeometryDrawing.Geometry>
        <GeometryDrawing.Pen>
          <Pen Brush="{DynamicResource PrimaryBrush}" Thickness="1.5"/>
        </GeometryDrawing.Pen>
      </GeometryDrawing>
    </DrawingImage.Drawing>
  </DrawingImage>
</ResourceDictionary>

<!-- 使用方式 -->
<Image Source="{StaticResource PlusIcon}" 
       Width="16" Height="16"/>
```

### 6.3 图标字体实现
```css
@font-face {
  font-family: 'FlexToDoIcons';
  src: url('icons/FlexToDoIcons.woff2') format('woff2'),
       url('icons/FlexToDoIcons.woff') format('woff');
  font-weight: normal;
  font-style: normal;
}

.icon {
  font-family: 'FlexToDoIcons';
  speak: none;
  font-style: normal;
  font-weight: normal;
  font-variant: normal;
  text-transform: none;
  line-height: 1;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.icon-plus:before { content: '\e001'; }
.icon-check:before { content: '\e002'; }
.icon-x:before { content: '\e003'; }
```

## 7. 导出规格

### 7.1 文件格式
```
SVG: 矢量图标，主要格式
PNG: 12px, 16px, 20px, 24px, 32px (各DPI)
ICO: 应用程序图标 (16, 32, 48, 256px)
XAML: WPF原生格式
```

### 7.2 命名规范
```
格式: [功能]_[状态]_[尺寸].svg
示例:
- plus_default_16.svg (添加图标)
- check_completed_16.svg (完成状态)
- settings_hover_20.svg (设置悬浮)
- trash_danger_16.svg (删除图标)
```

### 7.3 目录结构
```
/icons/
  /svg/           # SVG源文件
    /16px/        # 标准尺寸
    /20px/        # 大尺寸
    /24px/        # 特大尺寸
  /png/           # 位图导出
    /1x/          # 标准DPI
    /1.5x/        # 高DPI
    /2x/          # 超高DPI
  /xaml/          # WPF格式
  /font/          # 图标字体
    FlexToDoIcons.woff2
    FlexToDoIcons.woff
    FlexToDoIcons.css
```

## 8. 质量检查

### 8.1 视觉检查清单
- [ ] 16px下图标清晰可辨
- [ ] 描边粗细一致 (1.5px)
- [ ] 圆角处理规范 (2px)
- [ ] 视觉权重平衡
- [ ] 颜色对比度符合标准
- [ ] 与品牌风格一致

### 8.2 技术检查清单
- [ ] SVG代码简洁优化
- [ ] 路径数据精确
- [ ] 颜色使用currentColor
- [ ] 支持高DPI缩放
- [ ] 文件大小适中 (<2KB)
- [ ] 兼容目标浏览器

### 8.3 可用性检查清单
- [ ] 功能含义直观
- [ ] 文化背景适应性
- [ ] 无障碍友好
- [ ] 不同背景下可见
- [ ] 点击目标足够大 (最小24px)

## 9. 图标库管理

### 9.1 版本控制
```
v1.0: 基础功能图标 (20个)
v1.1: 增加分类图标 (8个)
v1.2: 增加状态图标 (12个)
v2.0: 重构设计语言
```

### 9.2 使用文档
每个图标应包含:
- 功能说明
- 使用场景
- 尺寸建议
- 颜色建议
- 实现代码

### 9.3 自动化工具
```bash
# SVG优化
svgo --folder=./icons/svg --output=./icons/optimized

# PNG导出
svg2png --input=./icons/svg --output=./icons/png --sizes=16,20,24

# 图标字体生成
fontello-cli --config=./icons/config.json --output=./icons/font
```

---

这个图标设计要求确保了FlexToDo拥有一套完整、一致、高质量的图标系统，支撑应用的视觉体验和功能交互。