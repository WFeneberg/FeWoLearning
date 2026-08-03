"""Exercise 022 — datetime arithmetic (reference solution)."""

from datetime import date, datetime, timedelta


def days_between(start: date, end: date) -> int:
    # Subtracting dates yields a timedelta; .days is signed.
    return (end - start).days


def add_days(start: date, days: int) -> date:
    return start + timedelta(days=days)


def age_in_years(born: date, today: date) -> int:
    if today < born:
        raise ValueError("age_in_years() today must not precede born")
    # The bool subtracts 1 when this year's birthday has not happened yet.
    had_birthday = (today.month, today.day) >= (born.month, born.day)
    return today.year - born.year - (0 if had_birthday else 1)


def is_weekend(day: date) -> bool:
    # weekday(): Monday is 0, Saturday 5, Sunday 6.
    return day.weekday() >= 5


def next_weekday(start: date, weekday: int) -> date:
    if not 0 <= weekday <= 6:
        raise ValueError("next_weekday() weekday must be between 0 and 6")
    # (target - current) mod 7 is 0 when they match, so map that to a full week
    # and guarantee the result is strictly after `start`.
    ahead = (weekday - start.weekday()) % 7
    return start + timedelta(days=ahead or 7)


def duration_hours(start: datetime, end: datetime) -> float:
    # total_seconds() handles partial days and negative gaps; .seconds would not,
    # since it is always the non-negative remainder after whole days.
    return (end - start).total_seconds() / 3600


def end_of_month(day: date) -> date:
    # Step into the next month on day 1, then back one day — no month-length table
    # and leap years take care of themselves.
    if day.month == 12:
        first_next = date(day.year + 1, 1, 1)
    else:
        first_next = date(day.year, day.month + 1, 1)
    return first_next - timedelta(days=1)
