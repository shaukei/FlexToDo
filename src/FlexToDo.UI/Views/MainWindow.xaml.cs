using System.Windows;
using System.Windows.Input;
using FlexToDo.UI.ViewModels;
using FlexToDo.Infrastructure.Win32;
using FlexToDo.Core.Models;

namespace FlexToDo.UI.Views;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private readonly GlobalHotkeyManager _hotkeyManager;
    private WindowStateController? _stateController;
    private GlobalMouseMonitor? _mouseMonitor;

    public MainWindow(MainWindowViewModel viewModel, GlobalHotkeyManager hotkeyManager, WindowStateController? stateController)
    {
        InitializeComponent();

        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _hotkeyManager = hotkeyManager ?? throw new ArgumentNullException(nameof(hotkeyManager));
        _stateController = stateController;

        // 设置窗口初始位置（右边缘）
        SetWindowPosition();

        // 窗口加载完成后初始化
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
        
        // 键盘事件处理
        KeyDown += MainWindow_KeyDown;

        // 在构造函数中直接创建全局鼠标监听器
        Console.WriteLine("[FlexToDo] MainWindow constructor called");
        
        // 由于背景穿透模式会阻止WPF鼠标事件，我们必须使用Win32全局钩子
        _mouseMonitor = new FlexToDo.Infrastructure.Win32.GlobalMouseMonitor();
        Console.WriteLine("[FlexToDo] GlobalMouseMonitor created as instance member");
        
        _mouseMonitor.MouseMove += (sender, e) => {
            try 
            {
                // 检查鼠标是否在窗口内
                var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero && FlexToDo.Infrastructure.Win32.Win32Api.GetWindowRect(hwnd, out var rect))
                {
                    bool isInWindow = e.ScreenPosition.X >= rect.Left && e.ScreenPosition.X <= rect.Right &&
                                      e.ScreenPosition.Y >= rect.Top && e.ScreenPosition.Y <= rect.Bottom;
                    
                    if (isInWindow)
                    {
                        // 转换为窗口相对坐标
                        var windowPosition = new System.Windows.Point(
                            e.ScreenPosition.X - rect.Left,
                            e.ScreenPosition.Y - rect.Top);
                        
                        Console.WriteLine($"[FlexToDo] Mouse over window at screen({e.ScreenPosition.X}, {e.ScreenPosition.Y}) window({windowPosition.X}, {windowPosition.Y})");
                        
                        Application.Current.Dispatcher.BeginInvoke(() => {
                            // 设置窗口背景高亮
                            this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 158, 11)) { Opacity = 0.2 };
                            
                            // 检测具体悬停的待办事项
                            var hoveredItem = HitTestTodoItem(windowPosition);
                            if (DataContext is MainWindowViewModel viewModel)
                            {
                                viewModel.HoveredTodoItem = hoveredItem;
                            }
                        });
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(() => {
                            this.Background = System.Windows.Media.Brushes.Transparent;
                            
                            // 清除悬停状态
                            if (DataContext is MainWindowViewModel viewModel)
                            {
                                viewModel.HoveredTodoItem = null;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FlexToDo] MouseMove error: {ex.Message}");
            }
        };
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[FlexToDo] MainWindow loaded successfully");
            
            // 初始化鼠标悬停管理器
            System.Diagnostics.Debug.WriteLine($"[FlexToDo] DataContext type: {DataContext?.GetType().Name}");
            if (DataContext is MainWindowViewModel viewModel)
            {
                System.Diagnostics.Debug.WriteLine("[FlexToDo] Initializing mouse hover manager...");
                viewModel.InitializeMouseHoverManager(this);
                // 在背景模式下启用悬停检测
                viewModel.EnableMouseHover();
                System.Diagnostics.Debug.WriteLine("[FlexToDo] Mouse hover manager initialized successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[FlexToDo] ERROR: DataContext is not MainWindowViewModel");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"窗口加载失败: {ex.Message}");
            MessageBox.Show($"应用程序加载失败: {ex.Message}", "Flex ToDo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // 最小化到托盘而不是关闭
        e.Cancel = true;
        Hide();
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        // Esc键切换到背景穿透模式
        if (e.Key == Key.Escape && _stateController?.IsInteractionMode == true)
        {
            _ = _stateController.SwitchToBackgroundModeAsync();
            e.Handled = true;
        }
    }

    private void HotkeyManager_HotkeyTriggered(object? sender, HotkeyTriggeredEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            try
            {
                switch (e.HotkeyInfo.Id)
                {
                    case "ToggleMain":
                        Console.WriteLine($"[FlexToDo] 处理ToggleMain热键, StateController为null: {_stateController == null}");
                        if (_stateController != null)
                        {
                            await _stateController.ActivateWindowAsync();
                            QuickAddTextBox.Focus();
                        }
                        else
                        {
                            Console.WriteLine("[FlexToDo] 错误：StateController为null，无法激活窗口");
                        }
                        break;

                    case "QuickAdd":
                        Console.WriteLine($"[FlexToDo] 处理QuickAdd热键, StateController为null: {_stateController == null}");
                        if (_stateController != null)
                        {
                            await _stateController.ActivateWindowAsync();
                            QuickAddTextBox.Focus();
                        }
                        else
                        {
                            Console.WriteLine("[FlexToDo] 错误：StateController为null，无法激活窗口");
                        }
                        break;

                    case "QuickView":
                        Console.WriteLine($"[FlexToDo] 处理QuickView热键, StateController为null: {_stateController == null}");
                        if (_stateController != null)
                        {
                            await _stateController.ActivateWindowAsync();
                        }
                        break;

                    case "ClearCompleted":
                        if (DataContext is MainWindowViewModel viewModel)
                        {
                            viewModel.ClearCompletedCommand?.Execute(null);
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"未处理的热键: {e.HotkeyInfo.Id}");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"处理热键事件失败: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 设置窗口位置到屏幕右边缘
    /// </summary>
    private void SetWindowPosition()
    {
        var (screenWidth, screenHeight) = Win32Api.GetScreenSize();
        
        // 窗口位置：右边缘，垂直居中
        Left = screenWidth - Width;
        Top = (screenHeight - Height) / 2;
        
        System.Diagnostics.Debug.WriteLine($"窗口位置: ({Left}, {Top}), 屏幕尺寸: ({screenWidth}, {screenHeight})");
    }

    /// <summary>
    /// 获取窗口状态控制器
    /// </summary>
    public WindowStateController? StateController => _stateController;

    /// <summary>
    /// 获取热键管理器
    /// </summary>
    public GlobalHotkeyManager HotkeyManager => _hotkeyManager;

    /// <summary>
    /// 强制显示窗口（用于系统托盘恢复）
    /// </summary>
    public void ForceShow()
    {
        Show();
        WindowState = WindowState.Normal;
        Topmost = true;
        
        // 确保窗口位置正确
        SetWindowPosition();
        
        System.Diagnostics.Debug.WriteLine("强制显示窗口");
    }

    /// <summary>
    /// 安全关闭窗口
    /// </summary>
    public void SafeClose()
    {
        try
        {
            _mouseMonitor?.Dispose();
            _hotkeyManager?.Dispose();
            _stateController?.Dispose();
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"安全关闭失败: {ex.Message}");
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        // 窗口句柄创建后，初始化热键管理器和窗口状态
        try
        {
            // 初始化热键管理器
            _hotkeyManager.Initialize(this);
            _hotkeyManager.HotkeyTriggered += HotkeyManager_HotkeyTriggered;
            
            // 注册默认热键
            _hotkeyManager.RegisterDefaultHotkeys();
            
            // 窗口句柄创建完成后，强制设置为背景穿透模式
            if (_stateController != null)
            {
                _stateController.ForceBackgroundMode();
                System.Diagnostics.Debug.WriteLine("窗口句柄创建后，强制设置背景穿透模式");
            }

            // 启动全局鼠标监听器（窗口句柄创建后）
            Console.WriteLine("[FlexToDo] OnSourceInitialized: Starting mouse monitoring...");
            if (_mouseMonitor != null)
            {
                bool started = _mouseMonitor.StartMonitoring();
                Console.WriteLine($"[FlexToDo] Mouse monitoring started: {started}");
                if (!started)
                {
                    Console.WriteLine("[FlexToDo] WARNING: Failed to start mouse monitoring");
                }
            }
            else
            {
                Console.WriteLine("[FlexToDo] ERROR: Mouse monitor is null");
            }

            // 强制初始化鼠标悬停管理器
            System.Diagnostics.Debug.WriteLine("[FlexToDo] OnSourceInitialized: Force initializing mouse hover");
            if (DataContext is MainWindowViewModel viewModel)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[FlexToDo] Creating mouse hover manager...");
                    viewModel.InitializeMouseHoverManager(this);
                    System.Diagnostics.Debug.WriteLine("[FlexToDo] Enabling mouse hover detection...");
                    viewModel.EnableMouseHover();
                    System.Diagnostics.Debug.WriteLine("[FlexToDo] Mouse hover setup completed in OnSourceInitialized");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[FlexToDo] Mouse hover setup failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"[FlexToDo] Stack trace: {ex.StackTrace}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[FlexToDo] DataContext is not MainWindowViewModel: {DataContext?.GetType().Name}");
            }
            
            System.Diagnostics.Debug.WriteLine("窗口句柄已创建，热键管理器和窗口状态初始化完成");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"设置初始窗口状态失败: {ex.Message}");
            MessageBox.Show($"热键初始化失败: {ex.Message}", "Flex ToDo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// 重置自动返回定时器（用户交互时调用）
    /// </summary>
    public void ResetIdleTimer()
    {
        _stateController?.ResetAutoReturnTimer();
    }

    /// <summary>
    /// 初始化鼠标悬停管理器（在构造函数中调用）
    /// </summary>
    private void InitializeMouseHoverManager(MainWindowViewModel viewModel)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[FlexToDo] Starting mouse hover manager initialization");
            viewModel.InitializeMouseHoverManager(this);
            System.Diagnostics.Debug.WriteLine("[FlexToDo] Mouse hover manager initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[FlexToDo] Mouse hover manager initialization failed: {ex.Message}");
        }
    }

    /// <summary>
    /// 处理鼠标交互
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        
        // 重置自动返回定时器（如果处于交互模式）
        if (_stateController?.IsInteractionMode == true)
        {
            ResetIdleTimer();
        }
    }

    /// <summary>
    /// 处理键盘交互
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        // 重置自动返回定时器（如果处于交互模式）
        if (_stateController?.IsInteractionMode == true)
        {
            ResetIdleTimer();
        }
    }

    /// <summary>
    /// 命中测试：确定鼠标位置对应的待办项
    /// </summary>
    private TodoItem? HitTestTodoItem(Point windowPosition)
    {
        try
        {
            if (DataContext is not MainWindowViewModel viewModel)
                return null;

            // 基于UI布局的近似计算
            // 标题栏高度约50px，边距16px
            double contentStartY = 50 + 16;
            double leftMargin = 16;
            double rightMargin = 16;
            double itemHeight = 60; // 每个待办项约60px高（包含边距）
            
            // 检查是否在内容区域内
            if (windowPosition.Y < contentStartY || 
                windowPosition.X < leftMargin || 
                windowPosition.X > (Width - rightMargin))
            {
                return null;
            }

            // 计算当前的垂直偏移
            double relativeY = windowPosition.Y - contentStartY;
            
            // 获取所有待办项（按显示顺序）
            var allTodos = new List<TodoItem>();
            
            if (viewModel.CriticalTodos != null)
                allTodos.AddRange(viewModel.CriticalTodos);
            if (viewModel.HighTodos != null)
                allTodos.AddRange(viewModel.HighTodos);
            if (viewModel.TodayTodos != null)
                allTodos.AddRange(viewModel.TodayTodos);

            // 根据垂直位置确定悬停的待办项
            int itemIndex = (int)(relativeY / itemHeight);
            
            if (itemIndex >= 0 && itemIndex < allTodos.Count)
            {
                var item = allTodos[itemIndex];
                Console.WriteLine($"[FlexToDo] Hit test found item: {item.Title} at index {itemIndex}");
                return item;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FlexToDo] Hit test error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 更新状态控制器引用（用于解决循环依赖问题）
    /// </summary>
    public void UpdateStateController(WindowStateController stateController)
    {
        _stateController = stateController ?? throw new ArgumentNullException(nameof(stateController));
        
        // 启用鼠标悬停检测
        if (DataContext is MainWindowViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine("[FlexToDo] Enabling mouse hover detection after StateController update");
            viewModel.EnableMouseHover();
        }
        
        System.Diagnostics.Debug.WriteLine("MainWindow StateController 引用已更新");
    }

    /// <summary>
    /// 输入框获得焦点事件处理
    /// </summary>
    private void QuickAddTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.InputGotFocusCommand?.Execute(null);
        }
    }

    /// <summary>
    /// 输入框失去焦点事件处理
    /// </summary>
    private void QuickAddTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.InputLostFocusCommand?.Execute(null);
        }
    }
}