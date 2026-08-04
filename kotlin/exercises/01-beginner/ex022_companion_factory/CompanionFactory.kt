package fewolearning.exercises.beginner.ex022_companion_factory

/*
Exercise 022 - Companion factory (beginner).

Goal:   Create User instances through a companion object factory method.
Drills: companion objects, factory methods.
*/
class User private constructor(val name: String, val email: String) {
    companion object {
        fun of(name: String, email: String): User {
            TODO()
        }
    }
}
