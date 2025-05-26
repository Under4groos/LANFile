using System.Collections.Generic;
using System.Collections.Specialized;

namespace LANFile.Extentions;

public static class ExtNameValueCollection
{
    public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        foreach (var key in nvc.AllKeys)
        {
            // Добавляем только первое значение для каждого ключа
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = nvc[key];
            }
        }

        return dictionary;
    }
}