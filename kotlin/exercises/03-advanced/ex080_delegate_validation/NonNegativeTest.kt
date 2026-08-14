package fewolearning.exercises.advanced.ex080_delegate_validation

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertThrows

class NonNegativeTest {

    private class Account {
        var balance: Int by NonNegative(0)
    }

    @Test
    fun acceptsAndReflectsANonNegativeAssignment() {
        val account = Account()

        account.balance = 50

        assertEquals(50, account.balance)
    }

    @Test
    fun rejectsANegativeAssignmentWithAnIllegalArgumentException() {
        val account = Account()

        assertThrows(IllegalArgumentException::class.java) {
            account.balance = -1
        }
    }
}
