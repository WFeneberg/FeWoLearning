package fewolearning.exercises.beginner.ex034_delegated_lazy

/*
Exercise 034 - Delegated lazy (beginner).

Goal:   Defer an expensive computation until its value is first accessed.
Drills: lazy, deferred initialization.
*/
class ExpensiveConfig(private val loader: () -> String) {
    val value: String by lazy {
        TODO()
    }
}
