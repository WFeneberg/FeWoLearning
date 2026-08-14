package fewolearning.exercises.advanced.ex082_annotation_retention;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;
import java.lang.reflect.Method;
import java.util.NoSuchElementException;

/*
Exercise 082 - Annotation retention (reference solution).
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
        Method method = type.getMethod(methodName);
        Important annotation = method.getAnnotation(Important.class);
        if (annotation == null) {
            throw new NoSuchElementException("No @Important annotation present on method: " + methodName);
        }
        return annotation.reason();
    }
}
