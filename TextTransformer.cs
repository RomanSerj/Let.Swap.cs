using System.Collections.Generic;
using System.Text;

namespace Let.Swap_cs;

public static class TextTransformer
{
    private static readonly Dictionary<char, char> Map = new()
    {
        // Латиница → кириллица (нижний регистр)
        { '-', '-' }, { 'q', 'й' }, { 'w', 'ц' }, { 'e', 'у' }, { 'r', 'к' },
        { 't', 'е' }, { 'y', 'н' }, { 'u', 'г' }, { 'i', 'ш' }, { 'o', 'щ' },
        { 'p', 'з' }, { '[', 'х' }, { ']', 'ъ' }, { 'a', 'ф' }, { 's', 'ы' },
        { 'd', 'в' }, { 'f', 'а' }, { 'g', 'п' }, { 'h', 'р' }, { 'j', 'о' },
        { 'k', 'л' }, { 'l', 'д' }, { ';', 'ж' }, { '\'', 'э' }, { 'z', 'я' },
        { 'x', 'ч' }, { 'c', 'с' }, { 'v', 'м' }, { 'b', 'и' }, { 'n', 'т' },
        { 'm', 'ь' }, { ',', 'б' }, { '/', '.' }, { '!', '!' }, { '@', '"' },
        { '#', '№' }, { '$', ';' }, { '%', '%' }, { '^', ':' }, { '*', '*' },
        { '(', '(' }, { ')', ')' }, { '_', '_' }, { '+', '+' }, { '|', '|' },
        { '~', 'Ё' }, { '`', 'ё' },

        // Латиница → кириллица (верхний регистр)
        { 'Q', 'Й' }, { 'W', 'Ц' }, { 'E', 'У' }, { 'R', 'К' }, { 'T', 'Е' },
        { 'Y', 'Н' }, { 'U', 'Г' }, { 'I', 'Ш' }, { 'O', 'Щ' }, { 'P', 'З' },
        { '{', 'Х' }, { '}', 'Ъ' }, { 'A', 'Ф' }, { 'S', 'Ы' }, { 'D', 'В' },
        { 'F', 'А' }, { 'G', 'П' }, { 'H', 'Р' }, { 'J', 'О' }, { 'K', 'Л' },
        { 'L', 'Д' }, { ':', 'Ж' }, { '"', 'Э' }, { 'Z', 'Я' }, { 'X', 'Ч' },
        { 'C', 'С' }, { 'V', 'М' }, { 'B', 'И' }, { 'N', 'Т' }, { 'M', 'Ь' },
        { '<', 'Б' }, { '>', 'Ю' }, { '?', ',' },

        // Кириллица → латиница (нижний регистр)
        { 'й', 'q' }, { 'ц', 'w' }, { 'у', 'e' }, { 'к', 'r' }, { 'е', 't' },
        { 'н', 'y' }, { 'г', 'u' }, { 'ш', 'i' }, { 'щ', 'o' }, { 'з', 'p' },
        { 'х', '[' }, { 'ъ', ']' }, { 'ф', 'a' }, { 'ы', 's' }, { 'в', 'd' },
        { 'а', 'f' }, { 'п', 'g' }, { 'р', 'h' }, { 'о', 'j' }, { 'л', 'k' },
        { 'д', 'l' }, { 'ж', ';' }, { 'э', '\'' }, { 'я', 'z' }, { 'ч', 'x' },
        { 'с', 'c' }, { 'м', 'v' }, { 'и', 'b' }, { 'т', 'n' }, { 'ь', 'm' },
        { 'б', ',' }, { 'ю', '.' },

        // Кириллица → латиница (верхний регистр)
        { 'Й', 'Q' }, { 'Ц', 'W' }, { 'У', 'E' }, { 'К', 'R' }, { 'Е', 'T' },
        { 'Н', 'Y' }, { 'Г', 'U' }, { 'Ш', 'I' }, { 'Щ', 'O' }, { 'З', 'P' },
        { 'Х', '{' }, { 'Ъ', '}' }, { 'Ф', 'A' }, { 'Ы', 'S' }, { 'В', 'D' },
        { 'А', 'F' }, { 'П', 'G' }, { 'Р', 'H' }, { 'О', 'J' }, { 'Л', 'K' },
        { 'Д', 'L' }, { 'Ж', ':' }, { 'Э', '"' }, { 'Я', 'Z' }, { 'Ч', 'X' },
        { 'С', 'C' }, { 'М', 'V' }, { 'И', 'B' }, { 'Т', 'N' }, { 'Ь', 'M' },
        { 'Б', '<' }, { 'Ю', '>' },

        // Цифры и общие символы
        { ' ', ' ' }, { '1', '1' }, { '2', '2' }, { '3', '3' }, { '4', '4' },
        { '5', '5' }, { '6', '6' }, { '7', '7' }, { '8', '8' }, { '9', '9' },
        { '0', '0' }, { '&', '?' }
    };

    private static readonly HashSet<char> Alphabet = new()
    {
        'а','б','в','г','д','е','ё','ж','з','и','й','к','л','м','н','о',
        'п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я'
    };

    public static string Transliterate(string input)
    {
        var chars = input.ToCharArray();
        var sb = new StringBuilder(chars.Length);

        for (int i = 0; i < chars.Length; i++)
        {
            char c = chars[i];

            if (c == '.')
            {
                char prev = i > 0 ? chars[i - 1] : '\0';
                char next = i + 1 < chars.Length ? chars[i + 1] : '\0';

                if (Alphabet.Contains(prev))
                    sb.Append('ю');
                else if (Alphabet.Contains(next))
                    sb.Append('/');
                else
                    sb.Append('ю');
            }
            else if (c == ',')
            {
                char prev = i > 0 ? chars[i - 1] : '\0';
                char next = i + 1 < chars.Length ? chars[i + 1] : '\0';

                if (Alphabet.Contains(prev))
                    sb.Append('б');
                else if (Alphabet.Contains(next))
                    sb.Append('?');
                else
                    sb.Append('б');
            }
            else if (Map.TryGetValue(c, out char mapped))
            {
                sb.Append(mapped);
            }
        }

        return sb.ToString();
    }

    public static string ToUpper(string input) => input.ToUpper();

    public static string ToLower(string input) => input.ToLower();

    public static string SwapCase(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (char.IsUpper(c))
                sb.Append(char.ToLower(c));
            else if (char.IsLower(c))
                sb.Append(char.ToUpper(c));
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
}
