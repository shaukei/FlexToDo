using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace FlexToDo.Core.Models;

/// <summary>
/// 待办事项数据模型
/// </summary>
public class TodoItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime? _deadline;
    private UrgencyLevel _urgency = UrgencyLevel.Low;
    private bool _isCompleted = false;
    private DateTime? _completedAt;

    /// <summary>
    /// 唯一标识符
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 标题
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value))
            {
                ValidateTitle();
            }
        }
    }

    /// <summary>
    /// 详细描述
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>
    /// 截止时间
    /// </summary>
    public DateTime? Deadline
    {
        get => _deadline;
        set
        {
            if (SetProperty(ref _deadline, value))
            {
                UpdateUrgencyLevel();
                OnPropertyChanged(nameof(TimeUntilDeadline));
                OnPropertyChanged(nameof(IsOverdue));
                OnPropertyChanged(nameof(DeadlineText));
            }
        }
    }

    /// <summary>
    /// 紧急程度（自动计算）
    /// </summary>
    public UrgencyLevel Urgency
    {
        get => _urgency;
        private set => SetProperty(ref _urgency, value);
    }

    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                if (value && !CompletedAt.HasValue)
                {
                    CompletedAt = DateTime.Now;
                }
                else if (!value)
                {
                    CompletedAt = null;
                }
                UpdateUrgencyLevel();
            }
        }
    }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedAt
    {
        get => _completedAt;
        set => SetProperty(ref _completedAt, value);
    }

    #region 计算属性

    /// <summary>
    /// 距离截止时间还有多长时间
    /// </summary>
    [JsonIgnore]
    public TimeSpan? TimeUntilDeadline => Deadline?.Subtract(DateTime.Now);

    /// <summary>
    /// 是否已过期
    /// </summary>
    [JsonIgnore]
    public bool IsOverdue => Deadline.HasValue && DateTime.Now > Deadline.Value && !IsCompleted;

    /// <summary>
    /// 截止时间显示文本
    /// </summary>
    [JsonIgnore]
    public string DeadlineText
    {
        get
        {
            if (!Deadline.HasValue) return string.Empty;

            var timeSpan = TimeUntilDeadline;
            if (timeSpan == null) return string.Empty;

            if (IsOverdue)
            {
                var overdue = timeSpan.Value.Negate();
                if (overdue.TotalDays >= 1)
                    return $"已过期 {Math.Floor(overdue.TotalDays)} 天";
                else if (overdue.TotalHours >= 1)
                    return $"已过期 {Math.Floor(overdue.TotalHours)} 小时";
                else
                    return $"已过期 {Math.Floor(overdue.TotalMinutes)} 分钟";
            }

            if (timeSpan.Value.TotalDays >= 1)
                return $"{Math.Floor(timeSpan.Value.TotalDays)} 天后到期";
            else if (timeSpan.Value.TotalHours >= 1)
                return $"{Math.Floor(timeSpan.Value.TotalHours)} 小时后到期";
            else if (timeSpan.Value.TotalMinutes >= 1)
                return $"{Math.Floor(timeSpan.Value.TotalMinutes)} 分钟后到期";
            else
                return "即将到期";
        }
    }

    /// <summary>
    /// 紧急程度颜色
    /// </summary>
    [JsonIgnore]
    public string UrgencyColor => Urgency switch
    {
        UrgencyLevel.Critical => "#ef4444",
        UrgencyLevel.High => "#f97316",
        UrgencyLevel.Medium => "#eab308",
        UrgencyLevel.Low => "#22c55e",
        _ => "#6b7280"
    };

    /// <summary>
    /// 紧急程度图标
    /// </summary>
    [JsonIgnore]
    public string UrgencyIcon => Urgency switch
    {
        UrgencyLevel.Critical => "🔥",
        UrgencyLevel.High => "⚡",
        UrgencyLevel.Medium => "📋",
        UrgencyLevel.Low => "✓",
        _ => "📝"
    };

    #endregion

    #region 私有方法

    /// <summary>
    /// 基于截止时间和完成状态自动更新紧急程度
    /// </summary>
    private void UpdateUrgencyLevel()
    {
        if (IsCompleted)
        {
            Urgency = UrgencyLevel.Low;
            return;
        }

        if (!Deadline.HasValue)
        {
            Urgency = UrgencyLevel.Medium;
            return;
        }

        var timeLeft = TimeUntilDeadline;
        if (timeLeft == null)
        {
            Urgency = UrgencyLevel.Medium;
            return;
        }

        // 已过期
        if (timeLeft.Value.TotalMinutes < 0)
        {
            Urgency = UrgencyLevel.Critical;
        }
        // 6小时内
        else if (timeLeft.Value.TotalHours < 6)
        {
            Urgency = UrgencyLevel.Critical;
        }
        // 24小时内
        else if (timeLeft.Value.TotalHours < 24)
        {
            Urgency = UrgencyLevel.High;
        }
        // 3天内
        else if (timeLeft.Value.TotalDays < 3)
        {
            Urgency = UrgencyLevel.Medium;
        }
        // 3天以上
        else
        {
            Urgency = UrgencyLevel.Low;
        }
    }

    /// <summary>
    /// 验证标题有效性
    /// </summary>
    private void ValidateTitle()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new ArgumentException("待办事项标题不能为空", nameof(Title));
        }

        if (Title.Length > 100)
        {
            throw new ArgumentException("待办事项标题不能超过100个字符", nameof(Title));
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 标记为已完成
    /// </summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

    /// <summary>
    /// 标记为未完成
    /// </summary>
    public void MarkAsIncomplete()
    {
        IsCompleted = false;
    }

    /// <summary>
    /// 延期指定时间
    /// </summary>
    /// <param name="duration">延期时长</param>
    public void Postpone(TimeSpan duration)
    {
        if (Deadline.HasValue)
        {
            Deadline = Deadline.Value.Add(duration);
        }
        else
        {
            Deadline = DateTime.Now.Add(duration);
        }
    }

    /// <summary>
    /// 克隆待办事项
    /// </summary>
    /// <returns>新的待办事项实例</returns>
    public TodoItem Clone()
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(), // 新的ID
            Title = Title,
            Description = Description,
            Deadline = Deadline,
            CreatedAt = DateTime.Now
        };
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

    #region 重写方法

    public override string ToString()
    {
        return $"{Title} ({Urgency})";
    }

    public override bool Equals(object? obj)
    {
        return obj is TodoItem item && Id.Equals(item.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    #endregion
}