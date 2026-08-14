package fewolearning.exercises.advanced.ex072_custom_collector;

import java.util.List;
import java.util.stream.Collector;

/*
Exercise 072 - Custom collector (reference solution).
*/
public final class CustomCollector {
    private CustomCollector() {
    }

    public static Collector<String, StringBuilder, String> joining(String separator) {
        return Collector.of(
                StringBuilder::new,
                (builder, value) -> {
                    if (builder.length() > 0) {
                        builder.append(separator);
                    }
                    builder.append(value);
                },
                (left, right) -> {
                    if (left.length() > 0 && right.length() > 0) {
                        left.append(separator);
                    }
                    return left.append(right);
                },
                StringBuilder::toString
        );
    }

    public static String joinAll(List<String> values, String separator) {
        return values.stream().collect(joining(separator));
    }
}
