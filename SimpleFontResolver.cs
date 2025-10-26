using PdfSharp.Fonts;
using System;
using System.IO;

namespace Library_Final;
public class SimpleFontResolver : IFontResolver
{
    public static readonly SimpleFontResolver Instance = new SimpleFontResolver();

    public string DefaultFontName => "Arial";

    public byte[] GetFont(string faceName)
    {
        // Load Arial directly from the system fonts folder
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            "arial.ttf");
        return File.ReadAllBytes(path);
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        => new FontResolverInfo(DefaultFontName);
}

