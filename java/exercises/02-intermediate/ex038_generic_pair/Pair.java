package fewolearning.exercises.intermediate.ex038_generic_pair;

/*
Exercise 038 - Generic pair (intermediate).

Goal:   Model a two-value tuple and a method that swaps its elements.
Drills: multiple type parameters, tuple-like modeling.
*/
public record Pair<A, B>(A first, B second) {
    public Pair<B, A> swap() {
        throw new UnsupportedOperationException("TODO");
    }
}
