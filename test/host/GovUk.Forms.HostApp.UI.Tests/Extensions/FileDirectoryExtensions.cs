namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class FileDirectoryExtensions
{
    public static string FilePathCombine(this string directory, string fileName)
    {
        {
            ArgumentNullException.ThrowIfNull(directory);
            ArgumentNullException.ThrowIfNull(fileName);

            string sanitizedDirectory = directory.SanitizeDirectoryName();
            string sanitizedFileName = fileName.SanitizeFileName();

            return string.IsNullOrEmpty(sanitizedFileName)
                ? throw new ArgumentException("Sanitization removed all characters from the file name.", nameof(fileName))
                : Path.Combine(sanitizedDirectory, sanitizedFileName);
        }
    }


    public static string SanitizeDirectoryName(this string directoryName)
    {
        return string.Concat(directoryName.Split(Path.GetInvalidPathChars()));
    }

    public static string SanitizeFileName(this string fileName)
    {
        return string.Concat(fileName.Split(Path.GetInvalidFileNameChars())).Trim('.');
    }

    public static string? FindProjectRoot(this string projectRoot)
    {
        DirectoryInfo? dir = new(projectRoot);

        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Length != 0)
            {
                return dir.FullName;
            }
            dir = dir.Parent;
        }

        return null;
    }

    public static IEnumerable<string> FindAllFilePaths(this string root, string fileExtension, HashSet<string> ignoreDirs)
    {
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
            catch (Exception)
            {
                // Skip directories we can't access
                continue;
            }

            foreach (string dir in subDirs)
            {
                string name = new DirectoryInfo(dir).Name;

                if (!ignoreDirs.Contains(name))
                {
                    dirs.Push(name);
                }
            }
            IEnumerable<string> files;

            try
            {
                files = Directory.EnumerateFiles(current, "*" + fileExtension.TrimStart('.'), SearchOption.TopDirectoryOnly);
            }
            catch (Exception)
            {
                // Skip directories we can't list files for
                continue;
            }


            foreach (string file in files)
            {
                yield return file;
            }

        }

    }

    public static string DirectoryPathCombine(params string[] subDirectories)
    {
        ArgumentNullException.ThrowIfNull(subDirectories);
        return Path.Combine([.. subDirectories.Select(path => path.SanitizeDirectoryName())]);
    }
}
