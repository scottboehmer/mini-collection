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
        return Path.Combine(settings.Path, "forces");
    }

    public static string GetRenderDirectory()
    {
        var settings = SettingsManager.GetSettings();
        return Path.Combine(settings.Path, "md");
    }

    public static string GetForceFileName(string forceName)
    {
        var filename = forceName.ToLower().Replace(' ','-').Replace("'", "");
        return $"{GetForcesDirectory()}/{filename}.json";
    }
}