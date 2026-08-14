package fewolearning.exercises.intermediate.ex055_pattern_switch;

import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PatternSwitchTest {

    @Test
    void describesAString() {
        assertEquals("String[3]", PatternSwitch.describe("abc"));
    }

    @Test
    void describesAnInteger() {
        assertEquals("Integer[42]", PatternSwitch.describe(42));
    }

    @Test
    void describesAList() {
        assertEquals("List[2]", PatternSwitch.describe(List.of("a", "b")));
    }

    @Test
    void describesAnUnrecognizedType() {
        assertEquals("Unknown", PatternSwitch.describe(3.14));
    }

    @Test
    void describesNull() {
        assertEquals("Null", PatternSwitch.describe(null));
    }
}
