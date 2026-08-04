package fewolearning.exercises.beginner.ex033_interface_default_method;

/*
Exercise 033 - Interface default method (beginner).

Goal:   Provide a default method on an interface and override it selectively.
Drills: interfaces, default methods.
*/
public final class InterfaceDefaultMethod {
    private InterfaceDefaultMethod() {
    }

    public interface Greeter {
        String name();

        default String greet() {
            throw new UnsupportedOperationException("TODO");
        }
    }

    public static class FormalGreeter implements Greeter {
        private final String name;

        public FormalGreeter(String name) {
            this.name = name;
        }

        @Override
        public String name() {
            return name;
        }
    }
}
