package fewolearning.exercises.intermediate.ex043_map_compute_if_absent;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/*
Exercise 043 - Map computeIfAbsent (reference solution).
*/
public final class MapComputeIfAbsent {
    private MapComputeIfAbsent() {
    }

    public static Map<String, List<Integer>> groupByKey(List<Map.Entry<String, Integer>> entries) {
        Map<String, List<Integer>> grouped = new LinkedHashMap<>();
        for (Map.Entry<String, Integer> entry : entries) {
            grouped.computeIfAbsent(entry.getKey(), key -> new ArrayList<>()).add(entry.getValue());
        }
        return grouped;
    }
}
