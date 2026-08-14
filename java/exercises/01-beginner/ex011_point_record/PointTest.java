package fewolearning.exercises.beginner.ex011_point_record;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PointTest {

    @Test
    void distanceToComputesEuclideanDistance() {
        Point origin = new Point(0, 0);
        Point target = new Point(3, 4);

        assertEquals(5.0, origin.distanceTo(target), 1e-9);
    }

    @Test
    void distanceToItselfIsZero() {
        Point point = new Point(2.5, -1.5);

        assertEquals(0.0, point.distanceTo(point), 1e-9);
    }

    @Test
    void translateReturnsANewShiftedPoint() {
        Point point = new Point(1.0, 2.0);

        Point translated = point.translate(3.0, -1.0);

        assertEquals(new Point(4.0, 1.0), translated);
    }

    @Test
    void translateDoesNotChangeTheOriginalPoint() {
        Point point = new Point(1.0, 2.0);

        point.translate(5.0, 5.0);

        assertEquals(1.0, point.x(), 1e-9);
        assertEquals(2.0, point.y(), 1e-9);
    }
}
