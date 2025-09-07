using System;
using System.Globalization;
using System.Windows.Data;
using FlexToDo.Core.Models;

namespace FlexToDo.UI.Converters;

/// <summary>
/// 待办项悬停状态转换器
/// 用于判断当前待办项是否为悬停的待办项
/// </summary>
public class TodoItemHoverConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
            return false;

        // values[0] 是当前待办项
        // values[1] 是悬停的待办项
        var currentItem = values[0] as TodoItem;
        var hoveredItem = values[1] as TodoItem;

        if (currentItem == null || hoveredItem == null)
            return false;

        // 比较待办项的ID
        return currentItem.Id == hoveredItem.Id;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}