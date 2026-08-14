package fewolearning.exercises.beginner.ex027_try_with_resources;

/*
Exercise 027 - Try-with-resources (reference solution).
*/
public final class TryWithResources {
    private TryWithResources() {
    }

    public static String readAndClose(TrackedResource resource) {
        try (resource) {
            return resource.read();
        }
    }

    public static final class TrackedResource implements AutoCloseable {
        private boolean closed;

        public String read() {
            if (closed) {
                throw new IllegalStateException("resource is already closed");
            }
            return "data";
        }

        public boolean isClosed() {
            return closed;
        }

        @Override
        public void close() {
            closed = true;
        }
    }
}
