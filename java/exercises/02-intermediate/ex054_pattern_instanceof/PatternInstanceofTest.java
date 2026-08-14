package fewolearning.exercises.intermediate.ex054_pattern_instanceof;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PatternInstanceofTest {

    @Test
    void describesAString() {
        assertEquals("String[3]", PatternInstanceof.describe("abc"));
    }

    @Test
    void describesAnInteger() {
        assertEquals("Integer[42]", PatternInstanceof.describe(42));
    }

    @Test
    void describesAList() {
        assertEquals("List[2]", PatternInstanceof.describe(List.of("a", "b")));
    }

    @Test
    void describesAnUnrecognizedType() {
        assertEquals("Unknown", PatternInstanceof.describe(3.14));
    }
}
