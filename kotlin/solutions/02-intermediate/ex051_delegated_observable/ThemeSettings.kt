package fewolearning.exercises.intermediate.ex051_delegated_observable

import kotlin.properties.Delegates

/** Records every themeName transition using Delegates.observable. */
class ThemeSettings {
    val changeLog = mutableListOf<String>()

    var themeName: String by Delegates.observable("light") { _, old, new ->
        changeLog.add("$old -> $new")
    }
}
