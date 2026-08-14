package fewolearning.exercises.intermediate.ex051_delegated_observable

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class ThemeSettingsTest {

    @Test
    fun changingTheThemeNameAppendsEachTransitionToTheChangeLog() {
        val settings = ThemeSettings()

        settings.themeName = "dark"
        settings.themeName = "blue"

        assertEquals(listOf("light -> dark", "dark -> blue"), settings.changeLog)
    }
}
