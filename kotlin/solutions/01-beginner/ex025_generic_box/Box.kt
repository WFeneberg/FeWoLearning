package fewolearning.exercises.beginner.ex025_generic_box

/*
Exercise 025 - Generic box (reference solution).
*/
class Box<T>(private var value: T? = null) {
    fun set(value: T) {
        this.value = value
    }

    fun get(): T? = value
}
