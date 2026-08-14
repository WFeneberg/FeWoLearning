package fewolearning.exercises.intermediate.ex052_delegated_map_backed

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class UserProfileTest {

    @Test
    fun propertiesAreReadFromTheBackingMap() {
        val profile = UserProfile(mutableMapOf("name" to "Ada", "age" to 36))

        assertEquals("Ada", profile.name)
        assertEquals(36, profile.age)
    }

    @Test
    fun propertiesReflectLaterMutationsOfTheBackingMap() {
        val source = mutableMapOf<String, Any?>("name" to "Ada", "age" to 36)
        val profile = UserProfile(source)

        source["name"] = "Grace"

        assertEquals("Grace", profile.name)
    }
}
