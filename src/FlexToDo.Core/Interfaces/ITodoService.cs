using FlexToDo.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FlexToDo.Core.Interfaces;

/// <summary>
/// 待办事项业务服务接口
/// </summary>
public interface ITodoService : INotifyPropertyChanged
{
    /// <summary>
    /// 待办事项集合
    /// </summary>
    ObservableCollection<TodoItem> Todos { get; }

    /// <summary>
    /// 获取按紧急程度分组的待办事项
    /// </summary>
    IEnumerable<IGrouping<UrgencyLevel, TodoItem>> TodosByUrgency { get; }

    /// <summary>
    /// 获取最高紧急程度
    /// </summary>
    UrgencyLevel HighestUrgency { get; }

    /// <summary>
    /// 获取未完成的待办事项数量
    /// </summary>
    int PendingCount { get; }

    /// <summary>
    /// 异步加载数据
    /// </summary>
    Task LoadDataAsync();

    /// <summary>
    /// 异步保存数据
    /// </summary>
    Task SaveDataAsync();

    /// <summary>
    /// 添加待办事项
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="deadline">截止时间</param>
    /// <returns>新创建的待办事项</returns>
    TodoItem AddTodo(string title, string? description = null, DateTime? deadline = null);

    /// <summary>
    /// 快速添加待办事项
    /// </summary>
    /// <param name="title">标题</param>
    /// <returns>新创建的待办事项</returns>
    TodoItem QuickAdd(string title);

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="id">待办事项ID</param>
    /// <returns>是否删除成功</returns>
    bool RemoveTodo(Guid id);

    /// <summary>
    /// 删除待办事项
    /// </summary>
    /// <param name="todoItem">待办事项</param>
    /// <returns>是否删除成功</returns>
    bool RemoveTodo(TodoItem todoItem);

    /// <summary>
    /// 根据ID获取待办事项
    /// </summary>
    /// <param name="id">待办事项ID</param>
    /// <returns>待办事项，如果不存在则返回null</returns>
    TodoItem? GetTodo(Guid id);

    /// <summary>
    /// 标记待办事项为已完成
    /// </summary>
    /// <param name="id">待办事项ID</param>
    /// <returns>是否操作成功</returns>
    bool CompleteTodo(Guid id);

    /// <summary>
    /// 标记待办事项为未完成
    /// </summary>
    /// <param name="id">待办事项ID</param>
    /// <returns>是否操作成功</returns>
    bool UncompleteTodo(Guid id);

    /// <summary>
    /// 延期待办事项
    /// </summary>
    /// <param name="id">待办事项ID</param>
    /// <param name="duration">延期时长</param>
    /// <returns>是否操作成功</returns>
    bool PostponeTodo(Guid id, TimeSpan duration);

    /// <summary>
    /// 清除已完成的待办事项
    /// </summary>
    /// <returns>清除的数量</returns>
    int ClearCompleted();

    /// <summary>
    /// 获取需要提醒的待办事项（即将到期或已过期）
    /// </summary>
    /// <returns>需要提醒的待办事项列表</returns>
    IEnumerable<TodoItem> GetUrgentTodos();

    /// <summary>
    /// 更新所有待办事项的紧急程度（定时调用）
    /// </summary>
    void RefreshUrgencyLevels();
}