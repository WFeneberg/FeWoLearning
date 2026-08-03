from datetime import date, datetime

import pytest

from ex022_datetime_arithmetic import (
    add_days,
    age_in_years,
    days_between,
    duration_hours,
    end_of_month,
    is_weekend,
    next_weekday,
)


@pytest.mark.parametrize(
    "start, end, expected",
    [
        (date(2024, 1, 1), date(2024, 1, 31), 30),
        (date(2024, 1, 1), date(2024, 1, 1), 0),
        (date(2024, 1, 31), date(2024, 1, 1), -30),
        # 2024 is a leap year, so February has 29 days.
        (date(2024, 2, 1), date(2024, 3, 1), 29),
        (date(2023, 2, 1), date(2023, 3, 1), 28),
    ],
)
def test_days_between(start: date, end: date, expected: int) -> None:
    assert days_between(start, end) == expected


@pytest.mark.parametrize(
    "start, days, expected",
    [
        (date(2024, 1, 1), 1, date(2024, 1, 2)),
        (date(2024, 1, 1), 0, date(2024, 1, 1)),
        (date(2024, 1, 1), -1, date(2023, 12, 31)),
        (date(2024, 2, 28), 1, date(2024, 2, 29)),
        (date(2023, 2, 28), 1, date(2023, 3, 1)),
    ],
)
def test_add_days(start: date, days: int, expected: date) -> None:
    assert add_days(start, days) == expected


@pytest.mark.parametrize(
    "born, today, expected",
    [
        (date(2000, 6, 1), date(2024, 6, 1), 24),
        (date(2000, 6, 1), date(2024, 5, 31), 23),
        (date(2000, 6, 1), date(2024, 6, 2), 24),
        (date(2000, 1, 1), date(2000, 12, 31), 0),
        (date(2004, 2, 29), date(2024, 2, 28), 19),
        (date(2004, 2, 29), date(2024, 3, 1), 20),
    ],
)
def test_age_in_years(born: date, today: date, expected: int) -> None:
    assert age_in_years(born, today) == expected


def test_age_in_years_rejects_a_date_before_birth() -> None:
    with pytest.raises(ValueError):
        age_in_years(date(2024, 1, 1), date(2023, 1, 1))


@pytest.mark.parametrize(
    "day, expected",
    [
        (date(2024, 8, 3), True),   # Saturday
        (date(2024, 8, 4), True),   # Sunday
        (date(2024, 8, 5), False),  # Monday
        (date(2024, 8, 9), False),  # Friday
    ],
)
def test_is_weekend(day: date, expected: bool) -> None:
    assert is_weekend(day) is expected


def test_next_weekday_moves_forward() -> None:
    # 2024-08-05 is a Monday; the next Wednesday (2) is the 7th.
    assert next_weekday(date(2024, 8, 5), 2) == date(2024, 8, 7)


def test_next_weekday_wraps_into_the_following_week() -> None:
    # From Friday, the next Monday is three days later.
    assert next_weekday(date(2024, 8, 9), 0) == date(2024, 8, 12)


def test_next_weekday_never_returns_the_start_itself() -> None:
    monday = date(2024, 8, 5)
    assert next_weekday(monday, 0) == date(2024, 8, 12)


@pytest.mark.parametrize("weekday", [-1, 7, 100])
def test_next_weekday_rejects_a_bad_weekday(weekday: int) -> None:
    with pytest.raises(ValueError):
        next_weekday(date(2024, 8, 5), weekday)


@pytest.mark.parametrize(
    "start, end, expected",
    [
        (datetime(2024, 1, 1, 8), datetime(2024, 1, 1, 12), 4.0),
        (datetime(2024, 1, 1, 8), datetime(2024, 1, 1, 8, 30), 0.5),
        (datetime(2024, 1, 1, 12), datetime(2024, 1, 1, 8), -4.0),
        (datetime(2024, 1, 1), datetime(2024, 1, 2), 24.0),
        (datetime(2024, 1, 1, 8), datetime(2024, 1, 1, 8), 0.0),
    ],
)
def test_duration_hours(start: datetime, end: datetime, expected: float) -> None:
    assert duration_hours(start, end) == pytest.approx(expected)


@pytest.mark.parametrize(
    "day, expected",
    [
        (date(2024, 1, 15), date(2024, 1, 31)),
        (date(2024, 2, 1), date(2024, 2, 29)),
        (date(2023, 2, 1), date(2023, 2, 28)),
        (date(2024, 4, 30), date(2024, 4, 30)),
        (date(2024, 12, 5), date(2024, 12, 31)),
    ],
)
def test_end_of_month(day: date, expected: date) -> None:
    assert end_of_month(day) == expected
