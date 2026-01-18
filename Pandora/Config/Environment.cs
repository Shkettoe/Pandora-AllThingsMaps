using System;
using System.IO;

namespace Pandora.Config;

public static class AppEnvironment
{
    public enum FolderEnum
    {
        DefaultAppData,
        LinuxAppData,
        WindowsAppData
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
}