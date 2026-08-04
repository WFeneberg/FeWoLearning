package fewolearning.exercises.intermediate.ex041_comparable_person;

/*
Exercise 041 - Comparable person (intermediate).

Goal:   Order people by age, then by name, implementing Comparable consistently.
Drills: Comparable, consistent ordering.
*/
public final class ComparablePerson implements Comparable<ComparablePerson> {
    private final String name;
    private final int age;

    public ComparablePerson(String name, int age) {
        this.name = name;
        this.age = age;
    }

    public String name() {
        return name;
    }

    public int age() {
        return age;
    }

    @Override
    public int compareTo(ComparablePerson other) {
        throw new UnsupportedOperationException("TODO");
    }
}
