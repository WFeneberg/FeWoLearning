package fewolearning.exercises.beginner.ex020_immutable_list_copy;

import org.junit.jupiter.api.Test;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ImmutableListCopyTest {

    @Test
    void toImmutableCopyContainsTheSameElements() {
        List<String> source = new ArrayList<>(List.of("a", "b", "c"));

        List<String> copy = ImmutableListCopy.toImmutableCopy(source);

        assertEquals(List.of("a", "b", "c"), copy);
    }

    @Test
    void toImmutableCopyIsNotAffectedByLaterMutationOfTheSource() {
        List<String> source = new ArrayList<>(List.of("a", "b"));

        List<String> copy = ImmutableListCopy.toImmutableCopy(source);
        source.add("c");

        assertEquals(List.of("a", "b"), copy);
    }

    @Test
    void isMutationBlockedIsTrueForAnImmutableCopy() {
        List<String> copy = ImmutableListCopy.toImmutableCopy(List.of("a", "b"));

        assertTrue(ImmutableListCopy.isMutationBlocked(copy));
    }
}
