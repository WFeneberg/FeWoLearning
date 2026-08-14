package fewolearning.exercises.beginner.ex001_primitive_math;

/*
Exercise 001 - Primitive math (reference solution).
*/
public final class PrimitiveMath {
    private PrimitiveMath() {
    }

    public static int sum(int left, int right) {
        return left + right;
    }

    public static int quotient(int dividend, int divisor) {
        return dividend / divisor;
    }

    public static int averageRoundedDown(int first, int second, int third) {
        return (first + second + third) / 3;
    }
}
