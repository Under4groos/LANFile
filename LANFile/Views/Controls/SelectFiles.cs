using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LANFile.Helper;

namespace LANFile.Views.Controls;

public class SelectFiles : ContentControl
{
    private Button? _buttonSelectFile;


    public static readonly StyledProperty<ObservableCollection<string>> FilesProperty =
        AvaloniaProperty.Register<LanDevice, ObservableCollection<string>>(nameof(Files));

    public ObservableCollection<string> Files
    {
        get => GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _buttonSelectFile = ControlHelper.FindTemplateControlByName<Button>(this, "ButtonSelectFile");
        if (_buttonSelectFile != null)
            _buttonSelectFile.Click += async (o, e) => { await ButtonSelectFileOnClick(o, e); };
    }

    private async Task ButtonSelectFileOnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null)
            return;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select files",
            AllowMultiple = true
        });
        ObservableCollection<string> _files = new ObservableCollection<string>();
        foreach (var file in files.Select(f => f.Path.AbsolutePath).ToArray())
        {
            _files.Add(file);
             
        }
        Files = _files;
    }
}