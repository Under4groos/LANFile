using Avalonia;
using Avalonia.Controls;

namespace LANFile.Views.Controls;

public class IconTabItem : TabItem
{
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<IconTabItem, string>(nameof(Source));

    public static readonly StyledProperty<bool> IconVisibleProperty =
        AvaloniaProperty.Register<IconTabItem, bool>(nameof(IconVisible), true);


    public static readonly StyledProperty<double> WidthSvgProperty =
        AvaloniaProperty.Register<IconTabItem, double>(nameof(WidthSvg), 25);

    public static readonly StyledProperty<double> HeightSvgProperty =
        AvaloniaProperty.Register<IconTabItem, double>(nameof(HeightSvg), 25);

    public static readonly StyledProperty<double> TextMinWidthProperty =
        AvaloniaProperty.Register<IconTabItem, double>(nameof(TextMinWidth), 25);

    public static readonly StyledProperty<double> TextMinHeightProperty =
        AvaloniaProperty.Register<IconTabItem, double>(nameof(TextMinHeight), 25);

    public static readonly StyledProperty<Thickness> TextMarginProperty =
        AvaloniaProperty.Register<IconTabItem, Thickness>(nameof(TextMargin), new Thickness(0));

    public static readonly StyledProperty<Thickness> BorderMarginProperty =
        AvaloniaProperty.Register<IconTabItem, Thickness>(nameof(BorderMargin), new Thickness(5));

    public double WidthSvg
    {
        get => GetValue(WidthSvgProperty);
        set => SetValue(WidthSvgProperty, value);
    }

    public double HeightSvg
    {
        get => GetValue(HeightSvgProperty);
        set => SetValue(HeightSvgProperty, value);
    }

    public double TextMinWidth
    {
        get => GetValue(TextMinWidthProperty);
        set => SetValue(TextMinWidthProperty, value);
    }

    public double TextMinHeight
    {
        get => GetValue(TextMinHeightProperty);
        set => SetValue(TextMinHeightProperty, value);
    }

    public Thickness TextMargin
    {
        get => GetValue(TextMarginProperty);
        set => SetValue(TextMarginProperty, value);
    }

    public Thickness BorderMargin
    {
        get => GetValue(BorderMarginProperty);
        set => SetValue(BorderMarginProperty, value);
    }


    public string Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public bool IconVisible
    {
        get => GetValue(IconVisibleProperty);
        set => SetValue(IconVisibleProperty, value);
    }
}