package fewolearning.exercises.intermediate.ex061_coroutine_launch_join

import kotlinx.coroutines.CoroutineScope

/*
Exercise 061 - Coroutine launch/join (intermediate).

Goal:   Launch two child coroutines and wait for both before returning.
Drills: structured coroutines, launch, join.
*/
suspend fun runBothAndWait(scope: CoroutineScope, first: suspend () -> Unit, second: suspend () -> Unit) {
    TODO()
}
