package fewolearning.exercises.intermediate.ex066_time_zone_conversion;

import java.time.ZoneOffset;
import java.time.ZonedDateTime;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class TimeZoneConversionTest {

    @Test
    void convertToPreservesTheInstantAndShiftsTheLocalTime() {
        ZonedDateTime source = ZonedDateTime.of(2024, 6, 15, 12, 0, 0, 0, ZoneOffset.ofHours(2));

        ZonedDateTime converted = TimeZoneConversion.convertTo(source, ZoneOffset.ofHours(-5));

        assertEquals(source.toInstant(), converted.toInstant());
        assertEquals(5, converted.getHour());
        assertEquals(ZoneOffset.ofHours(-5), converted.getZone());
    }

    @Test
    void hoursBetweenCountsWholeHoursWithinTheSameOffset() {
        ZonedDateTime start = ZonedDateTime.of(2024, 1, 1, 0, 0, 0, 0, ZoneOffset.ofHours(2));
        ZonedDateTime end = start.plusHours(5);

        assertEquals(5, TimeZoneConversion.hoursBetween(start, end));
    }

    @Test
    void hoursBetweenAccountsForDifferingOffsets() {
        // start's instant is 08:00 UTC (10:00 - 2h); end's instant is 13:00 UTC
        // (08:00 - (-5h)), five hours later.
        ZonedDateTime start = ZonedDateTime.of(2024, 1, 1, 10, 0, 0, 0, ZoneOffset.ofHours(2));
        ZonedDateTime end = ZonedDateTime.of(2024, 1, 1, 8, 0, 0, 0, ZoneOffset.ofHours(-5));

        assertEquals(5, TimeZoneConversion.hoursBetween(start, end));
    }
}
