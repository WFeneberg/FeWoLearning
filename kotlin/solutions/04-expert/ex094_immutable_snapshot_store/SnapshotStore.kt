package fewolearning.exercises.expert.ex094_immutable_snapshot_store

/**
 * `revertTo` truncates history so everything after the target index is discarded, not
 * merely returning the old value while leaving later snapshots dangling - a subsequent
 * `commit` appends right after the reverted-to entry, exactly as if the later history
 * had never existed. Out-of-bounds indices throw naturally via `List` indexing.
 */
class SnapshotStore<T>(initial: T) {
    private val history = mutableListOf(initial)

    fun current(): T = history.last()

    fun commit(next: T) {
        history.add(next)
    }

    fun revertTo(index: Int): T {
        val value = history[index]
        history.subList(index + 1, history.size).clear()
        return value
    }
}
