using System;
using System.IO;

namespace Pandora.Config;

public static class AppEnvironment
{
    public static string ApplicationPath { get; private set; } = null!;

    public enum FolderEnum
    {
        DefaultAppData,
        LinuxAppData,
        WindowsAppData
    }

    public static void SetFolder()
    {
        if (OperatingSystem.IsWindows()) ApplicationPath = GetFolderPath(FolderEnum.WindowsAppData);
        else if (OperatingSystem.IsLinux()) ApplicationPath = GetFolderPath(FolderEnum.LinuxAppData);
        
        Directory.CreateDirectory(ApplicationPath);
    }

    public static string GetFolderPath(FolderEnum folder)
    {
        if (!Enum.IsDefined(folder))
            throw new ArgumentOutOfRangeException(nameof(folder), folder, null);
        return folder switch
        {
            FolderEnum.DefaultAppData => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pandora"),
            FolderEnum.LinuxAppData => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pandora"),
            FolderEnum.WindowsAppData => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Pandora"),
            _ => throw new ArgumentOutOfRangeException(nameof(folder), folder, null)
        };
    }

    public static string[] GetFiles(string path)
    {
        try
        {
            return Directory.GetFiles(path);
        }
        catch (Exception)
        {
            Directory.CreateDirectory(path);
            return [];
        }
    }
}