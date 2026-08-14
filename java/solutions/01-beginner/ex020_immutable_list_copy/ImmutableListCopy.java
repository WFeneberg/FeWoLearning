package fewolearning.exercises.beginner.ex020_immutable_list_copy;

import java.util.List;

/*
Exercise 020 - Immutable list copy (reference solution).
*/
public final class ImmutableListCopy {
    private ImmutableListCopy() {
    }

    public static List<String> toImmutableCopy(List<String> source) {
        return List.copyOf(source);
    }

    public static boolean isMutationBlocked(List<String> immutableList) {
        try {
            immutableList.add("__probe__");
            return false;
        } catch (UnsupportedOperationException expected) {
            return true;
        }
    }
}
