using Avalonia;

namespace LuYao.Avalonia.Fonts.MiSans.Regular;

public static class AppBuilderExtension
{
    public static AppBuilder WithMiSansRegularFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
        {
            fontManager.AddFontCollection(new MiSansRegularFontCollection());
        });
    }
}
