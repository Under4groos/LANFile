using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace LANFile.Helper;

public class ControlHelper
{
    public static T? FindTemplateControlByName<T>(ContentControl parent, string name) where T : class
    {
        foreach (var child in parent.GetTemplateChildren())
            if (child.GetType() == typeof(T) && child.Name == name)
                return child as T;
        return null;
    }
}