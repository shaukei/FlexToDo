# FlexToDo WPF实现指南

## 1. WPF架构设计

### 1.1 项目结构
```
FlexToDo/
├── App.xaml                    # 应用程序入口
├── App.xaml.cs                 # 应用程序逻辑
├── MainWindow.xaml             # 主窗口 (隐藏)
├── Views/                      # 视图层
│   ├── FloatingWindow.xaml     # 悬浮窗口
│   ├── MinimizedView.xaml      # 最小化状态
│   ├── PreviewView.xaml        # 预览状态
│   └── DetailedView.xaml       # 详细状态
├── ViewModels/                 # 视图模型
│   ├── MainViewModel.cs        # 主视图模型
│   ├── TaskItemViewModel.cs    # 任务项模型
│   └── SettingsViewModel.cs    # 设置模型
├── Models/                     # 数据模型
│   ├── TodoTask.cs            # 任务数据模型
│   ├── TaskPriority.cs        # 优先级枚举
│   └── AppSettings.cs         # 应用设置
├── Services/                   # 服务层
│   ├── DataService.cs         # 数据服务
│   ├── HotkeyService.cs       # 热键服务
│   └── NotificationService.cs  # 通知服务
├── Controls/                   # 自定义控件
│   ├── TaskItemControl.xaml   # 任务项控件
│   ├── QuickAddControl.xaml   # 快速添加控件
│   └── PriorityIndicator.xaml # 优先级指示器
├── Resources/                  # 资源文件
│   ├── Styles.xaml            # 样式资源
│   ├── Colors.xaml            # 颜色资源
│   ├── Icons.xaml             # 图标资源
│   └── Animations.xaml        # 动画资源
├── Utilities/                  # 工具类
│   ├── WindowPositionHelper.cs # 窗口定位助手
│   ├── DpiAwarenessHelper.cs   # DPI适配助手
│   └── ThemeHelper.cs          # 主题切换助手
└── Assets/                     # 静态资源
    ├── Icons/                  # 图标文件
    ├── Fonts/                  # 字体文件
    └── Images/                 # 图片资源
```

### 1.2 MVVM模式实现
```csharp
// 基础视图模型
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;
            
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

// 命令实现
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;
    
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
    
    public void Execute(object parameter) => _execute();
    
    public event EventHandler CanExecuteChanged;
}
```

## 2. 悬浮窗口实现

### 2.1 窗口基础设置
```xml
<Window x:Class="FlexToDo.Views.FloatingWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="FlexToDo"
        Width="30" Height="30"
        WindowStyle="None"
        AllowsTransparency="True"
        Background="Transparent"
        Topmost="True"
        ShowInTaskbar="False"
        ResizeMode="NoResize"
        WindowStartupLocation="Manual"
        Left="{Binding WindowLeft, Mode=TwoWay}"
        Top="{Binding WindowTop, Mode=TwoWay}">
    
    <!-- 窗口内容 -->
    <Grid x:Name="MainContainer">
        <!-- 最小化状态 -->
        <UserControl x:Name="MinimizedView" 
                     Visibility="{Binding IsMinimized, Converter={StaticResource BoolToVisibilityConverter}}"/>
        
        <!-- 预览状态 -->
        <UserControl x:Name="PreviewView"
                     Visibility="{Binding IsPreview, Converter={StaticResource BoolToVisibilityConverter}}"/>
        
        <!-- 详细状态 -->
        <UserControl x:Name="DetailedView"
                     Visibility="{Binding IsDetailed, Converter={StaticResource BoolToVisibilityConverter}}"/>
    </Grid>
</Window>
```

### 2.2 窗口行为控制
```csharp
public partial class FloatingWindow : Window
{
    private bool _isDragging = false;
    private Point _dragStartPoint;
    
    public FloatingWindow()
    {
        InitializeComponent();
        this.Loaded += OnWindowLoaded;
        this.MouseDown += OnMouseDown;
        this.MouseMove += OnMouseMove;
        this.MouseUp += OnMouseUp;
    }
    
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        // 设置窗口初始位置
        PositionWindowToScreenEdge();
        
        // 启用毛玻璃效果
        EnableBlurEffect();
    }
    
    private void PositionWindowToScreenEdge()
    {
        var workingArea = SystemParameters.WorkArea;
        this.Left = workingArea.Right - this.Width - 20;
        this.Top = workingArea.Bottom - this.Height - 40;
    }
    
    private void EnableBlurEffect()
    {
        var windowHelper = new WindowBlurHelper();
        windowHelper.EnableBlur(this);
    }
}
```

## 3. 透明度和毛玻璃效果

### 3.1 Win32 API实现
```csharp
public class WindowBlurHelper
{
    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }
    
    internal enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }
    
    public void EnableBlur(Window window)
    {
        var windowHelper = new WindowInteropHelper(window);
        var accent = new AccentPolicy()
        {
            AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
        };
        
        var accentStructSize = Marshal.SizeOf(accent);
        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);
        
        var data = new WindowCompositionAttributeData()
        {
            Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
            SizeOfData = accentStructSize,
            Data = accentPtr
        };
        
        SetWindowCompositionAttribute(windowHelper.Handle, ref data);
        Marshal.FreeHGlobal(accentPtr);
    }
}
```

### 3.2 样式实现
```xml
<ResourceDictionary>
    <!-- 半透明背景样式 -->
    <Style x:Key="TransparentBackground" TargetType="Border">
        <Setter Property="Background">
            <Setter.Value>
                <SolidColorBrush Color="White" Opacity="0.85"/>
            </Setter.Value>
        </Setter>
        <Setter Property="Effect">
            <Setter.Value>
                <BlurEffect Radius="10"/>
            </Setter.Value>
        </Setter>
    </Style>
    
    <!-- 最小化状态样式 -->
    <Style x:Key="MinimizedStyle" TargetType="Ellipse">
        <Setter Property="Fill">
            <Setter.Value>
                <RadialGradientBrush>
                    <GradientStop Color="#2563EB" Offset="0"/>
                    <GradientStop Color="#1D4ED8" Offset="1"/>
                </RadialGradientBrush>
            </Setter.Value>
        </Setter>
        <Setter Property="Opacity" Value="0.7"/>
        <Setter Property="Effect">
            <Setter.Value>
                <DropShadowEffect Color="#2563EB" BlurRadius="8" Opacity="0.4"/>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```

## 4. 状态管理

### 4.1 状态枚举
```csharp
public enum WindowState
{
    Minimized,
    Preview,
    Detailed
}

public class WindowStateManager : INotifyPropertyChanged
{
    private WindowState _currentState = WindowState.Minimized;
    private Timer _autoCollapseTimer;
    
    public WindowState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                var oldState = _currentState;
                _currentState = value;
                OnStateChanged(oldState, value);
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsMinimized => CurrentState == WindowState.Minimized;
    public bool IsPreview => CurrentState == WindowState.Preview;
    public bool IsDetailed => CurrentState == WindowState.Detailed;
    
    private void OnStateChanged(WindowState oldState, WindowState newState)
    {
        // 重置自动收起计时器
        ResetAutoCollapseTimer();
        
        // 触发状态切换动画
        TriggerStateAnimation(oldState, newState);
        
        // 调整窗口尺寸
        UpdateWindowSize(newState);
    }
    
    private void ResetAutoCollapseTimer()
    {
        _autoCollapseTimer?.Stop();
        
        if (CurrentState != WindowState.Minimized)
        {
            _autoCollapseTimer = new Timer(5000) { AutoReset = false };
            _autoCollapseTimer.Elapsed += (s, e) => 
            {
                App.Current.Dispatcher.Invoke(() => CurrentState = WindowState.Minimized);
            };
            _autoCollapseTimer.Start();
        }
    }
}
```

### 4.2 尺寸动画
```csharp
public class WindowSizeAnimator
{
    public void AnimateToSize(Window window, Size targetSize, TimeSpan duration)
    {
        var widthAnimation = new DoubleAnimation
        {
            From = window.Width,
            To = targetSize.Width,
            Duration = duration,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        
        var heightAnimation = new DoubleAnimation
        {
            From = window.Height,
            To = targetSize.Height,
            Duration = duration,
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        
        window.BeginAnimation(FrameworkElement.WidthProperty, widthAnimation);
        window.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
    }
    
    public void AnimateOpacity(UIElement element, double targetOpacity, TimeSpan duration)
    {
        var opacityAnimation = new DoubleAnimation
        {
            To = targetOpacity,
            Duration = duration,
            EasingFunction = new QuadraticEase()
        };
        
        element.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
    }
}
```

## 5. 热键服务

### 5.1 全局热键注册
```csharp
public class GlobalHotkeyService
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
    
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    
    private const int HOTKEY_ID = 9000;
    private const int MOD_CTRL = 0x0002;
    private const int MOD_ALT = 0x0001;
    private const int VK_SPACE = 0x20;
    
    public void RegisterHotkeys(Window window)
    {
        var windowHelper = new WindowInteropHelper(window);
        var source = HwndSource.FromHwnd(windowHelper.Handle);
        source.AddHook(HwndHook);
        
        // 注册 Ctrl+Alt+Space 热键
        RegisterHotKey(windowHelper.Handle, HOTKEY_ID, MOD_CTRL | MOD_ALT, VK_SPACE);
    }
    
    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_HOTKEY = 0x0312;
        
        if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
        {
            OnHotkeyPressed?.Invoke();
            handled = true;
        }
        
        return IntPtr.Zero;
    }
    
    public event Action OnHotkeyPressed;
}
```

### 5.2 快捷键配置
```xml
<Window.InputBindings>
    <!-- ESC键返回上一状态 -->
    <KeyBinding Key="Escape" Command="{Binding CollapseCommand}"/>
    
    <!-- Enter键确认操作 -->
    <KeyBinding Key="Return" Command="{Binding ConfirmCommand}"/>
    
    <!-- Ctrl+N 添加任务 -->
    <KeyBinding Key="N" Modifiers="Ctrl" Command="{Binding AddTaskCommand}"/>
    
    <!-- Delete键删除任务 -->
    <KeyBinding Key="Delete" Command="{Binding DeleteTaskCommand}"/>
    
    <!-- F2键重命名任务 -->
    <KeyBinding Key="F2" Command="{Binding RenameTaskCommand}"/>
</Window.InputBindings>
```

## 6. 任务数据管理

### 6.1 数据模型
```csharp
public class TodoTask : INotifyPropertyChanged
{
    private string _title;
    private string _description;
    private DateTime? _dueDate;
    private TaskPriority _priority;
    private bool _isCompleted;
    private List<string> _tags;
    
    public int Id { get; set; }
    
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
    
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }
    
    public TaskPriority Priority
    {
        get => _priority;
        set => SetProperty(ref _priority, value);
    }
    
    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }
    
    public List<string> Tags
    {
        get => _tags ??= new List<string>();
        set => SetProperty(ref _tags, value);
    }
    
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && !IsCompleted;
    public bool IsDueToday => DueDate?.Date == DateTime.Today;
    
    public event PropertyChangedEventHandler PropertyChanged;
}

public enum TaskPriority
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}
```

### 6.2 数据服务
```csharp
public class TaskDataService
{
    private readonly string _dataFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        "FlexToDo", 
        "tasks.json");
    
    public async Task<List<TodoTask>> LoadTasksAsync()
    {
        if (!File.Exists(_dataFilePath))
            return new List<TodoTask>();
            
        try
        {
            var json = await File.ReadAllTextAsync(_dataFilePath);
            return JsonSerializer.Deserialize<List<TodoTask>>(json) ?? new List<TodoTask>();
        }
        catch (Exception ex)
        {
            // 记录错误，返回空列表
            Debug.WriteLine($"Failed to load tasks: {ex.Message}");
            return new List<TodoTask>();
        }
    }
    
    public async Task SaveTasksAsync(List<TodoTask> tasks)
    {
        try
        {
            var directory = Path.GetDirectoryName(_dataFilePath);
            Directory.CreateDirectory(directory);
            
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            await File.WriteAllTextAsync(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save tasks: {ex.Message}");
        }
    }
}
```

## 7. 自定义控件

### 7.1 任务项控件
```xml
<UserControl x:Class="FlexToDo.Controls.TaskItemControl">
    <Border Style="{StaticResource TaskItemBorderStyle}"
            Background="{Binding BackgroundBrush}"
            BorderBrush="{Binding BorderBrush}">
        
        <Grid Margin="12,8">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/> <!-- 复选框 -->
                <ColumnDefinition Width="*"/>    <!-- 内容 -->
                <ColumnDefinition Width="Auto"/> <!-- 操作 -->
            </Grid.ColumnDefinitions>
            
            <!-- 完成状态复选框 -->
            <CheckBox Grid.Column="0" 
                      IsChecked="{Binding IsCompleted}"
                      Style="{StaticResource TaskCheckBoxStyle}"
                      Margin="0,0,8,0"/>
            
            <!-- 任务内容 -->
            <StackPanel Grid.Column="1">
                <TextBlock Text="{Binding Title}" 
                           Style="{StaticResource TaskTitleStyle}"
                           TextDecorations="{Binding IsCompleted, Converter={StaticResource StrikethroughConverter}}"/>
                
                <TextBlock Text="{Binding Description}" 
                           Style="{StaticResource TaskDescriptionStyle}"
                           Visibility="{Binding Description, Converter={StaticResource StringToVisibilityConverter}}"/>
                
                <StackPanel Orientation="Horizontal" Margin="0,4,0,0">
                    <!-- 优先级指示 -->
                    <Ellipse Width="8" Height="8" 
                             Fill="{Binding Priority, Converter={StaticResource PriorityColorConverter}}"
                             Margin="0,0,6,0"/>
                    
                    <!-- 时间显示 -->
                    <TextBlock Text="{Binding DueDate, StringFormat='MM/dd HH:mm'}"
                               Style="{StaticResource TaskMetaStyle}"/>
                    
                    <!-- 标签显示 -->
                    <ItemsControl ItemsSource="{Binding Tags}" Margin="8,0,0,0">
                        <ItemsControl.ItemsPanel>
                            <ItemsPanelTemplate>
                                <StackPanel Orientation="Horizontal"/>
                            </ItemsPanelTemplate>
                        </ItemsControl.ItemsPanel>
                        <ItemsControl.ItemTemplate>
                            <DataTemplate>
                                <Border Style="{StaticResource TagStyle}" Margin="0,0,4,0">
                                    <TextBlock Text="{Binding}" Style="{StaticResource TagTextStyle}"/>
                                </Border>
                            </DataTemplate>
                        </ItemsControl.ItemTemplate>
                    </ItemsControl>
                </StackPanel>
            </StackPanel>
            
            <!-- 操作按钮 -->
            <StackPanel Grid.Column="2" Orientation="Horizontal">
                <Button Command="{Binding EditCommand}" 
                        Style="{StaticResource IconButtonStyle}"
                        ToolTip="编辑任务">
                    <Image Source="{StaticResource EditIcon}" Width="14" Height="14"/>
                </Button>
                
                <Button Command="{Binding DeleteCommand}" 
                        Style="{StaticResource IconButtonStyle}"
                        ToolTip="删除任务">
                    <Image Source="{StaticResource DeleteIcon}" Width="14" Height="14"/>
                </Button>
            </StackPanel>
        </Grid>
    </Border>
</UserControl>
```

### 7.2 快速添加控件
```xml
<UserControl x:Class="FlexToDo.Controls.QuickAddControl">
    <Border Style="{StaticResource QuickAddBorderStyle}">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            
            <Button Grid.Column="0" 
                    Content="+" 
                    Style="{StaticResource AddButtonStyle}"
                    Command="{Binding ShowAddFormCommand}"/>
            
            <TextBox Grid.Column="1" 
                     Text="{Binding QuickTaskTitle, UpdateSourceTrigger=PropertyChanged}"
                     Style="{StaticResource QuickAddTextBoxStyle}"
                     Tag="快速添加任务..."/>
            
            <Button Grid.Column="2"
                    Content="添加"
                    Style="{StaticResource PrimaryButtonStyle}"
                    Command="{Binding AddQuickTaskCommand}"
                    Visibility="{Binding HasQuickTaskTitle, Converter={StaticResource BoolToVisibilityConverter}}"/>
        </Grid>
    </Border>
</UserControl>
```

## 8. DPI适配

### 8.1 DPI感知设置
```xml
<!-- app.manifest -->
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/pm</dpiAware>
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">permonitorv2,permonitor</dpiAwareness>
  </windowsSettings>
</application>
```

### 8.2 DPI助手类
```csharp
public class DpiHelper
{
    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);
    
    public static double GetDpiScale(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        var dpi = GetDpiForWindow(hwnd);
        return dpi / 96.0;
    }
    
    public static Size ScaleSize(Size size, double scale)
    {
        return new Size(size.Width * scale, size.Height * scale);
    }
    
    public static Thickness ScaleThickness(Thickness thickness, double scale)
    {
        return new Thickness(
            thickness.Left * scale,
            thickness.Top * scale,
            thickness.Right * scale,
            thickness.Bottom * scale);
    }
}
```

## 9. 性能优化

### 9.1 虚拟化列表
```xml
<ListBox ItemsSource="{Binding Tasks}"
         VirtualizingPanel.IsVirtualizing="True"
         VirtualizingPanel.VirtualizationMode="Recycling"
         VirtualizingPanel.ScrollUnit="Item">
    
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <VirtualizingStackPanel/>
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
    
    <ListBox.ItemTemplate>
        <DataTemplate>
            <local:TaskItemControl DataContext="{Binding}"/>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### 9.2 异步加载
```csharp
public class MainViewModel : BaseViewModel
{
    private readonly TaskDataService _dataService;
    private ObservableCollection<TodoTask> _tasks;
    
    public ObservableCollection<TodoTask> Tasks
    {
        get => _tasks ??= new ObservableCollection<TodoTask>();
        set => SetProperty(ref _tasks, value);
    }
    
    public async Task LoadTasksAsync()
    {
        try
        {
            IsLoading = true;
            var tasks = await _dataService.LoadTasksAsync();
            
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public async Task SaveTasksAsync()
    {
        await _dataService.SaveTasksAsync(Tasks.ToList());
    }
}
```

## 10. 部署配置

### 10.1 应用配置
```xml
<!-- App.config -->
<configuration>
  <appSettings>
    <add key="AutoStart" value="true"/>
    <add key="MinimizeToTray" value="true"/>
    <add key="AutoCollapseDelay" value="5000"/>
    <add key="DataBackupEnabled" value="true"/>
  </appSettings>
  
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/>
  </startup>
</configuration>
```

### 10.2 安装配置
```csharp
// 开机自启动
public class AutoStartupHelper
{
    public static void SetAutoStartup(bool enable)
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(
            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            
        if (enable)
        {
            registryKey?.SetValue("FlexToDo", Assembly.GetExecutingAssembly().Location);
        }
        else
        {
            registryKey?.DeleteValue("FlexToDo", false);
        }
        
        registryKey?.Close();
    }
}
```

---

这个WPF实现指南提供了FlexToDo应用的完整技术实现方案，涵盖了从架构设计到部署的各个方面，确保开发团队能够高效地构建出符合设计要求的悬浮待办工具。