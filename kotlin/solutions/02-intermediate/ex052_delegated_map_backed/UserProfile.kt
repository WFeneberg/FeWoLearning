package fewolearning.exercises.intermediate.ex052_delegated_map_backed

/** Backs each property by a mutable map using Kotlin's map-delegated properties. */
class UserProfile(private val source: MutableMap<String, Any?>) {
    val name: String by source
    val age: Int by source
}
