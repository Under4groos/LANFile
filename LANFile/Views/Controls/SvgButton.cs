using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LANFile.Helper;

namespace LANFile.Views.Controls;

public class SvgButton : Button
{
    // IsVisible="{TemplateBinding Source, Converter={StaticResource SourceToVisibilityConverter}}"

    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<SvgButton, string>(nameof(Source));

    public static readonly StyledProperty<bool> IconVisibleProperty =
        AvaloniaProperty.Register<SvgButton, bool>(nameof(IconVisible), true);


    public static readonly StyledProperty<double> WidthSvgProperty =
        AvaloniaProperty.Register<SvgButton, double>(nameof(WidthSvg), 25);

    public static readonly StyledProperty<double> HeightSvgProperty =
        AvaloniaProperty.Register<SvgButton, double>(nameof(HeightSvg), 25);

    public static readonly StyledProperty<Thickness> BorderMarginProperty =
        AvaloniaProperty.Register<SvgButton, Thickness>(nameof(BorderMargin), new Thickness(5));

    public Thickness BorderMargin
    {
        get => GetValue(BorderMarginProperty);
        set => SetValue(BorderMarginProperty, value);
    }


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

    public Avalonia.Svg.Svg? SvgIcon { get; private set; }


    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        SvgIcon = ControlHelper.FindTemplateControlByName<Avalonia.Svg.Svg>(this, "_svg");
    }
}