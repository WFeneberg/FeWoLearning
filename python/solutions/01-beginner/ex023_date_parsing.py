"""Exercise 023 — Date parsing and formatting (reference solution)."""

from datetime import date, datetime

_MONTHS = (
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
)


def parse_iso(text: str) -> date:
    try:
        return date.fromisoformat(text)
    except ValueError:
        raise ValueError(f"invalid date: {text}") from None


def format_iso(day: date) -> str:
    return day.isoformat()


def format_german(day: date) -> str:
    # %d and %m are zero-padded, unlike a plain f-string of the integers.
    return day.strftime("%d.%m.%Y")


def parse_german(text: str) -> date:
    try:
        return datetime.strptime(text, "%d.%m.%Y").date()
    except ValueError:
        raise ValueError(f"invalid date: {text}") from None


def parse_any(text: str, formats: list[str]) -> date:
    for fmt in formats:
        try:
            return datetime.strptime(text, fmt).date()
        except ValueError:
            continue
    raise ValueError(f"no format matched: {text}")


def parse_timestamp(text: str) -> datetime:
    try:
        return datetime.strptime(text, "%Y-%m-%d %H:%M:%S")
    except ValueError:
        raise ValueError(f"invalid timestamp: {text}") from None


def month_name(day: date) -> str:
    # An explicit table keeps the answer identical on every machine; %B would
    # follow the process locale.
    return _MONTHS[day.month - 1]
