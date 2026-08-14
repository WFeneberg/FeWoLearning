package fewolearning.exercises.expert.ex094_immutable_snapshot_store

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class SnapshotStoreTest {

    @Test
    fun currentReflectsTheMostRecentlyCommittedSnapshot() {
        val store = SnapshotStore(0)
        store.commit(1)
        store.commit(2)

        assertEquals(2, store.current())
    }

    @Test
    fun revertToTruncatesLaterHistorySoASubsequentCommitReplacesIt() {
        val store = SnapshotStore("v0")
        store.commit("v1")
        store.commit("v2")

        val reverted = store.revertTo(1)
        assertEquals("v1", reverted)
        assertEquals("v1", store.current())

        store.commit("v1-b")
        assertEquals("v1-b", store.current())

        // v2 used to be at index 2; after the revert truncated history, that index no
        // longer exists. A "revert without truncation" implementation would still find
        // it there and fail this assertion.
        assertThrows(IndexOutOfBoundsException::class.java) { store.revertTo(2) }
    }

    @Test
    fun revertToAnOutOfBoundsIndexThrows() {
        val store = SnapshotStore(0)

        assertThrows(IndexOutOfBoundsException::class.java) { store.revertTo(5) }
    }
}
