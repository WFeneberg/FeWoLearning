package fewolearning.exercises.intermediate.ex039_collection_associate_by

/*
Exercise 039 - Collection associateBy (intermediate).

Goal:   Index people by id, keeping the last entry when ids collide.
Drills: associateBy, key collisions.
*/
data class Person(val id: Int, val name: String)

fun indexById(people: List<Person>): Map<Int, Person> {
    TODO()
}
