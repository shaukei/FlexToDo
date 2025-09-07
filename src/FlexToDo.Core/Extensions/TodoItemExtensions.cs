using FlexToDo.Core.Models;
using System.ComponentModel;

namespace FlexToDo.Core.Extensions;

/// <summary>
/// TodoItem扩展方法
/// </summary>
public static class TodoItemExtensions
{
    /// <summary>
    /// 手动触发PropertyChanged事件（用于从外部更新紧急程度等）
    /// </summary>
    public static void OnPropertyChanged(this TodoItem todoItem, string propertyName)
    {
        // 使用反射触发私有的OnPropertyChanged方法
        var method = typeof(TodoItem).GetMethod("OnPropertyChanged", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        method?.Invoke(todoItem, new object[] { propertyName });
    }
}