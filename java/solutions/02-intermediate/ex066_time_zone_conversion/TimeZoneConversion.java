package fewolearning.exercises.intermediate.ex066_time_zone_conversion;

import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoUnit;

/*
Exercise 066 - Time zone conversion (reference solution).
*/
public final class TimeZoneConversion {
    private TimeZoneConversion() {
    }

    public static ZonedDateTime convertTo(ZonedDateTime source, ZoneId targetZone) {
        return source.withZoneSameInstant(targetZone);
    }

    public static long hoursBetween(ZonedDateTime start, ZonedDateTime end) {
        return ChronoUnit.HOURS.between(start, end);
    }
}
