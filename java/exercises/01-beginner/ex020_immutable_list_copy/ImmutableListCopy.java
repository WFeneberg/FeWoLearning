package fewolearning.exercises.beginner.ex020_immutable_list_copy;

import java.util.List;

/*
Exercise 020 - Immutable list copy (beginner).

Goal:   Defensively copy input lists so callers cannot mutate internal state.
Drills: defensive copying, List.copyOf.
*/
public final class ImmutableListCopy {
    private ImmutableListCopy() {
    }

    public static List<String> toImmutableCopy(List<String> source) {
        throw new UnsupportedOperationException("TODO");
    }

    public static boolean isMutationBlocked(List<String> immutableList) {
        throw new UnsupportedOperationException("TODO");
    }
}
