package fewolearning.exercises.intermediate.ex051_delegated_observable

import kotlin.properties.Delegates

/*
Exercise 051 - Delegated observable (intermediate).

Goal:   React to changes of a property using Delegates.observable.
Drills: Delegates.observable, change hooks.
*/
class ThemeSettings {
    val changeLog = mutableListOf<String>()

    var themeName: String by Delegates.observable("light") { _, old, new ->
        TODO()
    }
}
