package fewolearning.exercises.advanced.ex081_reflection_callable_ref

import kotlin.reflect.KFunction1

/*
Exercise 081 - Reflection callable reference (advanced).

Goal:   Apply a callable reference to every element of a list.
Drills: reflection, callable references.
*/
fun <T, R> applyAll(items: List<T>, function: KFunction1<T, R>): List<R> {
    TODO()
}
