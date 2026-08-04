package fewolearning.exercises.intermediate.ex052_delegated_map_backed

/*
Exercise 052 - Map-backed delegated properties (intermediate).

Goal:   Back a class's properties by a mutable map using property delegation.
Drills: map-backed properties, dynamic models.
*/
class UserProfile(private val source: MutableMap<String, Any?>) {
    val name: String
        get() = TODO()

    val age: Int
        get() = TODO()
}
