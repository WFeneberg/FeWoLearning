using Avalonia;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex074_
public static class Ex074_GeometryAndPen
{
    public static PathGeometry BuildChevron(double width, double height, double inset) =>
        new()
        {
            Figures =
            [
                Figure(
                    new Point(0, 0),
                    new Point(width - inset, 0),
                    new Point(width, height / 2),
                    new Point(width - inset, height),
                    new Point(0, height),
                    new Point(inset, height / 2)),
            ],
        };

    public static Pen BuildPen() =>
        new(Brushes.Black, 4)
        {
            LineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round,
            DashStyle = new DashStyle([2, 2], 0),
        };

    public static PathGeometry BuildRing(FillRule rule) =>
        new()
        {
            FillRule = rule,
            Figures =
            [
                Square(0, 40),
                Square(10, 30),
            ],
        };

    private static PathFigure Square(double from, double to) =>
        Figure(
            new Point(from, from),
            new Point(to, from),
            new Point(to, to),
            new Point(from, to));

    private static PathFigure Figure(Point start, params Point[] rest)
    {
        var figure = new PathFigure
        {
            StartPoint = start,
            IsClosed = true,
            IsFilled = true,
        };

        foreach (var point in rest)
        {
            figure.Segments!.Add(new LineSegment { Point = point });
        }

        return figure;
    }
}
