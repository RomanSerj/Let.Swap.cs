using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpHook;
using SharpHook.Data;
using SharpHook.Simulation;
using TextCopy;

namespace Let.Swap_cs;

/// <summary>
/// Обёртка над SharpHook: регистрация глобальных хоткеев и симуляция Ctrl+C / Ctrl+V.
/// </summary>
public sealed class HotkeyManager : IDisposable
{
    private readonly SimpleGlobalHook _hook;
    private readonly Dictionary<string, Action> _hotkeys = new();
    private readonly object _lock = new();

    private readonly EventSimulator _simulator;

    public HotkeyManager()
    {
        _hook = new SimpleGlobalHook();
        _hook.KeyPressed += OnKeyPressed;

        _simulator = EventSimulator.Create("Let.Swap");
    }

    public void Start()
    {
        _hook.RunAsync();
    }

    public void Register(string combination, Action callback)
    {
        lock (_lock)
        {
            _hotkeys[combination] = callback;
        }
    }

    public void UnregisterAll()
    {
        lock (_lock)
        {
            _hotkeys.Clear();
        }
    }

    public void CopyOnly()
    {
        SimulateCtrlKey(KeyCode.VcC);
    }

    public void PasteOnly()
    {
        SimulateCtrlKey(KeyCode.VcV);
    }

    public void CopyPaste()
    {
        CopyOnly();
        System.Threading.Thread.Sleep(50);
        PasteOnly();
    }

    private void SimulateCtrlKey(KeyCode key)
    {
        _simulator.SimulateKeyPress(KeyCode.VcLeftControl);
        _simulator.SimulateKeyPress(key);
        _simulator.SimulateKeyRelease(key);
        _simulator.SimulateKeyRelease(KeyCode.VcLeftControl);
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        var combo = BuildCombinationString(e);

        Action? callback = null;
        lock (_lock)
        {
            if (_hotkeys.TryGetValue(combo, out var cb))
                callback = cb;
        }

        if (callback != null)
        {
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(200); // большая пауза как в рабочей версии
                callback();
            });
        }
    }

    private static string BuildCombinationString(KeyboardHookEventArgs e)
    {
        var parts = new List<string>();
        var mask = e.RawEvent.Mask;

        if ((mask & EventMask.Ctrl) != 0) parts.Add("Ctrl");
        if ((mask & EventMask.Alt) != 0) parts.Add("Alt");
        if ((mask & EventMask.Shift) != 0) parts.Add("Shift");

        parts.Add(KeyCodeToString(e.Data.KeyCode));
        return string.Join("+", parts);
    }

    private static string KeyCodeToString(KeyCode code)
    {
        var name = code.ToString();
        if (name.StartsWith("Vc"))
            name = name.Substring(2);
        return name;
    }

    public void Dispose()
    {
        _hook.KeyPressed -= OnKeyPressed;
        _hook.Dispose();
        _simulator.Dispose();
    }
}
