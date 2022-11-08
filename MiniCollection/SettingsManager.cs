class SettingsManager
{
    private SettingsManager()
    {
        using (var stream = new FileStream("settings.json", FileMode.Open, FileAccess.Read))
        {
            var settings = System.Text.Json.JsonSerializer.Deserialize<Settings>(stream);
            if (settings != null)
            {
                Settings = settings;
            }
            else
            {
                Settings = new Settings();
                Settings.Path = System.IO.Directory.GetCurrentDirectory();
            }
        }
    }

    private Settings Settings { get; }

    private static SettingsManager? _instance;

    public static Settings GetSettings()
    {
        if (_instance == null)
        {
            _instance = new SettingsManager();
        }
        return _instance.Settings;
    }
}