package fewolearning.exercises.advanced.ex075_select_expression

import kotlinx.coroutines.channels.ReceiveChannel

/*
Exercise 075 - Select expression (advanced).

Goal:   Receive whichever of two channels produces a value first.
Drills: selecting the first available coroutine event.
*/
suspend fun <T> firstAvailable(first: ReceiveChannel<T>, second: ReceiveChannel<T>): T {
    TODO()
}
