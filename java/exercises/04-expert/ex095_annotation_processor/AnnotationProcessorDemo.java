package fewolearning.exercises.expert.ex095_annotation_processor;

import java.util.List;

/*
Exercise 095 - Annotation processor (expert).

Goal:   Validate that every annotated element name follows the expected naming convention.
Drills: code generation, compile-time validation.
*/
public final class AnnotationProcessorDemo {
    private AnnotationProcessorDemo() {
    }

    public @interface GeneratedName {
        String value();
    }

    public static List<String> validateNames(List<String> annotatedElementNames) {
        throw new UnsupportedOperationException("TODO");
    }
}
