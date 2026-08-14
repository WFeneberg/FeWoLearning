package fewolearning.exercises.beginner.ex033_interface_default_method;

/*
Exercise 033 - Interface default method (reference solution).
*/
public final class InterfaceDefaultMethod {
    private InterfaceDefaultMethod() {
    }

    public interface Greeter {
        String name();

        default String greet() {
            return "Hello, " + name() + "!";
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
