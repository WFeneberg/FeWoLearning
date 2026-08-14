package fewolearning.exercises.beginner.ex032_inheritance_override;

/*
Exercise 032 - Inheritance override (reference solution).
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
            return super.describe() + " (a dog)";
        }
    }
}
