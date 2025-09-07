namespace FlexToDo.Core.Models;

/// <summary>
/// 紧急程度枚举
/// </summary>
public enum UrgencyLevel
{
    /// <summary>
    /// 低优先级 - 绿色 (#22c55e)
    /// </summary>
    Low = 0,

    /// <summary>
    /// 普通优先级 - 黄色 (#eab308)
    /// </summary>
    Medium = 1,

    /// <summary>
    /// 高优先级 - 橙色 (#f97316)
    /// </summary>
    High = 2,

    /// <summary>
    /// 紧急/重要 - 红色 (#ef4444)
    /// </summary>
    Critical = 3
}