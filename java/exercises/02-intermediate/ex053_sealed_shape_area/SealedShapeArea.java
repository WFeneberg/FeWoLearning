package fewolearning.exercises.intermediate.ex053_sealed_shape_area;

/*
Exercise 053 - Sealed shape area (intermediate).

Goal:   Compute the area of each permitted shape using exhaustive branching.
Drills: sealed hierarchies, exhaustive branching.
*/
public final class SealedShapeArea {
    private SealedShapeArea() {
    }

    public sealed interface Shape permits Circle, Rectangle {
    }

    public record Circle(double radius) implements Shape {
    }

    public record Rectangle(double width, double height) implements Shape {
    }

    public static double area(Shape shape) {
        throw new UnsupportedOperationException("TODO");
    }
}
