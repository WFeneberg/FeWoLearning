package fewolearning.exercises.advanced.ex082_annotation_retention;

import java.util.NoSuchElementException;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class AnnotationRetentionDemoTest {

    @Test
    void readsTheReasonFromAnAnnotatedMethod() throws NoSuchMethodException {
        String reason = AnnotationRetentionDemo.readImportantReason(SampleTarget.class, "annotatedMethod");

        assertEquals("critical path", reason);
    }

    @Test
    void throwsWhenTheMethodHasNoImportantAnnotation() {
        assertThrows(NoSuchElementException.class,
                () -> AnnotationRetentionDemo.readImportantReason(SampleTarget.class, "plainMethod"));
    }

    @Test
    void throwsNoSuchMethodExceptionWhenTheMethodDoesNotExist() {
        assertThrows(NoSuchMethodException.class,
                () -> AnnotationRetentionDemo.readImportantReason(SampleTarget.class, "missingMethod"));
    }

    private static final class SampleTarget {
        @AnnotationRetentionDemo.Important(reason = "critical path")
        public void annotatedMethod() {
        }

        public void plainMethod() {
        }
    }
}
