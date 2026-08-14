package fewolearning.exercises.beginner.ex023_interface_default_impl

/*
Exercise 023 - Interface default implementation (reference solution).
*/
interface Greeter {
    val name: String

    fun greet(): String {
        return "Hello, my name is $name."
    }
}

class FormalGreeter(override val name: String) : Greeter
