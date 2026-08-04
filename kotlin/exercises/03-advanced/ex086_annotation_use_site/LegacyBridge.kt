package fewolearning.exercises.advanced.ex086_annotation_use_site

/*
Exercise 086 - Annotation use-site targets (advanced).

Goal:   Apply a JvmName use-site annotation so the getter has an explicit Java name.
Drills: use-site targets, annotations.
*/
class LegacyBridge(rawCount: Int) {
    @get:JvmName("fetchCount")
    val count: Int = rawCount

    fun doubled(): Int {
        TODO()
    }
}
