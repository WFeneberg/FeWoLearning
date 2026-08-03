"""Exercise 023 — Date parsing and formatting (beginner).

Goal:   Convert between dates and strings, and reject what does not parse.
Drills: strptime/strftime, date.fromisoformat, format directives, trying several
        formats, why "%d/%m" and "%m/%d" cannot be told apart.
Passes: when `pytest exercises/01-beginner/test_ex023_date_parsing.py` is green.
"""

from datetime import date, datetime


def parse_iso(text: str) -> date:
    """Parse ``"2024-08-03"``.

    Invalid input raises ValueError whose message starts with ``"invalid date: "``.
    """
    raise NotImplementedError


def format_iso(day: date) -> str:
    """Render as ``"YYYY-MM-DD"``."""
    raise NotImplementedError


def format_german(day: date) -> str:
    """Render as ``"DD.MM.YYYY"`` with zero padding.

    ``format_german(date(2024, 8, 3))`` -> ``"03.08.2024"``.
    """
    raise NotImplementedError


def parse_german(text: str) -> date:
    """Parse ``"03.08.2024"``.

    Invalid input raises ValueError starting with ``"invalid date: "``.
    """
    raise NotImplementedError


def parse_any(text: str, formats: list[str]) -> date:
    """Try each strptime format in order and return the first that parses.

    None matching raises ValueError ``"no format matched: <text>"``. Order matters:
    "%d/%m/%Y" and "%m/%d/%Y" both accept "01/02/2024" but mean different days, so
    the caller's order decides.
    """
    raise NotImplementedError


def parse_timestamp(text: str) -> datetime:
    """Parse ``"2024-08-03 14:30:00"`` into a datetime.

    Invalid input raises ValueError starting with ``"invalid timestamp: "``.
    """
    raise NotImplementedError


def month_name(day: date) -> str:
    """Return the full English month name.

    Use an explicit table rather than ``%B``, which follows the process locale and
    would differ between machines.
    """
    raise NotImplementedError
