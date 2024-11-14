using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using CollectionRenderSandbox.ViewModels;

namespace CollectionRenderSandbox.Controls;
public class TestDataGrid : Control, ILogicalScrollable
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public bool CanHorizontallyScroll { get; set; } = false;
    public bool CanVerticallyScroll { get; set; } = true;
    public bool IsLogicalScrollEnabled { get; } = true;
    public Size ScrollSize { get; private set; } = new(1, 1);
    public Size PageScrollSize { get; private set; }
    public Size Extent { get; private set; }
    public Size Viewport { get; private set; }
    public Vector Offset
    {
        get => _offset;
        set
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            _offset = value;
            InvalidateScrollable();
            _isUpdating = false;
        }
    }

    public event EventHandler? ScrollInvalidated;

    private FontFamily? _fontFamily;
    private double _fontSize;
    private IBrush? _foreground;
    private Typeface _typeface;
    private double _lineHeight;
    private TextRunProperties? _textRunProperties;
    private TextShaperOptions? _textShaperOptions;

    private Vector _offset;
    private bool _isUpdating;

    private List<FormattedText?> _textCache = [];
    private List<ShapedTextRun?> _runCache = [];
    private List<RenderTargetBitmap?> _bitmapCache = [];
    private string _longText = "aljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfslj";

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _textCache = new(new FormattedText?[ViewModel.Items.Count]);
        _runCache = new(new ShapedTextRun?[ViewModel.Items.Count]);
        _bitmapCache = new(new RenderTargetBitmap?[ViewModel.Items.Count]);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        Invalidate();
        InvalidateScrollable();
    }

    public bool BringIntoView(Control target, Rect targetRect)
    {
        return true;
        //throw new NotImplementedException();
    }

    public Control? GetControlInDirection(NavigationDirection direction, Control? from)
    {
        return null;
        //throw new NotImplementedException();
    }

    public void RaiseScrollInvalidated(EventArgs e)
    {
        ScrollInvalidated?.Invoke(this, e);
    }

    public override void Render(DrawingContext context)
    {
        Invalidate();

        base.Render(context);

        double y = 0;
        int startIndex = (int)(Offset.Y / _lineHeight);
        int viewPortLines = (int)(Viewport.Height / _lineHeight) + 1; // Add 1 to render a partially on-screen element
        int lastIndex = Math.Clamp(startIndex + viewPortLines, 0, ViewModel.Items.Count - 1);

        for (int i = startIndex; i <= lastIndex; i++)
        {
            //y += RenderFormattedText(context, i, y);
            y += RenderShapedTextRun(context, i, y);
            //y += RenderRenderTargetBitmap(context, i, y);
        }
    }

    public double RenderFormattedText(DrawingContext context, int i, double y)
    {
        var item = ViewModel.Items[i];

        FormattedText? ft = _textCache[i];
        if (ft is null)
        {
            var text = $"{i}: {item} {_longText}";
            ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, _fontSize, _foreground);
            ft.SetFontWeight(FontWeight.Bold);
            _textCache[i] = ft;
        }

        context.DrawText(ft, new Point(0, y));
        return ft.Height;
    }

    public double RenderShapedTextRun(DrawingContext context, int i, double y)
    {
        var item = ViewModel.Items[i];

        ShapedTextRun? run = _runCache[i];
        if (run is null)
        {
            var text = $"{i}: {item} {_longText}";
            var sb = TextShaper.Current.ShapeText(text, _textShaperOptions!.Value);
            run = new ShapedTextRun(sb, _textRunProperties!);
            _runCache[i] = run;
        }

        using (context.PushTransform(Matrix.CreateTranslation(0, y)))
        {
            context.DrawGlyphRun(_textRunProperties!.ForegroundBrush, run.GlyphRun);
        }

        return run.TextMetrics.LineHeight;
    }

    public double RenderRenderTargetBitmap(DrawingContext context, int i, double y)
    {
        var item = ViewModel.Items[i];

        RenderTargetBitmap? bitmap = _bitmapCache[i];
        if (bitmap is null)
        {
            var text = $"{i}: {item} {_longText}";

            var sb = TextShaper.Current.ShapeText(text, _textShaperOptions!.Value);
            var run = new ShapedTextRun(sb, _textRunProperties!);

            bitmap = new RenderTargetBitmap(new((int)Math.Ceiling(run.Size.Width), (int)Math.Ceiling(run.Size.Height)));
            using (var rtbContext = bitmap.CreateDrawingContext())
            {
                rtbContext.DrawGlyphRun(_textRunProperties!.ForegroundBrush, run.GlyphRun);
            }

            _bitmapCache[i] = bitmap;
            run.Dispose();
        }

        using (context.PushTransform(Matrix.CreateTranslation(0, y)))
        {
            context.DrawImage(bitmap, new Rect(0, y, bitmap.PixelSize.Width, bitmap.PixelSize.Height));
        }

        return bitmap.PixelSize.Height;
    }

    private FormattedText CreateFormattedText(string text)
    {
        return new FormattedText(text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            _typeface,
            _fontSize,
            _foreground);
    }

    private void Invalidate()
    {
        _fontFamily = TextElement.GetFontFamily(this);
        _fontSize = TextElement.GetFontSize(this);
        _foreground = TextElement.GetForeground(this);
        _typeface = new Typeface(_fontFamily);

        _lineHeight = CreateFormattedText("0").Height;

        _textRunProperties = new GenericTextRunProperties(_typeface, _fontSize, null, Brushes.Purple, Brushes.Red);
        _textShaperOptions = new TextShaperOptions(_typeface.GlyphTypeface, _fontSize);
    }

    public void InvalidateScrollable()
    {
        if (this is not ILogicalScrollable scrollable || DataContext is null)
        {
            return;
        }

        var lines = ViewModel.Items.Count;
        var width = Bounds.Width;
        var height = Bounds.Height;

        ScrollSize = new Size(1, _fontSize);
        PageScrollSize = new Size(Viewport.Width, Viewport.Height);
        Extent = new Size(width, lines * _fontSize);
        Viewport = new Size(width, height);

        scrollable.RaiseScrollInvalidated(EventArgs.Empty);

        InvalidateVisual();
    }


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BoundsProperty)
        {
            InvalidateScrollable();
        }

        if (change.Property == TextElement.FontFamilyProperty
            || change.Property == TextElement.FontSizeProperty
            || change.Property == TextElement.ForegroundProperty)
        {
            Invalidate();
            InvalidateScrollable();
        }
    }
}
