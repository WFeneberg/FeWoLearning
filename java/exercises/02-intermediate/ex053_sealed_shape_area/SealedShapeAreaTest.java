package fewolearning.exercises.intermediate.ex053_sealed_shape_area;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class SealedShapeAreaTest {

    @Test
    void computesTheAreaOfACircle() {
        double area = SealedShapeArea.area(new SealedShapeArea.Circle(2.0));

        assertEquals(Math.PI * 4.0, area, 1e-9);
    }

    @Test
    void computesTheAreaOfARectangle() {
        double area = SealedShapeArea.area(new SealedShapeArea.Rectangle(3.0, 4.0));

        assertEquals(12.0, area, 1e-9);
    }
}
