package fewolearning.exercises.intermediate.ex039_collection_associate_by

data class Person(val id: Int, val name: String)

/** Indexes people by id, keeping the last entry seen when ids collide. */
fun indexById(people: List<Person>): Map<Int, Person> = people.associateBy { it.id }
