package fewolearning.exercises.intermediate.ex043_map_compute_if_absent;

import java.util.AbstractMap;
import java.util.List;
import java.util.Map;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class MapComputeIfAbsentTest {

    @Test
    void groupsValuesUnderTheirKeys() {
        List<Map.Entry<String, Integer>> entries = List.of(
                new AbstractMap.SimpleEntry<>("a", 1),
                new AbstractMap.SimpleEntry<>("b", 2),
                new AbstractMap.SimpleEntry<>("a", 3));

        Map<String, List<Integer>> grouped = MapComputeIfAbsent.groupByKey(entries);

        assertEquals(List.of(1, 3), grouped.get("a"));
        assertEquals(List.of(2), grouped.get("b"));
    }

    @Test
    void returnsAnEmptyMapForNoEntries() {
        Map<String, List<Integer>> grouped = MapComputeIfAbsent.groupByKey(List.of());

        assertTrue(grouped.isEmpty());
    }
}
