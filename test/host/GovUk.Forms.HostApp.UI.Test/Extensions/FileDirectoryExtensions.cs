namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class FileDirectoryExtensions
{
    public static string FilePathCombine(this string directory, string fileName)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(fileName);

        string safeFileName = SanitizeFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            throw new ArgumentException(
                "Sanitization removed all characters from the file name.",
                nameof(fileName));
        }

        return Path.Combine(directory, safeFileName);
    }

    public static string SanitizeFileName(this string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        return string.Concat(fileName
            .Split(Path.GetInvalidFileNameChars()))
            .Trim('.');
    }

    public static string? FindProjectRoot(this string startPath)
    {
        ArgumentNullException.ThrowIfNull(startPath);

        DirectoryInfo? dir = File.Exists(startPath)
            ? new FileInfo(startPath).Directory
            : new DirectoryInfo(startPath);

        while (dir != null)
        {
            if (dir.EnumerateFiles("*.csproj").Any() ||
                dir.EnumerateFiles("*.sln").Any())
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return null;
    }

    public static IEnumerable<string> FindAllFilePaths(
        this string root,
        string fileExtension,
        ISet<string> ignoreDirs)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(fileExtension);
        ArgumentNullException.ThrowIfNull(ignoreDirs);

        Stack<string> dirs = new();
        dirs.Push(root);

        while (dirs.Count > 0)
        {
            string current = dirs.Pop();

            IEnumerable<string> subDirs;
            try
            {
                subDirs = Directory.EnumerateDirectories(current);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (IOException)
            {
                continue;
            }

            foreach (string dir in subDirs)
            {
                string name = Path.GetFileName(dir);
                if (!ignoreDirs.Contains(name))
                {
                    dirs.Push(dir);
                }
            }

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(
                    current,
                    $"*.{fileExtension.TrimStart('.')}",
                    SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException)
            {
                continue;
            }
            catch (IOException)
            {
                continue;
            }

            foreach (string file in files)
            {
                yield return file;
            }
        }
    }

    public static string DirectoryPathCombine(params string[] paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        return Path.Combine(paths);
    }
}
