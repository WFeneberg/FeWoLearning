package fewolearning.exercises.beginner.ex030_local_date_parsing

import java.time.LocalDate
import java.time.temporal.ChronoUnit

/*
Exercise 030 - LocalDate parsing (reference solution).
*/
fun parseIsoDate(isoDate: String): LocalDate = LocalDate.parse(isoDate)

fun daysUntil(start: LocalDate, end: LocalDate): Long = ChronoUnit.DAYS.between(start, end)
