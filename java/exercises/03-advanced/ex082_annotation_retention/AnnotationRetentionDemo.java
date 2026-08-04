package fewolearning.exercises.advanced.ex082_annotation_retention;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/*
Exercise 082 - Annotation retention (advanced).

Goal:   Define a runtime-visible annotation and read it back via reflection.
Drills: annotation targets, runtime visibility.
*/
public final class AnnotationRetentionDemo {
    private AnnotationRetentionDemo() {
    }

    @Retention(RetentionPolicy.RUNTIME)
    @Target(ElementType.METHOD)
    public @interface Important {
        String reason();
    }

    public static String readImportantReason(Class<?> type, String methodName) throws NoSuchMethodException {
        throw new UnsupportedOperationException("TODO");
    }
}
