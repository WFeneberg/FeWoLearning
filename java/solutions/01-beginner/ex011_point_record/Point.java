package fewolearning.exercises.beginner.ex011_point_record;

/*
Exercise 011 - Point record (reference solution).
*/
public record Point(double x, double y) {

    public double distanceTo(Point other) {
        double dx = other.x - this.x;
        double dy = other.y - this.y;
        return Math.sqrt(dx * dx + dy * dy);
    }

    public Point translate(double dx, double dy) {
        return new Point(this.x + dx, this.y + dy);
    }
}
