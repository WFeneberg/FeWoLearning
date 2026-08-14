package fewolearning.exercises.beginner.ex015_local_date_basics;

import org.junit.jupiter.api.Test;

import java.time.LocalDate;

import static org.junit.jupiter.api.Assertions.assertEquals;

class LocalDateBasicsTest {

    @Test
    void parseIsoDateParsesAStandardIsoString() {
        assertEquals(LocalDate.of(2026, 8, 13), LocalDateBasics.parseIsoDate("2026-08-13"));
    }

    @Test
    void daysBetweenComputesTheNumberOfDaysForward() {
        LocalDate start = LocalDate.of(2026, 1, 1);
        LocalDate end = LocalDate.of(2026, 1, 31);

        assertEquals(30, LocalDateBasics.daysBetween(start, end));
    }

    @Test
    void daysBetweenIsNegativeWhenEndIsBeforeStart() {
        LocalDate start = LocalDate.of(2026, 1, 31);
        LocalDate end = LocalDate.of(2026, 1, 1);

        assertEquals(-30, LocalDateBasics.daysBetween(start, end));
    }
}
