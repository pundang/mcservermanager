using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MCServerManager.Controls;

// This is AI-Generated, do not expect consistency
public class Sparkline : Control
{
    public static readonly StyledProperty<IEnumerable?> PointsProperty =
        AvaloniaProperty.Register<Sparkline, IEnumerable?>(nameof(Points));

    public static readonly StyledProperty<IBrush> StrokeProperty =
        AvaloniaProperty.Register<Sparkline, IBrush>(nameof(Stroke), Brushes.CornflowerBlue);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(StrokeThickness), 2d);

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(Minimum), 0d);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<Sparkline, double>(nameof(Maximum), 100d);

    public IEnumerable? Points { get => GetValue(PointsProperty); set => SetValue(PointsProperty, value); }
    public IBrush Stroke { get => GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    public double StrokeThickness { get => GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }

    static Sparkline() => AffectsRender<Sparkline>(StrokeProperty, StrokeThicknessProperty, MinimumProperty, MaximumProperty);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != PointsProperty) return;

        if (change.OldValue is INotifyCollectionChanged oldIncc) oldIncc.CollectionChanged -= OnPointsChanged;
        if (change.NewValue is INotifyCollectionChanged newIncc) newIncc.CollectionChanged += OnPointsChanged;
        InvalidateVisual();
    }

    void OnPointsChanged(object? sender, NotifyCollectionChangedEventArgs e) => InvalidateVisual();

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var values = Points?.Cast<object>().Select(Convert.ToDouble).ToList() ?? [];
        if (values.Count < 2 || Bounds.Width <= 0 || Bounds.Height <= 0) return;

        var min = Minimum;
        var max = Math.Max(Maximum, min + 0.0001);
        var w = Bounds.Width;
        var h = Bounds.Height;
        var step = w / (values.Count - 1);

        Point At(int i) => new(i * step, h - (values[i] - min) / (max - min) * h);

        var line = new StreamGeometry();
        using (var ctx = line.Open())
        {
            ctx.BeginFigure(At(0), false);
            for (var i = 1; i < values.Count; i++) ctx.LineTo(At(i));
        }

        var fill = new StreamGeometry();
        using (var ctx = fill.Open())
        {
            ctx.BeginFigure(new Point(0, h), true);
            for (var i = 0; i < values.Count; i++) ctx.LineTo(At(i));
            ctx.LineTo(new Point(w, h));
        }

        var c = Stroke is ISolidColorBrush scb ? scb.Color : Colors.CornflowerBlue;
        var fillBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(60, c.R, c.G, c.B), 0),
                new GradientStop(Color.FromArgb(0, c.R, c.G, c.B), 1)
            }
        };

        context.DrawGeometry(fillBrush, null, fill);
        context.DrawGeometry(null, new Pen(Stroke, StrokeThickness), line);
    }
}
