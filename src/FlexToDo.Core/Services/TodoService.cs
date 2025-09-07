using FlexToDo.Core.Interfaces;
using FlexToDo.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlexToDo.Core.Services;

/// <summary>
/// 待办事项业务服务实现
/// </summary>
public class TodoService : ITodoService
{
    private readonly IStorageService _storageService;
    private readonly ObservableCollection<TodoItem> _todos;

    public TodoService(IStorageService storageService)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _todos = new ObservableCollection<TodoItem>();
        
        // 监听集合变化
        _todos.CollectionChanged += (sender, e) =>
        {
            OnPropertyChanged(nameof(TodosByUrgency));
            OnPropertyChanged(nameof(HighestUrgency));
            OnPropertyChanged(nameof(PendingCount));
        };
    }

    /// <inheritdoc/>
    public ObservableCollection<TodoItem> Todos => _todos;

    /// <inheritdoc/>
    public IEnumerable<IGrouping<UrgencyLevel, TodoItem>> TodosByUrgency =>
        _todos.Where(t => !t.IsCompleted)
              .GroupBy(t => t.Urgency)
              .OrderByDescending(g => g.Key);

    /// <inheritdoc/>
    public UrgencyLevel HighestUrgency =>
        _todos.Where(t => !t.IsCompleted)
              .DefaultIfEmpty()
              .Max(t => t?.Urgency ?? UrgencyLevel.Low);

    /// <inheritdoc/>
    public int PendingCount => _todos.Count(t => !t.IsCompleted);

    /// <inheritdoc/>
    public async Task LoadDataAsync()
    {
        try
        {
            var todos = await _storageService.LoadTodosAsync();
            
            _todos.Clear();
            foreach (var todo in todos)
            {
                // 重新计算紧急程度（因为时间可能已经过去）
                todo.OnPropertyChanged(nameof(TodoItem.Deadline)); // 触发紧急程度更新
                _todos.Add(todo);
            }
            
            OnPropertyChanged(nameof(TodosByUrgency));
            OnPropertyChanged(nameof(HighestUrgency));
            OnPropertyChanged(nameof(PendingCount));
        }
        catch (Exception ex)
        {
            // 记录日志，但不中断应用程序
            System.Diagnostics.Debug.WriteLine($"加载数据失败: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task SaveDataAsync()
    {
        try
        {
            await _storageService.SaveTodosAsync(_todos.ToList());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存数据失败: {ex.Message}");
            throw; // 保存失败应该通知用户
        }
    }

    /// <inheritdoc/>
    public TodoItem AddTodo(string title, string? description = null, DateTime? deadline = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("标题不能为空", nameof(title));

        var todoItem = new TodoItem
        {
            Title = title.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Deadline = deadline
        };

        _todos.Add(todoItem);
        
        // 异步保存
        _ = Task.Run(SaveDataAsync);
        
        return todoItem;
    }

    /// <inheritdoc/>
    public TodoItem QuickAdd(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("标题不能为空", nameof(title));

        var todoItem = new TodoItem
        {
            Title = title.Trim()
        };

        _todos.Add(todoItem);
        
        // 异步保存
        _ = Task.Run(SaveDataAsync);
        
        return todoItem;
    }

    /// <inheritdoc/>
    public bool RemoveTodo(Guid id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null) return false;

        return RemoveTodo(todo);
    }

    /// <inheritdoc/>
    public bool RemoveTodo(TodoItem todoItem)
    {
        if (todoItem == null) return false;

        var removed = _todos.Remove(todoItem);
        if (removed)
        {
            // 异步保存
            _ = Task.Run(SaveDataAsync);
        }
        
        return removed;
    }

    /// <inheritdoc/>
    public TodoItem? GetTodo(Guid id)
    {
        return _todos.FirstOrDefault(t => t.Id == id);
    }

    /// <inheritdoc/>
    public bool CompleteTodo(Guid id)
    {
        var todo = GetTodo(id);
        if (todo == null) return false;

        todo.MarkAsCompleted();
        
        // 异步保存
        _ = Task.Run(SaveDataAsync);
        
        return true;
    }

    /// <inheritdoc/>
    public bool UncompleteTodo(Guid id)
    {
        var todo = GetTodo(id);
        if (todo == null) return false;

        todo.MarkAsIncomplete();
        
        // 异步保存
        _ = Task.Run(SaveDataAsync);
        
        return true;
    }

    /// <inheritdoc/>
    public bool PostponeTodo(Guid id, TimeSpan duration)
    {
        var todo = GetTodo(id);
        if (todo == null) return false;

        todo.Postpone(duration);
        
        // 异步保存
        _ = Task.Run(SaveDataAsync);
        
        return true;
    }

    /// <inheritdoc/>
    public int ClearCompleted()
    {
        var completedTodos = _todos.Where(t => t.IsCompleted).ToList();
        var count = 0;
        
        foreach (var todo in completedTodos)
        {
            if (_todos.Remove(todo))
                count++;
        }
        
        if (count > 0)
        {
            // 异步保存
            _ = Task.Run(SaveDataAsync);
        }
        
        return count;
    }

    /// <inheritdoc/>
    public IEnumerable<TodoItem> GetUrgentTodos()
    {
        return _todos.Where(t => !t.IsCompleted && 
                                (t.Urgency == UrgencyLevel.Critical || 
                                 t.Urgency == UrgencyLevel.High))
                    .OrderByDescending(t => t.Urgency)
                    .ThenBy(t => t.Deadline ?? DateTime.MaxValue);
    }

    /// <inheritdoc/>
    public void RefreshUrgencyLevels()
    {
        foreach (var todo in _todos.Where(t => !t.IsCompleted))
        {
            // 触发紧急程度重新计算
            todo.OnPropertyChanged(nameof(TodoItem.Deadline));
        }
        
        OnPropertyChanged(nameof(TodosByUrgency));
        OnPropertyChanged(nameof(HighestUrgency));
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}