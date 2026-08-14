package fewolearning.exercises.beginner.ex015_local_date_basics;

import java.time.LocalDate;
import java.time.temporal.ChronoUnit;

/*
Exercise 015 - LocalDate basics (reference solution).
*/
public final class LocalDateBasics {
    private LocalDateBasics() {
    }

    public static LocalDate parseIsoDate(String isoDate) {
        return LocalDate.parse(isoDate);
    }

    public static long daysBetween(LocalDate start, LocalDate end) {
        return ChronoUnit.DAYS.between(start, end);
    }
}
