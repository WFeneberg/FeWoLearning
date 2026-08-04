package fewolearning.exercises.beginner.ex023_interface_default_impl

/*
Exercise 023 - Interface default implementation (beginner).

Goal:   Provide a default greet() implementation on an interface.
Drills: interfaces, default implementations.
*/
interface Greeter {
    val name: String

    fun greet(): String {
        TODO()
    }
}

class FormalGreeter(override val name: String) : Greeter
