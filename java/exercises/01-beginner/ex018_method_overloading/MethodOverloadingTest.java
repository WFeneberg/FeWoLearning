package fewolearning.exercises.beginner.ex018_method_overloading;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class MethodOverloadingTest {

    @Test
    void describeResolvesTheIntOverload() {
        assertEquals("int: 42", MethodOverloading.describe(42));
    }

    @Test
    void describeResolvesTheDoubleOverload() {
        assertEquals("double: 3.5", MethodOverloading.describe(3.5));
    }

    @Test
    void describeResolvesTheStringOverload() {
        assertEquals("String: hello", MethodOverloading.describe("hello"));
    }

    @Test
    void describeResolvesTheTwoArgOverloadByArity() {
        assertEquals("5.00", MethodOverloading.describe(5, 2));
        assertEquals("5.0", MethodOverloading.describe(5, 1));
    }
}
