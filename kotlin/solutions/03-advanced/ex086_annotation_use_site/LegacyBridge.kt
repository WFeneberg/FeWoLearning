package fewolearning.exercises.advanced.ex086_annotation_use_site

/**
 * `@get:JvmName("fetchCount")` is a use-site targeted annotation: it renames only the generated
 * getter's bytecode name (so Java callers see `fetchCount()`), leaving the Kotlin-visible name
 * `count` untouched. That effect is only observable from Java bytecode, not from Kotlin test code,
 * so it is not something a JUnit test here can assert on.
 */
class LegacyBridge(rawCount: Int) {
    @get:JvmName("fetchCount")
    val count: Int = rawCount

    fun doubled(): Int = count * 2
}
