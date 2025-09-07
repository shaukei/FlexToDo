using FlexToDo.App.Services;
using FlexToDo.Core.Interfaces;
using FlexToDo.Core.Services;
using FlexToDo.Infrastructure.Storage;
using FlexToDo.Infrastructure.Win32;
using FlexToDo.UI.ViewModels;
using FlexToDo.UI.Views;
using System.Windows;
using WpfApplication = System.Windows.Application;
using WpfMessageBox = System.Windows.MessageBox;

namespace FlexToDo.App;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : WpfApplication
{
    private MainWindow? _mainWindow;
    private NotifyIconService? _notifyIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // 初始化系统托盘
            InitializeNotifyIcon();
            
            // 创建并显示主窗口
            InitializeMainWindow();
            
            System.Diagnostics.Debug.WriteLine("FlexToDo 应用程序启动成功");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"应用程序启动失败: {ex.Message}");
            WpfMessageBox.Show($"应用程序启动失败: {ex.Message}", "Flex ToDo", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            // 清理资源
            _notifyIcon?.Dispose();
            _mainWindow?.SafeClose();
            
            System.Diagnostics.Debug.WriteLine("FlexToDo 应用程序正常退出");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"应用程序退出时出现错误: {ex.Message}");
        }
        
        base.OnExit(e);
    }

    /// <summary>
    /// 初始化系统托盘
    /// </summary>
    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIconService();
        _notifyIcon.Initialize();
        
        // 订阅托盘事件
        _notifyIcon.ShowMainWindow += (s, e) => _mainWindow?.ForceShow();
        _notifyIcon.ExitApplication += (s, e) => Shutdown();

        System.Diagnostics.Debug.WriteLine("系统托盘初始化完成");
    }

    /// <summary>
    /// 初始化主窗口
    /// </summary>
    private void InitializeMainWindow()
    {
        // 创建服务实例（简化的手动依赖注入）
        var storageService = new LocalStorageService();
        var todoService = new TodoService(storageService);
        var hotkeyManager = new GlobalHotkeyManager();
        
        // 先创建一个临时的状态控制器（不需要窗口实例）
        WindowStateController? tempStateController = null;
        
        // 创建ViewModel（使用临时状态控制器）
        var viewModel = new MainWindowViewModel(todoService, tempStateController!);
        
        // 创建主窗口（现在有了有效的ViewModel）
        _mainWindow = new MainWindow(viewModel, hotkeyManager, tempStateController!);
        
        // 创建真正的状态控制器（现在有了窗口实例）
        var stateController = new WindowStateController(_mainWindow);
        
        // 更新ViewModel和MainWindow的状态控制器引用
        viewModel.UpdateStateController(stateController);
        _mainWindow.UpdateStateController(stateController);

        // 设置为主窗口
        MainWindow = _mainWindow;
        
        // 显示窗口
        _mainWindow.ForceShow();

        System.Diagnostics.Debug.WriteLine("主窗口初始化完成");
    }

    /// <summary>
    /// 获取主窗口实例（用于调试和测试）
    /// </summary>
    internal MainWindow? MainWindowInstance => _mainWindow;
}