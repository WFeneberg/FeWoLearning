package fewolearning.exercises.beginner.ex030_local_date_parsing

import java.time.LocalDate
import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class LocalDateParsingTest {

    @Test
    fun parseIsoDateParsesAnIsoFormattedString() {
        assertEquals(LocalDate.of(2024, 1, 1), parseIsoDate("2024-01-01"))
    }

    @Test
    fun daysUntilComputesTheNumberOfDaysBetweenTwoDates() {
        val start = LocalDate.of(2024, 1, 1)
        val end = LocalDate.of(2024, 1, 11)

        assertEquals(10L, daysUntil(start, end))
    }

    @Test
    fun daysUntilIsNegativeWhenEndIsBeforeStart() {
        val start = LocalDate.of(2024, 1, 11)
        val end = LocalDate.of(2024, 1, 1)

        assertEquals(-10L, daysUntil(start, end))
    }
}
