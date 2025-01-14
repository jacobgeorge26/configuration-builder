using System.IO.Abstractions;

namespace Common.Extensions;

public static class FileSystemExtensions
{
    public static string? ReadFile(this IFileSystem fileSystem, string filePath)
    {
        var fileInfo = fileSystem.FileInfo.New(filePath);
        return !fileInfo.Exists ? null : fileSystem.File.ReadAllText(filePath);
    }
}