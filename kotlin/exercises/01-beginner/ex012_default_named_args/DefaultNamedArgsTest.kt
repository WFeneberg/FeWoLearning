package fewolearning.exercises.beginner.ex012_default_named_args

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class DefaultNamedArgsTest {

    @Test
    fun formatGreetingUsesDefaultTitleAndPunctuation() {
        assertEquals("Hello, Ms./Mr. Smith.", formatGreeting("Smith"))
    }

    @Test
    fun formatGreetingAcceptsACustomTitle() {
        assertEquals("Hello, Dr. Smith.", formatGreeting("Smith", title = "Dr."))
    }

    @Test
    fun formatGreetingAcceptsCustomPunctuation() {
        assertEquals("Hello, Ms./Mr. Smith!", formatGreeting("Smith", punctuation = "!"))
    }

    @Test
    fun formatGreetingAcceptsAllArgumentsByName() {
        assertEquals("Hello, Dr. Smith!", formatGreeting(name = "Smith", title = "Dr.", punctuation = "!"))
    }
}
