package fewolearning.exercises.intermediate.ex037_generic_box;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BoxTest {

    @Test
    void isEmptyIsTrueForANewBox() {
        Box<String> box = new Box<>();

        assertTrue(box.isEmpty());
    }

    @Test
    void setStoresAValueAndClearsTheEmptyFlag() {
        Box<String> box = new Box<>();

        box.set("hello");

        assertFalse(box.isEmpty());
    }

    @Test
    void getReturnsTheStoredValue() {
        Box<Integer> box = new Box<>();
        box.set(42);

        assertEquals(42, box.get());
    }

    @Test
    void getThrowsWhenTheBoxIsEmpty() {
        Box<String> box = new Box<>();

        assertThrows(IllegalStateException.class, box::get);
    }
}
