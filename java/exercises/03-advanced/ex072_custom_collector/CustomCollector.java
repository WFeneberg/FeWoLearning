package fewolearning.exercises.advanced.ex072_custom_collector;

import java.util.List;
import java.util.stream.Collector;

/*
Exercise 072 - Custom collector (advanced).

Goal:   Implement a Collector that joins strings with a separator via mutable reduction.
Drills: Collector contract, mutable reduction.
*/
public final class CustomCollector {
    private CustomCollector() {
    }

    public static Collector<String, StringBuilder, String> joining(String separator) {
        throw new UnsupportedOperationException("TODO");
    }

    public static String joinAll(List<String> values, String separator) {
        throw new UnsupportedOperationException("TODO");
    }
}
