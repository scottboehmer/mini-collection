namespace MiniCollectionTool;

static class FileHelpers
{
    public static string GetCollectionFileName()
    {
        var settings = SettingsManager.GetSettings();
        return Path.Combine(settings.Path, "collection.json");
    }

    public static string GetForcesDirectory()
    {
        var settings = SettingsManager.GetSettings();
        var path = Path.Combine(settings.Path, "forces");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static string GetRenderDirectory()
    {
        var settings = SettingsManager.GetSettings();
        var path = Path.Combine(settings.Path, "md");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static string GetForceFileName(string forceName)
    {
        var filename = forceName.ToLower().Replace(' ','-').Replace("'", "");
        return Path.Combine(GetForcesDirectory(), $"{filename}.json");
    }
}