package fewolearning.exercises.beginner.ex011_extension_function

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertTrue
import org.junit.jupiter.api.Assertions.assertFalse

class ExtensionFunctionTest {

    @Test
    fun isPalindromeIsTrueForASimplePalindrome() {
        assertTrue("level".isPalindrome())
    }

    @Test
    fun isPalindromeIgnoresCase() {
        assertTrue("Level".isPalindrome())
    }

    @Test
    fun isPalindromeIgnoresSpacesAndPunctuation() {
        assertTrue("A man a plan a canal Panama".isPalindrome())
    }

    @Test
    fun isPalindromeIsFalseForNonPalindromes() {
        assertFalse("hello".isPalindrome())
    }
}
