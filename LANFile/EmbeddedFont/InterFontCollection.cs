using System;
using Avalonia.Media.Fonts;

namespace LANFile.EmbeddedFont;

public sealed class InterFontCollection : EmbeddedFontCollection
{
    public InterFontCollection() : base(
        new Uri("fonts:Inter", UriKind.Absolute),
        new Uri("avares://Avalonia.Fonts.Inter/Assets/Fonts", UriKind.Absolute))
    {
    }
}