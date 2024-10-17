using Avalonia.Media.Fonts;
using System;

namespace LuYao.Avalonia.Fonts.MiSans.Regular;

public sealed class MiSansRegularFontCollection : EmbeddedFontCollection
{
    public MiSansRegularFontCollection() : base(
        new Uri("fonts:MiSans", UriKind.Absolute),
        new Uri("avares://LuYao.Avalonia.Fonts.MiSans.Regular/Fonts", UriKind.Absolute)
        )
    {

    }
}
