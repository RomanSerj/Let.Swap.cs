using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Let.Swap_cs;

public partial class MainWindow : Window
{
    private HotkeyManager? _hotkeys;
    private ClipboardActions? _actions;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void AttachServices(HotkeyManager hotkeys, ClipboardActions actions)
    {
        _hotkeys = hotkeys;
        _actions = actions;
    }

    public void ApplySettingsToUi(AppSettings settings)
    {
        Console.WriteLine("=== ApplySettingsToUi ===");
        Console.WriteLine($"Хочу выставить: '{settings.TransliterationKey}'");
        Console.WriteLine("Доступно в ComboTran:");
        foreach (var obj in ComboTran.Items)
        {
            var text = obj is ComboBoxItem cbi ? cbi.Content?.ToString() : obj?.ToString();
            Console.WriteLine($"  -> '{text}' ({obj?.GetType().Name})");
        }

        SetComboValue(ComboTran, settings.TransliterationKey);
        SetComboValue(ComboUp,   settings.UpperKey);
        SetComboValue(ComboLw,   settings.LowerKey);
        SetComboValue(ComboBw,   settings.SwapKey);

        ExtraCheck.IsChecked = settings.ExtraEnabled;

        Console.WriteLine($"После установки: '{GetSelectedText(ComboTran)}'");
        Console.WriteLine("========================");
    }

    public AppSettings ReadSettingsFromUi()
    {
        var s = new AppSettings
        {
            TransliterationKey = GetSelectedText(ComboTran),
            UpperKey           = GetSelectedText(ComboUp),
            LowerKey           = GetSelectedText(ComboLw),
            SwapKey            = GetSelectedText(ComboBw),
            ExtraEnabled       = ExtraCheck.IsChecked == true
        };

        Console.WriteLine($"[READ UI] '{s.TransliterationKey}' / '{s.UpperKey}' / '{s.LowerKey}' / '{s.SwapKey}' / Extra={s.ExtraEnabled}");
        return s;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
        base.OnClosing(e);
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e)
    {
        if (_hotkeys == null || _actions == null) return;

        var settings = ReadSettingsFromUi();
        settings.Save();

        _hotkeys.UnregisterAll();

        if (!string.IsNullOrEmpty(settings.TransliterationKey))
            _hotkeys.Register(settings.TransliterationKey, _actions.Transliterate);

        if (settings.ExtraEnabled)
        {
            if (!string.IsNullOrEmpty(settings.UpperKey))
                _hotkeys.Register(settings.UpperKey, _actions.ToUpper);

            if (!string.IsNullOrEmpty(settings.LowerKey))
                _hotkeys.Register(settings.LowerKey, _actions.ToLower);

            if (!string.IsNullOrEmpty(settings.SwapKey))
                _hotkeys.Register(settings.SwapKey, _actions.SwapCase);
        }
    }

    private void OnMinimizeClick(object? sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        ReadSettingsFromUi().Save();
        _hotkeys?.Dispose();
        Environment.Exit(0);
    }

    private static string GetSelectedText(ComboBox combo)
    {
        if (combo.SelectedItem is ComboBoxItem item)
            return item.Content?.ToString() ?? string.Empty;
        return string.Empty;
    }

    private static void SetComboValue(ComboBox combo, string value)
    {
        foreach (var obj in combo.Items)
        {
            if (obj is ComboBoxItem item && item.Content?.ToString() == value)
            {
                combo.SelectedItem = item;
                return;
            }
        }
    }
}
