package fewolearning.exercises.beginner.ex022_companion_factory

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CompanionFactoryTest {

    @Test
    fun ofCreatesAUserWithTheGivenNameAndEmail() {
        val user = User.of("Alice", "alice@example.com")

        assertEquals("Alice", user.name)
        assertEquals("alice@example.com", user.email)
    }
}
