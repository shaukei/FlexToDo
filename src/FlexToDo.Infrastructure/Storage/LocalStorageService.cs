using FlexToDo.Core.Interfaces;
using FlexToDo.Core.Models;
using System.IO;
using System.Text.Json;

namespace FlexToDo.Infrastructure.Storage;

/// <summary>
/// 本地JSON文件存储服务实现
/// </summary>
public class LocalStorageService : IStorageService
{
    private readonly string _dataPath;
    private readonly string _backupDirectory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public LocalStorageService()
    {
        // 获取应用程序数据目录
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appDirectory = Path.Combine(appDataPath, "FlexToDo");
        
        _dataPath = Path.Combine(appDirectory, "todos.json");
        _backupDirectory = Path.Combine(appDirectory, "Backups");
        
        // 确保目录存在
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(_backupDirectory);
        
        // JSON序列化选项
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        System.Diagnostics.Debug.WriteLine($"数据文件路径: {_dataPath}");
        System.Diagnostics.Debug.WriteLine($"备份目录路径: {_backupDirectory}");
    }

    /// <inheritdoc/>
    public async Task<List<TodoItem>> LoadTodosAsync()
    {
        await _fileLock.WaitAsync();
        
        try
        {
            if (!File.Exists(_dataPath))
            {
                System.Diagnostics.Debug.WriteLine("数据文件不存在，返回空列表");
                return new List<TodoItem>();
            }

            var json = await File.ReadAllTextAsync(_dataPath);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                System.Diagnostics.Debug.WriteLine("数据文件为空，返回空列表");
                return new List<TodoItem>();
            }

            var todos = JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions);
            
            if (todos == null)
            {
                System.Diagnostics.Debug.WriteLine("JSON反序列化失败，返回空列表");
                return new List<TodoItem>();
            }

            System.Diagnostics.Debug.WriteLine($"成功加载 {todos.Count} 个待办事项");
            return todos;
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"JSON解析错误: {ex.Message}");
            
            // 尝试从备份恢复
            var backups = await GetBackupFilesAsync();
            if (backups.Any())
            {
                System.Diagnostics.Debug.WriteLine("尝试从最新备份恢复数据");
                try
                {
                    return await RestoreFromBackupAsync(backups.First());
                }
                catch (Exception backupEx)
                {
                    System.Diagnostics.Debug.WriteLine($"从备份恢复失败: {backupEx.Message}");
                }
            }
            
            // 创建损坏文件的备份
            await CreateCorruptedFileBackupAsync();
            return new List<TodoItem>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载数据失败: {ex.Message}");
            return new List<TodoItem>();
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task SaveTodosAsync(List<TodoItem> todos)
    {
        if (todos == null) throw new ArgumentNullException(nameof(todos));

        await _fileLock.WaitAsync();
        
        try
        {
            // 创建备份（每次保存前）
            if (File.Exists(_dataPath))
            {
                await CreateBackupAsync();
            }

            var json = JsonSerializer.Serialize(todos, _jsonOptions);
            
            // 使用临时文件写入，然后原子性替换
            var tempPath = _dataPath + ".tmp";
            await File.WriteAllTextAsync(tempPath, json);
            
            // 原子性替换文件
            File.Move(tempPath, _dataPath, overwrite: true);
            
            System.Diagnostics.Debug.WriteLine($"成功保存 {todos.Count} 个待办事项");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存数据失败: {ex.Message}");
            throw new InvalidOperationException($"保存数据失败: {ex.Message}", ex);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task CreateBackupAsync()
    {
        await _fileLock.WaitAsync();
        
        try
        {
            if (!File.Exists(_dataPath))
            {
                System.Diagnostics.Debug.WriteLine("主数据文件不存在，无需备份");
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"todos_backup_{timestamp}.json";
            var backupPath = Path.Combine(_backupDirectory, backupFileName);

            File.Copy(_dataPath, backupPath, overwrite: false);
            
            System.Diagnostics.Debug.WriteLine($"创建备份: {backupFileName}");
            
            // 清理旧备份（保留最近20个）
            await CleanupOldBackupsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"创建备份失败: {ex.Message}");
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<List<TodoItem>> RestoreFromBackupAsync(string backupPath)
    {
        if (string.IsNullOrWhiteSpace(backupPath))
            throw new ArgumentException("备份路径不能为空", nameof(backupPath));

        if (!File.Exists(backupPath))
            throw new FileNotFoundException($"备份文件不存在: {backupPath}");

        await _fileLock.WaitAsync();
        
        try
        {
            var json = await File.ReadAllTextAsync(backupPath);
            var todos = JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions);
            
            if (todos == null)
                throw new InvalidOperationException("备份文件格式无效");

            System.Diagnostics.Debug.WriteLine($"从备份恢复 {todos.Count} 个待办事项: {Path.GetFileName(backupPath)}");
            return todos;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"从备份恢复失败: {ex.Message}");
            throw new InvalidOperationException($"从备份恢复失败: {ex.Message}", ex);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetBackupFilesAsync()
    {
        await Task.CompletedTask; // 保持异步接口一致性
        
        try
        {
            if (!Directory.Exists(_backupDirectory))
                return new List<string>();

            var backupFiles = Directory.GetFiles(_backupDirectory, "todos_backup_*.json")
                                       .OrderByDescending(f => File.GetCreationTime(f))
                                       .ToList();

            System.Diagnostics.Debug.WriteLine($"找到 {backupFiles.Count} 个备份文件");
            return backupFiles;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"获取备份文件列表失败: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// 清理旧备份文件，保留最近的数量
    /// </summary>
    /// <param name="keepCount">保留的备份数量</param>
    private async Task CleanupOldBackupsAsync(int keepCount = 20)
    {
        await Task.Run(() =>
        {
            try
            {
                var backupFiles = Directory.GetFiles(_backupDirectory, "todos_backup_*.json")
                                           .OrderByDescending(f => File.GetCreationTime(f))
                                           .Skip(keepCount)
                                           .ToList();

                foreach (var oldBackup in backupFiles)
                {
                    try
                    {
                        File.Delete(oldBackup);
                        System.Diagnostics.Debug.WriteLine($"删除旧备份: {Path.GetFileName(oldBackup)}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"删除备份文件失败 {Path.GetFileName(oldBackup)}: {ex.Message}");
                    }
                }

                if (backupFiles.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"清理了 {backupFiles.Count} 个旧备份文件");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清理备份文件失败: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 为损坏的文件创建备份
    /// </summary>
    private async Task CreateCorruptedFileBackupAsync()
    {
        try
        {
            if (!File.Exists(_dataPath)) return;

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var corruptedBackupPath = Path.Combine(_backupDirectory, $"todos_corrupted_{timestamp}.json");
            
            File.Copy(_dataPath, corruptedBackupPath, overwrite: false);
            System.Diagnostics.Debug.WriteLine($"创建损坏文件备份: {Path.GetFileName(corruptedBackupPath)}");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"创建损坏文件备份失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取数据文件信息
    /// </summary>
    public (bool exists, long size, DateTime lastModified) GetDataFileInfo()
    {
        try
        {
            if (File.Exists(_dataPath))
            {
                var fileInfo = new FileInfo(_dataPath);
                return (true, fileInfo.Length, fileInfo.LastWriteTime);
            }
            
            return (false, 0, DateTime.MinValue);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"获取文件信息失败: {ex.Message}");
            return (false, 0, DateTime.MinValue);
        }
    }

    /// <summary>
    /// 验证数据文件完整性
    /// </summary>
    public async Task<bool> ValidateDataFileAsync()
    {
        try
        {
            var todos = await LoadTodosAsync();
            return true; // 如果能成功加载，则文件有效
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _fileLock?.Dispose();
    }
}