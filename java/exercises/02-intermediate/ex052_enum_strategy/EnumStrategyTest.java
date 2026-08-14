package fewolearning.exercises.intermediate.ex052_enum_strategy;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class EnumStrategyTest {

    @Test
    void addAppliesAddition() {
        assertEquals(7, EnumStrategy.ADD.apply(3, 4));
    }

    @Test
    void subtractAppliesSubtraction() {
        assertEquals(-1, EnumStrategy.SUBTRACT.apply(3, 4));
    }

    @Test
    void multiplyAppliesMultiplication() {
        assertEquals(12, EnumStrategy.MULTIPLY.apply(3, 4));
    }
}
