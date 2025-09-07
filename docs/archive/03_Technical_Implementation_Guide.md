# FlexToDo C# WPF技术实现指南

## 项目概述
基于WPF开发的简化二元状态待办事项悬浮工具，实现280px固定宽度的始终可见待办列表，可在背景穿透和交互模式之间切换。

## 1. 项目架构设计

### 解决方案结构
```
FlexToDo/
├── FlexToDo.Core/                 # 核心业务逻辑
│   ├── Models/                    # 数据模型
│   ├── Services/                  # 业务服务
│   └── Interfaces/                # 接口定义
├── FlexToDo.UI/                   # WPF用户界面
│   ├── Views/                     # 视图
│   ├── ViewModels/                # 视图模型
│   ├── Controls/                  # 自定义控件
│   ├── Converters/                # 值转换器
│   └── Resources/                 # 资源文件
├── FlexToDo.Infrastructure/       # 基础设施
│   ├── Win32/                     # Win32 API封装
│   ├── Storage/                   # 本地存储
│   └── Utilities/                 # 工具类
└── FlexToDo.App/                  # 应用程序入口
    ├── App.xaml                   # 应用程序
    ├── MainWindow.xaml            # 主窗口
    └── NotifyIcon.cs              # 系统托盘
```

### 核心设计模式
- **MVVM模式**：视图与业务逻辑分离
- **依赖注入**：使用Microsoft.Extensions.DependencyInjection
- **命令模式**：热键和UI操作统一处理
- **观察者模式**：状态变化和UI更新

## 2. 关键技术实现

### 2.1 窗口状态管理器

```csharp
/// <summary>
/// 窗口状态控制器 - 管理背景穿透模式和交互模式的切换
/// </summary>
public class WindowStateController : INotifyPropertyChanged, IDisposable
{
    private readonly Window _window;
    private WindowMode _currentMode = WindowMode.InteractionActive;
    private Timer? _autoReturnTimer;
    private bool _isAnimating = false;

    // 窗口属性记录
    private double _windowWidth = 280;
    private double _normalOpacity = 0.7; // 背景穿透模式透明度
    private double _activeOpacity = 0.95; // 交互模式透明度
    
    public enum WindowMode
    {
        BackgroundTransparent, // 背景穿透模式
        InteractionActive      // 交互激活模式
    }
    
    public WindowMode CurrentMode
    {
        get => _currentMode;
        private set => SetProperty(ref _currentMode, value);
    }
    
    public bool IsAnimating
    {
        get => _isAnimating;
        private set => SetProperty(ref _isAnimating, value);
    }
    
    /// <summary>
    /// 切换到背景穿透模式（保持列表界面显示，只启用点击穿透，界面不变）
    /// </summary>
    public async Task SwitchToBackgroundModeAsync()
    {
        if (CurrentMode == WindowMode.BackgroundTransparent || IsAnimating)
            return;

        IsAnimating = true;
        
        try
        {
            // 取消自动返回定时器
            _autoReturnTimer?.Dispose();
            _autoReturnTimer = null;

            // 只降低透明度到正常水平，不改变窗口大小或界面
            await PlayCollapseAnimationAsync();
            
            // 设置背景穿透模式属性（启用点击穿透）
            SetBackgroundModeProperties();
            
            CurrentMode = WindowMode.BackgroundTransparent;
        }
        finally
        {
            IsAnimating = false;
        }
    }
    
    /// <summary>
    /// 激活窗口（增加不透明度，获取焦点）
    /// </summary>
    public async Task ActivateWindowAsync()
    {
        if (IsAnimating)
            return;

        IsAnimating = true;
        
        try
        {
            // 先设置为交互模式
            SetInteractionModeProperties();
            CurrentMode = WindowMode.InteractionActive;
            
            // 播放激活动画（增加不透明度）
            await PlayActivateAnimationAsync();
            
            // 获取焦点
            _window.Activate();
            _window.Focus();
            
            // 启动自动返回定时器
            SetAutoReturnTimer();
        }
        finally
        {
            IsAnimating = false;
        }
    }
    
    /// <summary>
    /// 播放激活动画
    /// </summary>
    private Task PlayActivateAnimationAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            var storyboard = new Storyboard();
            
            // 只有透明度动画，提高到激活不透明度
            var opacityAnimation = new DoubleAnimation
            {
                From = _window.Opacity,
                To = _activeOpacity, // 0.95
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            Storyboard.SetTarget(opacityAnimation, _window);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));
            storyboard.Children.Add(opacityAnimation);
            
            storyboard.Completed += (s, e) => tcs.SetResult(true);
            storyboard.Begin();
        });
        
        return tcs.Task;
    }
    
    /// <summary>
    /// 播放收起动画
    /// </summary>
    private Task PlayCollapseAnimationAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            var storyboard = new Storyboard();
            
            // 只有透明度动画，宽度保持280px不变
            var opacityAnimation = new DoubleAnimation
            {
                From = _activeOpacity, // 0.95
                To = _normalOpacity,   // 0.7
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            
            Storyboard.SetTarget(opacityAnimation, _window);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));
            storyboard.Children.Add(opacityAnimation);
            
            storyboard.Completed += (s, e) => tcs.SetResult(true);
            storyboard.Begin();
        });
        
        return tcs.Task;
    }
    
    /// <summary>
    /// 设置背景穿透模式窗口属性
    /// </summary>
    private void SetBackgroundModeProperties()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // 设置窗口样式 - 禁用鼠标交互（点击穿透）
            _window.IsHitTestVisible = false;
            
            // 设置Win32窗口属性
            var hwnd = new WindowInteropHelper(_window).Handle;
            if (hwnd != IntPtr.Zero)
            {
                var currentStyle = Win32Api.GetWindowLong(hwnd, Win32Api.GWL_EXSTYLE);
                
                // 添加透明、工具窗口和不激活样式 - 实现点击穿透
                var newStyle = currentStyle | Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_TOOLWINDOW | Win32Api.WS_EX_NOACTIVATE;
                Win32Api.SetWindowLong(hwnd, Win32Api.GWL_EXSTYLE, newStyle);
            }
        });
    }
    
    /// <summary>
    /// 设置交互模式窗口属性
    /// </summary>
    private void SetInteractionModeProperties()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // 先取消鼠标穿透
            var hwnd = new WindowInteropHelper(_window).Handle;
            if (hwnd != IntPtr.Zero)
            {
                var currentStyle = Win32Api.GetWindowLong(hwnd, Win32Api.GWL_EXSTYLE);
                
                // 移除透明和不激活样式，保留工具窗口样式
                var newStyle = (currentStyle & ~(Win32Api.WS_EX_TRANSPARENT | Win32Api.WS_EX_NOACTIVATE)) | Win32Api.WS_EX_TOOLWINDOW;
                Win32Api.SetWindowLong(hwnd, Win32Api.GWL_EXSTYLE, newStyle);
            }
            
            // 启用交互
            _window.IsHitTestVisible = true;
        });
    }
    
    /// <summary>
    /// 设置自动返回定时器
    /// </summary>
    private void SetAutoReturnTimer()
    {
        _autoReturnTimer?.Dispose();
        
        // 10秒后自动返回背景穿透模式
        _autoReturnTimer = new Timer(async _ =>
        {
            if (CurrentMode == WindowMode.InteractionActive)
            {
                await SwitchToBackgroundModeAsync();
            }
        }, null, TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
    }
}
```

### 2.2 全局热键管理器

```csharp
/// <summary>
/// 全局热键管理器
/// </summary>
public class GlobalHotkeyManager : IDisposable
{
    private const int WM_HOTKEY = 0x0312;
    private readonly Dictionary<int, HotkeyInfo> _registeredHotkeys = new();
    private int _nextHotkeyId = 1;
    
    public class HotkeyInfo
    {
        public int Id { get; set; }
        public ModifierKeys Modifiers { get; set; }
        public Key Key { get; set; }
        public Action Callback { get; set; }
        public string Description { get; set; }
    }
    
    /// <summary>
    /// 注册全局热键
    /// </summary>
    public bool RegisterHotkey(ModifierKeys modifiers, Key key, Action callback, string description = "")
    {
        var id = _nextHotkeyId++;
        var virtualKey = KeyInterop.VirtualKeyFromKey(key);
        
        if (RegisterHotKey(IntPtr.Zero, id, (uint)modifiers, (uint)virtualKey))
        {
            _registeredHotkeys[id] = new HotkeyInfo
            {
                Id = id,
                Modifiers = modifiers,
                Key = key,
                Callback = callback,
                Description = description
            };
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 消息循环处理
    /// </summary>
    public IntPtr HotkeyHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_HOTKEY)
        {
            var id = wParam.ToInt32();
            if (_registeredHotkeys.ContainsKey(id))
            {
                _registeredHotkeys[id].Callback?.Invoke();
                handled = true;
            }
        }
        return IntPtr.Zero;
    }
    
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    
    public void Dispose()
    {
        foreach (var hotkey in _registeredHotkeys.Values)
        {
            UnregisterHotKey(IntPtr.Zero, hotkey.Id);
        }
        _registeredHotkeys.Clear();
    }
}
```

### 2.3 数据模型设计

```csharp
/// <summary>
/// 待办事项模型
/// </summary>
public class TodoItem : INotifyPropertyChanged
{
    private string _title;
    private string _description;
    private DateTime? _deadline;
    private UrgencyLevel _urgency;
    private bool _isCompleted;
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public string Description  
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public DateTime? Deadline
    {
        get => _deadline;
        set
        {
            SetProperty(ref _deadline, value);
            UpdateUrgency();
        }
    }
    
    public UrgencyLevel Urgency
    {
        get => _urgency;
        private set => SetProperty(ref _urgency, value);
    }
    
    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// 基于deadline自动更新紧急程度
    /// </summary>
    private void UpdateUrgency()
    {
        if (!Deadline.HasValue || IsCompleted)
        {
            Urgency = UrgencyLevel.Low;
            return;
        }
        
        var timeLeft = Deadline.Value - DateTime.Now;
        
        if (timeLeft < TimeSpan.Zero)
            Urgency = UrgencyLevel.Critical; // 已过期
        else if (timeLeft < TimeSpan.FromHours(6))
            Urgency = UrgencyLevel.Critical; // 6小时内
        else if (timeLeft < TimeSpan.FromHours(24))
            Urgency = UrgencyLevel.High;     // 24小时内
        else if (timeLeft < TimeSpan.FromDays(3))
            Urgency = UrgencyLevel.Medium;   // 3天内
        else
            Urgency = UrgencyLevel.Low;      // 3天以上
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

public enum UrgencyLevel
{
    Low,      // 绿色
    Medium,   // 黄色  
    High,     // 橙色
    Critical  // 红色
}
```

### 2.4 本地数据存储

```csharp
/// <summary>
/// JSON本地存储服务
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly string _dataPath;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public LocalStorageService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dataPath = Path.Combine(appDataPath, "FlexToDo", "todos.json");
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // 确保目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(_dataPath));
    }
    
    public async Task<List<TodoItem>> LoadTodosAsync()
    {
        try
        {
            if (!File.Exists(_dataPath))
                return new List<TodoItem>();
                
            var json = await File.ReadAllTextAsync(_dataPath);
            return JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions) ?? new List<TodoItem>();
        }
        catch (Exception ex)
        {
            // 记录错误日志
            Debug.WriteLine($"加载数据失败: {ex.Message}");
            return new List<TodoItem>();
        }
    }
    
    public async Task SaveTodosAsync(List<TodoItem> todos)
    {
        try
        {
            var json = JsonSerializer.Serialize(todos, _jsonOptions);
            await File.WriteAllTextAsync(_dataPath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存数据失败: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 自动备份机制
    /// </summary>
    public async Task CreateBackupAsync()
    {
        if (!File.Exists(_dataPath)) return;
        
        var backupPath = _dataPath.Replace(".json", $"_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.Copy(_dataPath, backupPath);
        
        // 清理旧备份文件（保留最近10个）
        var backupDir = Path.GetDirectoryName(_dataPath);
        var backupFiles = Directory.GetFiles(backupDir, "*_backup_*.json")
                                  .OrderByDescending(f => File.GetCreationTime(f))
                                  .Skip(10);
        
        foreach (var oldBackup in backupFiles)
        {
            File.Delete(oldBackup);
        }
    }
}
```

## 3. 主窗口XAML设计

```xml
<Window x:Class="FlexToDo.UI.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="FlexToDo"
        Width="4" Height="600"
        WindowStyle="None"
        AllowsTransparency="True"
        Topmost="True"
        ShowInTaskbar="False"
        ResizeMode="NoResize"
        Left="{Binding WindowLeft}"
        Top="{Binding WindowTop}"
        Background="Transparent">
    
    <Window.Resources>
        <!-- 紧急程度颜色定义 -->
        <SolidColorBrush x:Key="CriticalBrush" Color="#ef4444"/>
        <SolidColorBrush x:Key="HighBrush" Color="#f97316"/>
        <SolidColorBrush x:Key="MediumBrush" Color="#eab308"/>
        <SolidColorBrush x:Key="LowBrush" Color="#22c55e"/>
        
        <!-- 渐变背景 -->
        <LinearGradientBrush x:Key="UrgencyGradient" StartPoint="0,0" EndPoint="0,1">
            <GradientStop Color="#ef4444" Offset="0"/>
            <GradientStop Color="#f97316" Offset="0.25"/>
            <GradientStop Color="#eab308" Offset="0.5"/>
            <GradientStop Color="#22c55e" Offset="0.75"/>
            <GradientStop Color="Transparent" Offset="1"/>
        </LinearGradientBrush>
    </Window.Resources>
    
    <!-- 主容器 -->
    <Border Name="MainBorder" CornerRadius="2,0,0,2">
        <!-- 背景水印模式 -->
        <Border Name="BackgroundMode" 
                Background="{StaticResource UrgencyGradient}"
                Visibility="{Binding IsBackgroundMode, Converter={StaticResource BooleanToVisibilityConverter}}"/>
        
        <!-- 交互模式 -->
        <Border Name="InteractionMode"
                Background="rgba(17, 24, 39, 0.85)"
                CornerRadius="12,0,0,12"
                BorderBrush="rgba(75, 85, 99, 0.3)"
                BorderThickness="1"
                Visibility="{Binding IsInteractionMode, Converter={StaticResource BooleanToVisibilityConverter}}">
            
            <Grid Margin="16">
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                
                <!-- 标题栏 -->
                <TextBlock Grid.Row="0" 
                          Text="{Binding Title}"
                          Foreground="White"
                          FontSize="14"
                          FontWeight="SemiBold"
                          Margin="0,0,0,16"/>
                
                <!-- 待办列表 -->
                <ScrollViewer Grid.Row="1">
                    <ItemsControl ItemsSource="{Binding TodoItems}">
                        <ItemsControl.ItemTemplate>
                            <DataTemplate>
                                <Border Margin="0,0,0,8" 
                                       Padding="12" 
                                       Background="rgba(255,255,255,0.1)"
                                       CornerRadius="8">
                                    <Grid>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="*"/>
                                            <ColumnDefinition Width="Auto"/>
                                        </Grid.ColumnDefinitions>
                                        
                                        <StackPanel Grid.Column="0">
                                            <TextBlock Text="{Binding Title}"
                                                      Foreground="White"
                                                      FontSize="13"/>
                                            <TextBlock Text="{Binding Deadline, StringFormat='📅 {0:MM/dd HH:mm}'}"
                                                      Foreground="rgba(255,255,255,0.7)"
                                                      FontSize="11"
                                                      Margin="0,4,0,0"/>
                                        </StackPanel>
                                        
                                        <StackPanel Grid.Column="1" Orientation="Horizontal">
                                            <Button Content="✓" 
                                                   Command="{Binding CompleteCommand}"
                                                   Style="{StaticResource SmallButtonStyle}"/>
                                        </StackPanel>
                                    </Grid>
                                </Border>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>
                </ScrollViewer>
                
                <!-- 快速添加 -->
                <Border Grid.Row="2" 
                       Background="rgba(255,255,255,0.1)"
                       CornerRadius="8"
                       Padding="12"
                       Margin="0,16,0,0">
                    <TextBox Name="QuickAddTextBox"
                            Text="{Binding QuickAddText, UpdateSourceTrigger=PropertyChanged}"
                            Background="Transparent"
                            Foreground="White"
                            BorderThickness="0"
                            FontSize="13">
                        <TextBox.InputBindings>
                            <KeyBinding Key="Return" Command="{Binding QuickAddCommand}"/>
                            <KeyBinding Key="Escape" Command="{Binding CancelCommand}"/>
                        </TextBox.InputBindings>
                    </TextBox>
                </Border>
            </Grid>
        </Border>
    </Border>
</Window>
```

## 4. 性能优化策略

### 4.1 内存管理
```csharp
/// <summary>
/// 性能监控和优化服务
/// </summary>
public class PerformanceOptimizer
{
    private readonly Timer _memoryCleanupTimer;
    private readonly Timer _urgencyUpdateTimer;
    
    public PerformanceOptimizer()
    {
        // 每5分钟进行内存清理
        _memoryCleanupTimer = new Timer(PerformMemoryCleanup, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            
        // 每分钟更新紧急程度
        _urgencyUpdateTimer = new Timer(UpdateUrgencyLevels, null,
            TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }
    
    private void PerformMemoryCleanup(object state)
    {
        // 在背景模式时进行垃圾回收
        if (WindowStateController.CurrentMode == WindowMode.BackgroundWatermark)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
    
    private void UpdateUrgencyLevels(object state)
    {
        // 更新所有待办事项的紧急程度
        foreach (var item in TodoService.GetAllTodos())
        {
            item.UpdateUrgency();
        }
    }
}
```

### 4.2 渲染优化
```csharp
/// <summary>
/// UI虚拟化和渲染优化
/// </summary>
public class RenderingOptimizer
{
    /// <summary>
    /// 在背景模式时暂停不必要的动画
    /// </summary>
    public void OptimizeForBackgroundMode()
    {
        // 暂停复杂动画
        Timeline.DesiredFrameRateProperty.OverrideMetadata(
            typeof(Timeline),
            new FrameworkPropertyMetadata { DefaultValue = 10 });
    }
    
    /// <summary>
    /// 在交互模式时恢复流畅动画
    /// </summary>
    public void OptimizeForInteractionMode()
    {
        // 恢复60fps动画
        Timeline.DesiredFrameRateProperty.OverrideMetadata(
            typeof(Timeline),
            new FrameworkPropertyMetadata { DefaultValue = 60 });
    }
}
```

## 5. 部署和打包

### 5.1 项目配置
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <ApplicationIcon>icon.ico</ApplicationIcon>
    <AssemblyTitle>FlexToDo</AssemblyTitle>
    <AssemblyDescription>轻量级悬浮待办事项工具</AssemblyDescription>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="System.Text.Json" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### 5.2 单文件发布
```bash
# 发布单文件可执行程序
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# 优化体积（启用裁剪）
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true
```

### 5.3 安装程序制作
使用WiX Toolset或Inno Setup创建安装程序，包含：
- 自动启动项设置
- 开始菜单快捷方式
- 卸载支持
- 数据目录创建

这个技术实现方案确保了FlexToDo能够完美实现"背景水印"式的用户体验，在保持轻量化的同时提供强大的功能。