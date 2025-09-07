using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApplication = System.Windows.Application;
using WpfMessageBox = System.Windows.MessageBox;

namespace FlexToDo.App;

/// <summary>
/// 应用程序入口点
/// </summary>
public static class Program
{
    private static Mutex? _mutex;
    
    /// <summary>
    /// 应用程序主入口点
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // 确保单实例运行
            if (!EnsureSingleInstance())
            {
                WpfMessageBox.Show("Flex ToDo 已在运行中。", "Flex ToDo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 启用高DPI支持
            SetProcessDpiAwareness();

            // 创建并运行WPF应用程序
            var app = new App();
            
            // 设置异常处理（在Application对象创建后）
            SetupExceptionHandling();
            
            app.Run();
        }
        catch (Exception ex)
        {
            HandleFatalException(ex);
        }
        finally
        {
            // 释放互斥锁
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }

    /// <summary>
    /// 确保单实例运行
    /// </summary>
    private static bool EnsureSingleInstance()
    {
        const string mutexName = "FlexToDo_SingleInstance_Mutex";
        
        _mutex = new Mutex(true, mutexName, out bool createdNew);
        
        if (!createdNew)
        {
            // 应用程序已在运行，尝试激活现有实例
            try
            {
                // 这里可以实现进程间通信，激活现有窗口
                // 为简化实现，这里只显示提示信息
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"激活现有实例失败: {ex.Message}");
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// 设置全局异常处理
    /// </summary>
    private static void SetupExceptionHandling()
    {
        // 处理UI线程异常
        WpfApplication.Current.DispatcherUnhandledException += (sender, e) =>
        {
            HandleException(e.Exception, "UI线程异常");
            e.Handled = true; // 阻止应用程序崩溃
        };

        // 处理非UI线程异常
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            HandleException(e.ExceptionObject as Exception, "非UI线程异常");
        };

        // 处理Task异常
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            HandleException(e.Exception, "Task异常");
            e.SetObserved(); // 标记异常已处理
        };
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    private static void HandleException(Exception? ex, string context)
    {
        if (ex == null) return;

        var message = $"[{context}] {ex.Message}";
        
        Debug.WriteLine($"异常: {message}");
        Debug.WriteLine($"堆栈跟踪: {ex.StackTrace}");

        // 记录到文件（可选）
        try
        {
            LogExceptionToFile(ex, context);
        }
        catch
        {
            // 忽略日志记录异常
        }

        // 非致命异常不显示给用户，只记录日志
        // 致命异常会通过HandleFatalException处理
    }

    /// <summary>
    /// 处理致命异常
    /// </summary>
    private static void HandleFatalException(Exception ex)
    {
        var message = $"应用程序遇到致命错误: {ex.Message}\n\n是否要查看详细信息?";
        
        var result = WpfMessageBox.Show(message, "Flex ToDo - 致命错误", 
            MessageBoxButton.YesNo, MessageBoxImage.Error);

        if (result == MessageBoxResult.Yes)
        {
            var detailMessage = $"异常类型: {ex.GetType().Name}\n" +
                               $"异常消息: {ex.Message}\n" +
                               $"堆栈跟踪:\n{ex.StackTrace}";
            
            WpfMessageBox.Show(detailMessage, "异常详细信息", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // 记录致命异常
        try
        {
            LogExceptionToFile(ex, "致命异常");
        }
        catch
        {
            // 忽略日志记录异常
        }
    }

    /// <summary>
    /// 将异常记录到文件
    /// </summary>
    private static void LogExceptionToFile(Exception ex, string context)
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var logDirectory = Path.Combine(appDataPath, "FlexToDo", "Logs");
            Directory.CreateDirectory(logDirectory);

            var logFile = Path.Combine(logDirectory, $"error_{DateTime.Now:yyyyMMdd}.log");
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{context}]\n" +
                          $"异常: {ex.Message}\n" +
                          $"类型: {ex.GetType().Name}\n" +
                          $"堆栈: {ex.StackTrace}\n" +
                          $"{new string('-', 80)}\n";

            File.AppendAllText(logFile, logEntry);
        }
        catch
        {
            // 如果无法写入日志文件，忽略错误
        }
    }

    /// <summary>
    /// 设置进程DPI感知
    /// </summary>
    private static void SetProcessDpiAwareness()
    {
        try
        {
            // Windows 10 1703及更高版本
            if (Environment.OSVersion.Version >= new Version(10, 0, 15063))
            {
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }
            // Windows 8.1及更高版本
            else if (Environment.OSVersion.Version >= new Version(6, 3))
            {
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
            }
            // Windows Vista及更高版本
            else if (Environment.OSVersion.Version >= new Version(6, 0))
            {
                SetProcessDPIAware();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"设置DPI感知失败: {ex.Message}");
            // 非致命错误，继续运行
        }
    }

    #region DPI相关的Win32 API

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

    private enum PROCESS_DPI_AWARENESS
    {
        PROCESS_DPI_UNAWARE = 0,
        PROCESS_SYSTEM_DPI_AWARE = 1,
        PROCESS_PER_MONITOR_DPI_AWARE = 2
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiAwarenessContext);

    [System.Runtime.InteropServices.DllImport("shcore.dll")]
    private static extern int SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    #endregion
}