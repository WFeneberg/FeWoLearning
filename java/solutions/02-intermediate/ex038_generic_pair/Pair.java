package fewolearning.exercises.intermediate.ex038_generic_pair;

/*
Exercise 038 - Generic pair (reference solution).
*/
public record Pair<A, B>(A first, B second) {
    public Pair<B, A> swap() {
        return new Pair<>(second, first);
    }
}
