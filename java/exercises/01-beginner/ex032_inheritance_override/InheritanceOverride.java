package fewolearning.exercises.beginner.ex032_inheritance_override;

/*
Exercise 032 - Inheritance override (beginner).

Goal:   Override a base method and extend its behavior using super.
Drills: inheritance, overriding, super.
*/
public final class InheritanceOverride {
    private InheritanceOverride() {
    }

    public static class Animal {
        public String describe() {
            return "an animal";
        }
    }

    public static class Dog extends Animal {
        @Override
        public String describe() {
            throw new UnsupportedOperationException("TODO");
        }
    }
}
