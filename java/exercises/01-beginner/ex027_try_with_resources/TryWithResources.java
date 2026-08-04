package fewolearning.exercises.beginner.ex027_try_with_resources;

/*
Exercise 027 - Try-with-resources (beginner).

Goal:   Use try-with-resources to guarantee a resource is closed exactly once.
Drills: automatic closing, resource safety.
*/
public final class TryWithResources {
    private TryWithResources() {
    }

    public static String readAndClose(TrackedResource resource) {
        throw new UnsupportedOperationException("TODO");
    }

    public static final class TrackedResource implements AutoCloseable {
        private boolean closed;

        public String read() {
            throw new UnsupportedOperationException("TODO");
        }

        public boolean isClosed() {
            throw new UnsupportedOperationException("TODO");
        }

        @Override
        public void close() {
            throw new UnsupportedOperationException("TODO");
        }
    }
}
