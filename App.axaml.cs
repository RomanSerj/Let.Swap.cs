using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Let.Swap_cs;

public partial class App : Application
{
    private HotkeyManager? _hotkeys;
    private ClipboardActions? _actions;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            desktop.MainWindow = window;

            _hotkeys = new HotkeyManager();
            _actions = new ClipboardActions(_hotkeys);

            var settings = AppSettings.Load();
            window.ApplySettingsToUi(settings);

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

            _hotkeys.Start();
            window.AttachServices(_hotkeys, _actions);
            window.Hide();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void OnTrayOpenClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Show();
            desktop.MainWindow?.WindowState = Avalonia.Controls.WindowState.Normal;
            desktop.MainWindow?.Activate();
        }
    }

    public void OnTrayExitClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is MainWindow mw)
        {
            mw.ReadSettingsFromUi().Save();
        }

        Environment.Exit(0);
    }
}
