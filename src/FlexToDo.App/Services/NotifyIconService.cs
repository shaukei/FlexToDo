using System.Drawing;
using System.Windows.Forms;

namespace FlexToDo.App.Services;

/// <summary>
/// 系统托盘图标服务
/// </summary>
public class NotifyIconService : IDisposable
{
    private NotifyIcon? _notifyIcon;
    private readonly Icon _defaultIcon;
    private readonly Icon _urgentIcon;
    
    public event EventHandler? ShowMainWindow;
    public event EventHandler? ExitApplication;

    public NotifyIconService()
    {
        // 创建默认图标（简单的圆形）
        _defaultIcon = CreateDefaultIcon();
        _urgentIcon = CreateUrgentIcon();
    }

    /// <summary>
    /// 初始化托盘图标
    /// </summary>
    public void Initialize()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = _defaultIcon,
            Text = "FlexToDo - 悬浮待办事项工具",
            Visible = true
        };

        // 设置上下文菜单
        var contextMenu = new ContextMenuStrip();
        
        var showItem = new ToolStripMenuItem("显示主窗口");
        showItem.Click += (s, e) => ShowMainWindow?.Invoke(this, EventArgs.Empty);
        contextMenu.Items.Add(showItem);
        
        contextMenu.Items.Add(new ToolStripSeparator());
        
        var settingsItem = new ToolStripMenuItem("设置");
        settingsItem.Click += (s, e) => ShowSettings();
        contextMenu.Items.Add(settingsItem);
        
        var aboutItem = new ToolStripMenuItem("关于");
        aboutItem.Click += (s, e) => ShowAbout();
        contextMenu.Items.Add(aboutItem);
        
        contextMenu.Items.Add(new ToolStripSeparator());
        
        var exitItem = new ToolStripMenuItem("退出");
        exitItem.Click += (s, e) => ExitApplication?.Invoke(this, EventArgs.Empty);
        contextMenu.Items.Add(exitItem);

        _notifyIcon.ContextMenuStrip = contextMenu;

        // 双击显示主窗口
        _notifyIcon.DoubleClick += (s, e) => ShowMainWindow?.Invoke(this, EventArgs.Empty);

        System.Diagnostics.Debug.WriteLine("系统托盘图标初始化完成");
    }

    /// <summary>
    /// 显示提示消息
    /// </summary>
    public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info, int timeout = 3000)
    {
        _notifyIcon?.ShowBalloonTip(timeout, title, text, icon);
    }

    /// <summary>
    /// 更新图标状态
    /// </summary>
    public void UpdateIconStatus(bool hasUrgentTodos)
    {
        if (_notifyIcon == null) return;

        _notifyIcon.Icon = hasUrgentTodos ? _urgentIcon : _defaultIcon;
        
        var tooltip = hasUrgentTodos 
            ? "FlexToDo - 有紧急待办事项！"
            : "FlexToDo - 悬浮待办事项工具";
            
        _notifyIcon.Text = tooltip;
    }

    /// <summary>
    /// 显示紧急提醒
    /// </summary>
    public void ShowUrgentNotification(string todoTitle)
    {
        ShowBalloonTip("紧急提醒", $"待办事项即将到期: {todoTitle}", ToolTipIcon.Warning, 5000);
        UpdateIconStatus(true);
    }

    /// <summary>
    /// 显示设置界面
    /// </summary>
    private void ShowSettings()
    {
        // TODO: 实现设置界面
        System.Windows.MessageBox.Show("设置功能即将推出！", "FlexToDo", 
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    /// <summary>
    /// 显示关于界面
    /// </summary>
    private void ShowAbout()
    {
        var aboutText = "FlexToDo v1.0.0\n\n" +
                       "一个轻量级的悬浮待办事项工具\n" +
                       "让您随时了解重要事项，不错过任何deadline\n\n" +
                       "快捷键:\n" +
                       "• Ctrl+Alt+Space: 切换主界面\n" +
                       "• Ctrl+Alt+N: 快速添加\n" +
                       "• Ctrl+Alt+V: 快速查看\n" +
                       "• Ctrl+Alt+C: 清除已完成\n\n" +
                       "开发: Claude Code Assistant";

        System.Windows.MessageBox.Show(aboutText, "关于 FlexToDo", 
            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    /// <summary>
    /// 创建默认图标
    /// </summary>
    private Icon CreateDefaultIcon()
    {
        var bitmap = new Bitmap(16, 16);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            // 绘制蓝色圆形
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.FromArgb(37, 99, 235))) // #2563EB
            {
                graphics.FillEllipse(brush, 2, 2, 12, 12);
            }
        }
        
        return Icon.FromHandle(bitmap.GetHicon());
    }

    /// <summary>
    /// 创建紧急状态图标
    /// </summary>
    private Icon CreateUrgentIcon()
    {
        var bitmap = new Bitmap(16, 16);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            // 绘制红色圆形
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(Color.FromArgb(239, 68, 68))) // #ef4444
            {
                graphics.FillEllipse(brush, 2, 2, 12, 12);
            }
            
            // 绘制感叹号
            using (var pen = new Pen(Color.White, 1))
            {
                graphics.DrawLine(pen, 8, 5, 8, 9);
                graphics.DrawEllipse(pen, 7, 11, 2, 2);
            }
        }
        
        return Icon.FromHandle(bitmap.GetHicon());
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _notifyIcon?.Dispose();
        _defaultIcon?.Dispose();
        _urgentIcon?.Dispose();
        
        System.Diagnostics.Debug.WriteLine("系统托盘图标服务已释放");
    }
}