using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OtcDataService.Views;

internal static class WindowIconFactory
{
    public static WindowIcon Create(string key)
    {
        var definition = WindowIconCatalog.Get(key);
        var size = definition.Size;
        var margin = definition.Margin;
        var innerSize = size - margin * 2;

        var visual = new Canvas
        {
            Width = size,
            Height = size,
            Children =
            {
                new Avalonia.Controls.Shapes.Path
                {
                    Data = definition.ParsedGeometry,
                    Fill = new SolidColorBrush(definition.ParsedFill),
                    Width = innerSize,
                    Height = innerSize,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(margin)
                }
            }
        };

        visual.Measure(new Size(size, size));
        visual.Arrange(new Rect(0, 0, size, size));

        var bitmap = new RenderTargetBitmap(new PixelSize(size, size), new Vector(96, 96));
        bitmap.Render(visual);

        var stream = new MemoryStream();
        bitmap.Save(stream);
        stream.Position = 0;
        return new WindowIcon(stream);
    }
}
