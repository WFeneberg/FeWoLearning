package fewolearning.exercises.intermediate.ex041_comparable_person;

/*
Exercise 041 - Comparable person (reference solution).
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
        int byAge = Integer.compare(age, other.age);
        if (byAge != 0) {
            return byAge;
        }
        return name.compareTo(other.name);
    }
}
