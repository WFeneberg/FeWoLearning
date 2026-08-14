package fewolearning.exercises.beginner.ex022_companion_factory

/*
Exercise 022 - Companion factory (reference solution).
*/
class User private constructor(val name: String, val email: String) {
    companion object {
        fun of(name: String, email: String): User = User(name, email)
    }
}
