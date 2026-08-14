package fewolearning.exercises.expert.ex095_annotation_processor;

import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

/*
Exercise 095 - Annotation processor (reference solution).

Naming convention enforced: every element annotated with @GeneratedName must
carry a name that starts with "Generated", followed by an uppercase letter and
then any number of letters or digits (UpperCamelCase), e.g. "GeneratedUserDto".
*/
public final class AnnotationProcessorDemo {
    private static final Pattern GENERATED_NAME = Pattern.compile("Generated[A-Z][A-Za-z0-9]*");

    private AnnotationProcessorDemo() {
    }

    public @interface GeneratedName {
        String value();
    }

    public static List<String> validateNames(List<String> annotatedElementNames) {
        List<String> violations = new ArrayList<>();
        for (String name : annotatedElementNames) {
            if (!GENERATED_NAME.matcher(name).matches()) {
                violations.add(name);
            }
        }
        return violations;
    }
}
