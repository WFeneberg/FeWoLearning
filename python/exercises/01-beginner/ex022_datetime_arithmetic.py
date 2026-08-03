"""Exercise 022 — datetime arithmetic (beginner).

Goal:   Do date maths with datetime/timedelta instead of counting days by hand.
Drills: date vs datetime, timedelta, comparisons, .days vs total_seconds(),
        weekday(), month-end arithmetic.
Passes: when `pytest exercises/01-beginner/test_ex022_datetime_arithmetic.py` is green.
"""

from datetime import date, datetime, timedelta


def days_between(start: date, end: date) -> int:
    """Return how many days lie between the two dates.

    The result is negative when `end` precedes `start`; the same date yields 0.
    """
    raise NotImplementedError


def add_days(start: date, days: int) -> date:
    """Return the date `days` later. A negative count moves backwards."""
    raise NotImplementedError


def age_in_years(born: date, today: date) -> int:
    """Return completed years between the two dates.

    A birthday that has not occurred yet this year does not count — so someone born
    on 2000-06-01 is 23 on 2024-05-31 and 24 on 2024-06-01. A `today` before `born`
    raises ValueError.
    """
    raise NotImplementedError


def is_weekend(day: date) -> bool:
    """Report whether the date falls on a Saturday or Sunday."""
    raise NotImplementedError


def next_weekday(start: date, weekday: int) -> date:
    """Return the next date **after** `start` whose weekday is `weekday`.

    Monday is 0 and Sunday is 6, matching ``date.weekday()``. When `start` already
    is that weekday the result is a week later, never `start` itself. A weekday
    outside 0–6 raises ValueError.
    """
    raise NotImplementedError


def duration_hours(start: datetime, end: datetime) -> float:
    """Return the gap in hours as a float.

    Use total_seconds(), not ``.days``/``.seconds``, so partial days and negative
    gaps come out right.
    """
    raise NotImplementedError


def end_of_month(day: date) -> date:
    """Return the last day of the month `day` falls in.

    Works for February in leap and non-leap years alike, without a lookup table:
    step to the first of the next month and go back one day.
    """
    raise NotImplementedError
