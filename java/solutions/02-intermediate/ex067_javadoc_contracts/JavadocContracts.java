package fewolearning.exercises.intermediate.ex067_javadoc_contracts;

/*
Exercise 067 - Javadoc contracts (reference solution).
*/
public final class JavadocContracts {
    private JavadocContracts() {
    }

    /**
     * Divides {@code dividend} by {@code divisor} using integer division.
     *
     * <p>The result is truncated toward zero, matching Java's native {@code int}
     * division semantics (for example, {@code divide(7, 2)} returns {@code 3},
     * and {@code divide(-7, 2)} returns {@code -3}).
     *
     * @param dividend the value to be divided
     * @param divisor  the value to divide by
     * @return the truncated quotient of {@code dividend / divisor}
     * @throws ArithmeticException if {@code divisor} is zero
     */
    public static int divide(int dividend, int divisor) {
        return dividend / divisor;
    }
}
