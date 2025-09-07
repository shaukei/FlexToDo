using FlexToDo.Core.Models;

namespace FlexToDo.Core.Interfaces;

/// <summary>
/// 数据存储服务接口
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// 异步加载所有待办事项
    /// </summary>
    /// <returns>待办事项列表</returns>
    Task<List<TodoItem>> LoadTodosAsync();

    /// <summary>
    /// 异步保存待办事项列表
    /// </summary>
    /// <param name="todos">待办事项列表</param>
    Task SaveTodosAsync(List<TodoItem> todos);

    /// <summary>
    /// 创建数据备份
    /// </summary>
    Task CreateBackupAsync();

    /// <summary>
    /// 恢复数据备份
    /// </summary>
    /// <param name="backupPath">备份文件路径</param>
    Task<List<TodoItem>> RestoreFromBackupAsync(string backupPath);

    /// <summary>
    /// 获取所有备份文件
    /// </summary>
    /// <returns>备份文件路径列表</returns>
    Task<List<string>> GetBackupFilesAsync();
}