using System.Windows;
using System.Windows.Input;
using FlexToDo.UI.ViewModels;
using FlexToDo.Infrastructure.Win32;

namespace FlexToDo.UI.Views;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private readonly GlobalHotkeyManager _hotkeyManager;
    private WindowStateController? _stateController;

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
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("主窗口加载完成");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"窗口加载失败: {ex.Message}");
            MessageBox.Show($"应用程序加载失败: {ex.Message}", "FlexToDo", MessageBoxButton.OK, MessageBoxImage.Error);
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
            
            System.Diagnostics.Debug.WriteLine("窗口句柄已创建，热键管理器和窗口状态初始化完成");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"设置初始窗口状态失败: {ex.Message}");
            MessageBox.Show($"热键初始化失败: {ex.Message}", "FlexToDo", MessageBoxButton.OK, MessageBoxImage.Error);
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
    /// 更新状态控制器引用（用于解决循环依赖问题）
    /// </summary>
    public void UpdateStateController(WindowStateController stateController)
    {
        _stateController = stateController ?? throw new ArgumentNullException(nameof(stateController));
        System.Diagnostics.Debug.WriteLine("MainWindow StateController 引用已更新");
    }
}