package fewolearning.exercises.beginner.ex034_delegated_lazy

/*
Exercise 034 - Delegated lazy (reference solution).
*/
class ExpensiveConfig(private val loader: () -> String) {
    val value: String by lazy {
        loader()
    }
}
