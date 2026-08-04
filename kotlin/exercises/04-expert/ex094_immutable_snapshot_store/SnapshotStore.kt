package fewolearning.exercises.expert.ex094_immutable_snapshot_store

/*
Exercise 094 - Immutable snapshot store (expert).

Goal:   Keep a history of immutable snapshots and support reverting to a prior one.
Drills: persistent-style state snapshots.
*/
class SnapshotStore<T>(initial: T) {
    private val history = mutableListOf(initial)

    fun current(): T {
        TODO()
    }

    fun commit(next: T) {
        TODO()
    }

    fun revertTo(index: Int): T {
        TODO()
    }
}
