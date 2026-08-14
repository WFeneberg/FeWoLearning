package fewolearning.exercises.intermediate.ex059_regex_named_groups

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals
import org.junit.jupiter.api.Assertions.assertNull

class RegexNamedGroupsTest {

    @Test
    fun extractsYearMonthAndDayFromAnIsoDate() {
        assertEquals(IsoDateParts("2024", "01", "15"), extractDateParts("2024-01-15"))
    }

    @Test
    fun returnsNullForNonMatchingInput() {
        assertNull(extractDateParts("not-a-date"))
    }
}
