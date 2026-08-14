package fewolearning.exercises.beginner.ex019_varargs_join;

/*
Exercise 019 - Varargs join (reference solution).
*/
public final class VarargsJoin {
    private VarargsJoin() {
    }

    public static String join(String separator, String... parts) {
        return String.join(separator, parts);
    }

    public static int sumAll(int... numbers) {
        int sum = 0;
        for (int number : numbers) {
            sum += number;
        }
        return sum;
    }
}
