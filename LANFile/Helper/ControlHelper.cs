using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace LANFile.Helper;

public class ControlHelper
{
    public static T? FindTemplateControlByName<T>(ContentControl parent, string name) where T : class
    {
        return parent.GetTemplateChildren()
            .OfType<T>()
            .FirstOrDefault(child => (child as Control).Name == name);
    }
    public static IEnumerable<Control> FindTemplateControlsByType<T>(ContentControl parent,  Type type ) where T : class
    {
        return parent.GetTemplateChildren().Where(control => control.GetType() == type );
    }
}