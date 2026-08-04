package fewolearning.exercises.beginner.ex024_inheritance_override

/*
Exercise 024 - Inheritance override (beginner).

Goal:   Override a base method and extend its behavior using super.
Drills: open classes, overriding, super.
*/
open class Animal {
    open fun describe(): String = "an animal"
}

class Dog : Animal() {
    override fun describe(): String {
        TODO()
    }
}
