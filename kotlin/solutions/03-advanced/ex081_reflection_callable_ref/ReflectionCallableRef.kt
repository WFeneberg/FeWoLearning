package fewolearning.exercises.advanced.ex081_reflection_callable_ref

import kotlin.reflect.KFunction1

/** Applies a callable reference to every element - a KFunction1 is directly invokable as a function. */
fun <T, R> applyAll(items: List<T>, function: KFunction1<T, R>): List<R> =
    items.map { function(it) }
