package fewolearning.exercises.beginner.ex031_class_invariant;

/*
Exercise 031 - Class invariant (reference solution).
*/
public final class Person {
    private final String name;
    private final int age;

    public Person(String name, int age) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("name must not be blank");
        }
        if (age < 0 || age > 150) {
            throw new IllegalArgumentException("age must be between 0 and 150: " + age);
        }
        this.name = name;
        this.age = age;
    }

    public String name() {
        return name;
    }

    public int age() {
        return age;
    }
}
