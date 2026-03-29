namespace Content.Shared.Utils;

public static class FileLengthFormater
{
    public static string FormatBytes(long bytes)
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;
        const long TB = GB * 1024;

        if (bytes >= TB)
            return $"{bytes / (double)TB:0.##} TB";
        if (bytes >= GB)
            return $"{bytes / (double)GB:0.##} GB";
        if (bytes >= MB)
            return $"{bytes / (double)MB:0.##} MB";
        if (bytes >= KB)
            return $"{bytes / (double)KB:0.##} KB";

        return $"{bytes} B";
    }
}