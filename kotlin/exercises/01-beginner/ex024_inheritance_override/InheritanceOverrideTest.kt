package fewolearning.exercises.beginner.ex024_inheritance_override

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class InheritanceOverrideTest {

    @Test
    fun animalDescribeReturnsTheBaseDescription() {
        assertEquals("an animal", Animal().describe())
    }

    @Test
    fun dogDescribeExtendsTheBaseDescriptionViaSuper() {
        assertEquals("an animal, specifically a dog", Dog().describe())
    }
}
