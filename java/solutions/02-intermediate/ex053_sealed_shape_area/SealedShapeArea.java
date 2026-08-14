package fewolearning.exercises.intermediate.ex053_sealed_shape_area;

/*
Exercise 053 - Sealed shape area (reference solution).
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
        return switch (shape) {
            case Circle circle -> Math.PI * circle.radius() * circle.radius();
            case Rectangle rectangle -> rectangle.width() * rectangle.height();
        };
    }
}
