using System;
using System.Threading;
using TextCopy;

namespace Let.Swap_cs;

/// <summary>
/// Действия над текстом в буфере обмена.
/// Аналог функций mainn(), mainn_2(), mainn_3(), mainn_4() из Python.
/// </summary>
public sealed class ClipboardActions
{
    private readonly HotkeyManager _hotkeys;

    public ClipboardActions(HotkeyManager hotkeys)
    {
        _hotkeys = hotkeys;
    }

    public void Transliterate()
    {
        var text = GrabSelectedText();
        Console.WriteLine($"[DEBUG] Скопировано: '{text}'");
        if (string.IsNullOrEmpty(text)) return;

        var result = TextTransformer.Transliterate(text);
        Console.WriteLine($"[DEBUG] После транслитерации: '{result}'");

        PasteText(result);
    }

    public void ToUpper()
    {
        var text = GrabSelectedText();
        if (string.IsNullOrEmpty(text)) return;

        PasteText(TextTransformer.ToUpper(text));
    }

    public void ToLower()
    {
        var text = GrabSelectedText();
        if (string.IsNullOrEmpty(text)) return;

        PasteText(TextTransformer.ToLower(text));
    }

    public void SwapCase()
    {
        var text = GrabSelectedText();
        if (string.IsNullOrEmpty(text)) return;

        PasteText(TextTransformer.SwapCase(text));
    }

    private string GrabSelectedText()
    {
        _hotkeys.CopyOnly();
        Thread.Sleep(300);

        try
        {
            return ClipboardService.GetText() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private void PasteText(string text)
    {
        try
        {
            ClipboardService.SetText(text);
            Console.WriteLine($"[DEBUG] Записано в буфер: '{text}'");
        }
        catch
        {
            return;
        }

        Thread.Sleep(80);
        _hotkeys.PasteOnly();
        Console.WriteLine("[DEBUG] Отправлен Ctrl+V");
    }
}
