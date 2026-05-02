namespace Finances.Api.Configurations;

public static class EnvironmentFileExtensions
{
    public static WebApplicationBuilder AddLocalEnvironmentFile(this WebApplicationBuilder builder)
    {
        var environmentFilePath = ResolveEnvironmentFilePath(builder.Environment.ContentRootPath);

        if (!File.Exists(environmentFilePath))
        {
            return builder;
        }

        foreach (var line in File.ReadLines(environmentFilePath))
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmedLine.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmedLine[..separatorIndex].Trim();
            var value = trimmedLine[(separatorIndex + 1)..].Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(key) || Environment.GetEnvironmentVariable(key) is not null)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(key, value);
        }

        return builder;
    }

    private static string ResolveEnvironmentFilePath(string contentRootPath)
    {
        var directory = new DirectoryInfo(contentRootPath);

        while (directory is not null)
        {
            var environmentFilePath = Path.Combine(directory.FullName, ".env");
            if (File.Exists(environmentFilePath))
            {
                return environmentFilePath;
            }

            directory = directory.Parent;
        }

        return Path.Combine(contentRootPath, ".env");
    }
}
