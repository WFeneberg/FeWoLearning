package fewolearning.exercises.beginner.ex024_inheritance_override

/*
Exercise 024 - Inheritance override (reference solution).
*/
open class Animal {
    open fun describe(): String = "an animal"
}

class Dog : Animal() {
    override fun describe(): String = "${super.describe()}, specifically a dog"
}
