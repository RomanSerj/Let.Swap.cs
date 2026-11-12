using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Let.Swap_cs;

[JsonSerializable(typeof(AppSettings))]
internal partial class SettingsJsonContext : JsonSerializerContext
{
}

public sealed class AppSettings
{
    public string TransliterationKey { get; set; } = "Alt+Z";
    public string UpperKey { get; set; } = "Alt+X";
    public string LowerKey { get; set; } = "Alt+Q";
    public string SwapKey { get; set; } = "Alt+B";
    public bool ExtraEnabled { get; set; } = true;

    private static readonly string ConfigDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Let.Swap");

    private static readonly string ConfigPath =
        Path.Combine(ConfigDir, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
                return new AppSettings();

            var json = File.ReadAllText(ConfigPath);
            var loaded = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.AppSettings);
            return loaded ?? new AppSettings();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOAD] ОШИБКА: {ex.Message}");
            return new AppSettings();
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(ConfigDir);
            var json = JsonSerializer.Serialize(this, SettingsJsonContext.Default.AppSettings);
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SAVE] ОШИБКА: {ex.Message}");
        }
    }
}
