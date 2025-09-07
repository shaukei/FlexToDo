using FlexToDo.Core.Interfaces;
using FlexToDo.Core.Models;
using FlexToDo.Infrastructure.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FlexToDo.UI.ViewModels;

/// <summary>
/// 主窗口视图模型
/// </summary>
public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ITodoService _todoService;
    private WindowStateController? _stateController;
    private string _quickAddText = string.Empty;

    public MainWindowViewModel(ITodoService todoService, WindowStateController? stateController)
    {
        _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
        _stateController = stateController;

        // 订阅服务变化
        _todoService.PropertyChanged += TodoService_PropertyChanged;
        if (_stateController != null)
        {
            _stateController.PropertyChanged += StateController_PropertyChanged;
        }

        // 初始化命令
        InitializeCommands();

        // 加载数据
        _ = LoadDataAsync();
    }

    #region 属性

    /// <summary>
    /// 快速添加文本
    /// </summary>
    public string QuickAddText
    {
        get => _quickAddText;
        set
        {
            if (SetProperty(ref _quickAddText, value))
            {
                OnPropertyChanged(nameof(IsQuickAddEmpty));
            }
        }
    }

    /// <summary>
    /// 快速添加文本是否为空
    /// </summary>
    public bool IsQuickAddEmpty => string.IsNullOrWhiteSpace(QuickAddText);

    /// <summary>
    /// 是否为背景模式（已废弃，界面始终显示列表）
    /// </summary>
    public bool IsBackgroundMode => false;

    /// <summary>
    /// 是否为交互模式（已废弃，界面始终显示列表）
    /// </summary>
    public bool IsInteractionMode => true;

    /// <summary>
    /// 窗口左边距（屏幕右边缘）
    /// </summary>
    public double WindowLeft
    {
        get
        {
            var (screenWidth, _) = Win32Api.GetScreenSize();
            // 方案2：始终为280px宽度
            return screenWidth - 280;
        }
    }

    /// <summary>
    /// 窗口顶部距离（屏幕中央）
    /// </summary>
    public double WindowTop
    {
        get
        {
            var (_, screenHeight) = Win32Api.GetScreenSize();
            return (screenHeight - 600) / 2;
        }
    }

    /// <summary>
    /// 窗口宽度
    /// </summary>
    public double WindowWidth
    {
        get
        {
            // 方案2：背景模式和交互模式都是280px宽度
            return 280;
        }
    }

    /// <summary>
    /// 标题栏主标题
    /// </summary>
    public string HeaderTitle
    {
        get
        {
            var urgentCount = CriticalTodos.Count;
            var totalCount = _todoService.PendingCount;

            if (urgentCount > 0)
                return $"🔥 {urgentCount} 个紧急事项";
            else if (totalCount > 0)
                return $"📋 {totalCount} 个待办事项";
            else
                return "✅ 全部完成";
        }
    }

    /// <summary>
    /// 标题栏副标题
    /// </summary>
    public string HeaderSubtitle
    {
        get
        {
            var now = DateTime.Now;
            return $"{now:MM月dd日} · {now:dddd}";
        }
    }

    /// <summary>
    /// 紧急待办事项
    /// </summary>
    public ObservableCollection<TodoItem> CriticalTodos
    {
        get
        {
            var items = _todoService.Todos
                .Where(t => !t.IsCompleted && t.Urgency == UrgencyLevel.Critical)
                .OrderBy(t => t.Deadline ?? DateTime.MaxValue)
                .Take(3) // 最多显示3个紧急事项
                .ToList();

            return new ObservableCollection<TodoItem>(items);
        }
    }

    /// <summary>
    /// 重要待办事项
    /// </summary>
    public ObservableCollection<TodoItem> HighTodos
    {
        get
        {
            var items = _todoService.Todos
                .Where(t => !t.IsCompleted && t.Urgency == UrgencyLevel.High)
                .OrderBy(t => t.Deadline ?? DateTime.MaxValue)
                .Take(2) // 最多显示2个重要事项
                .ToList();

            return new ObservableCollection<TodoItem>(items);
        }
    }

    /// <summary>
    /// 今日待办事项
    /// </summary>
    public ObservableCollection<TodoItem> TodayTodos
    {
        get
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var items = _todoService.Todos
                .Where(t => !t.IsCompleted && 
                           (t.Urgency == UrgencyLevel.Medium || t.Urgency == UrgencyLevel.Low) &&
                           (t.Deadline == null || (t.Deadline >= today && t.Deadline < tomorrow)))
                .OrderBy(t => t.Deadline ?? DateTime.MaxValue)
                .Take(5) // 最多显示5个今日事项
                .ToList();

            return new ObservableCollection<TodoItem>(items);
        }
    }

    /// <summary>
    /// 是否没有任何待办事项
    /// </summary>
    public bool HasNoTodos => _todoService.PendingCount == 0;

    #endregion

    #region 命令

    public ICommand? QuickAddCommand { get; private set; }
    public ICommand? CompleteCommand { get; private set; }
    public ICommand? PostponeCommand { get; private set; }
    public ICommand? CancelCommand { get; private set; }
    public ICommand? ClearCompletedCommand { get; private set; }

    private void InitializeCommands()
    {
        QuickAddCommand = new RelayCommand(ExecuteQuickAdd, CanExecuteQuickAdd);
        CompleteCommand = new RelayCommand<TodoItem>(ExecuteComplete, CanExecuteComplete);
        PostponeCommand = new RelayCommand<TodoItem>(ExecutePostpone, CanExecutePostpone);
        CancelCommand = new RelayCommand(ExecuteCancel);
        ClearCompletedCommand = new RelayCommand(ExecuteClearCompleted, CanExecuteClearCompleted);
    }

    #endregion

    #region 命令实现

    private bool CanExecuteQuickAdd()
    {
        return !string.IsNullOrWhiteSpace(QuickAddText);
    }

    private void ExecuteQuickAdd()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(QuickAddText))
                return;

            var title = QuickAddText.Trim();
            _todoService.QuickAdd(title);
            
            // 清空输入
            QuickAddText = string.Empty;
            
            // 刷新视图
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"快速添加待办事项: {title}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"快速添加失败: {ex.Message}");
        }
    }

    private bool CanExecuteComplete(TodoItem? todoItem)
    {
        return todoItem != null && !todoItem.IsCompleted;
    }

    private void ExecuteComplete(TodoItem? todoItem)
    {
        try
        {
            if (todoItem == null) return;

            _todoService.CompleteTodo(todoItem.Id);
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"完成待办事项: {todoItem.Title}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"完成待办事项失败: {ex.Message}");
        }
    }

    private bool CanExecutePostpone(TodoItem? todoItem)
    {
        return todoItem != null && !todoItem.IsCompleted && todoItem.Deadline.HasValue;
    }

    private void ExecutePostpone(TodoItem? todoItem)
    {
        try
        {
            if (todoItem == null) return;

            // 延期1小时
            _todoService.PostponeTodo(todoItem.Id, TimeSpan.FromHours(1));
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"延期待办事项: {todoItem.Title}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"延期待办事项失败: {ex.Message}");
        }
    }

    private async void ExecuteCancel()
    {
        try
        {
            // 清空快速添加文本
            QuickAddText = string.Empty;
            
            // 返回背景模式
            if (_stateController != null)
            {
                await _stateController.SwitchToBackgroundModeAsync();
            }
            
            System.Diagnostics.Debug.WriteLine("取消操作，返回背景模式");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取消操作失败: {ex.Message}");
        }
    }

    private bool CanExecuteClearCompleted()
    {
        return _todoService.Todos.Any(t => t.IsCompleted);
    }

    private void ExecuteClearCompleted()
    {
        try
        {
            var clearedCount = _todoService.ClearCompleted();
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"清除了 {clearedCount} 个已完成的待办事项");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"清除已完成事项失败: {ex.Message}");
        }
    }

    #endregion

    #region 私有方法

    private async Task LoadDataAsync()
    {
        try
        {
            await _todoService.LoadDataAsync();
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine("数据加载完成");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"数据加载失败: {ex.Message}");
        }
    }

    private void RefreshTodoLists()
    {
        OnPropertyChanged(nameof(CriticalTodos));
        OnPropertyChanged(nameof(HighTodos));
        OnPropertyChanged(nameof(TodayTodos));
        OnPropertyChanged(nameof(HasNoTodos));
        OnPropertyChanged(nameof(HeaderTitle));
        OnPropertyChanged(nameof(HeaderSubtitle));
    }

    private void TodoService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // 当TodoService的属性发生变化时，刷新相关的视图模型属性
        switch (e.PropertyName)
        {
            case nameof(ITodoService.Todos):
            case nameof(ITodoService.PendingCount):
            case nameof(ITodoService.HighestUrgency):
                RefreshTodoLists();
                break;
        }
    }

    private void StateController_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // 当窗口状态发生变化时，更新相关属性
        switch (e.PropertyName)
        {
            case nameof(WindowStateController.CurrentMode):
                OnPropertyChanged(nameof(WindowLeft));
                OnPropertyChanged(nameof(WindowWidth));
                break;
        }
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 手动刷新数据
    /// </summary>
    public void RefreshData()
    {
        _todoService.RefreshUrgencyLevels();
        RefreshTodoLists();
    }

    /// <summary>
    /// 添加新的待办事项（带截止时间）
    /// </summary>
    public void AddTodoWithDeadline(string title, DateTime deadline, string description = "")
    {
        try
        {
            _todoService.AddTodo(title, description, deadline);
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"添加带截止时间的待办事项: {title} (截止: {deadline})");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"添加待办事项失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 删除待办事项
    /// </summary>
    public void DeleteTodo(TodoItem todoItem)
    {
        try
        {
            if (todoItem == null) return;

            _todoService.RemoveTodo(todoItem);
            RefreshTodoLists();
            
            System.Diagnostics.Debug.WriteLine($"删除待办事项: {todoItem.Title}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"删除待办事项失败: {ex.Message}");
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 更新状态控制器引用（用于解决循环依赖问题）
    /// </summary>
    public void UpdateStateController(WindowStateController stateController)
    {
        // 取消旧订阅
        if (_stateController != null)
        {
            _stateController.PropertyChanged -= StateController_PropertyChanged;
        }
        
        // 设置新引用并订阅
        _stateController = stateController ?? throw new ArgumentNullException(nameof(stateController));
        _stateController.PropertyChanged += StateController_PropertyChanged;
        
        // 刷新状态相关属性
        OnPropertyChanged(nameof(WindowLeft));
        OnPropertyChanged(nameof(WindowWidth));
        
        System.Diagnostics.Debug.WriteLine("WindowStateController引用已更新");
    }

    #endregion
}

/// <summary>
/// 简单的RelayCommand实现
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke();
    }
}

/// <summary>
/// 带参数的RelayCommand实现
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke((T?)parameter);
    }
}