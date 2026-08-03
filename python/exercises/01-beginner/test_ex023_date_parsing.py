from datetime import date, datetime

import pytest

from ex023_date_parsing import (
    format_german,
    format_iso,
    month_name,
    parse_any,
    parse_german,
    parse_iso,
    parse_timestamp,
)


@pytest.mark.parametrize(
    "text, expected",
    [
        ("2024-08-03", date(2024, 8, 3)),
        ("2024-02-29", date(2024, 2, 29)),
        ("1999-12-31", date(1999, 12, 31)),
    ],
)
def test_parse_iso(text: str, expected: date) -> None:
    assert parse_iso(text) == expected


@pytest.mark.parametrize("text", ["not a date", "2024-13-01", "2023-02-29", "", "03.08.2024"])
def test_parse_iso_rejects_bad_input(text: str) -> None:
    with pytest.raises(ValueError, match=r"^invalid date: "):
        parse_iso(text)


def test_format_iso() -> None:
    assert format_iso(date(2024, 8, 3)) == "2024-08-03"


def test_format_german_pads() -> None:
    assert format_german(date(2024, 8, 3)) == "03.08.2024"


def test_format_german_two_digit_parts() -> None:
    assert format_german(date(2024, 12, 25)) == "25.12.2024"


@pytest.mark.parametrize(
    "text, expected",
    [("03.08.2024", date(2024, 8, 3)), ("25.12.1999", date(1999, 12, 25))],
)
def test_parse_german(text: str, expected: date) -> None:
    assert parse_german(text) == expected


@pytest.mark.parametrize("text", ["2024-08-03", "32.01.2024", "", "3/8/2024"])
def test_parse_german_rejects_bad_input(text: str) -> None:
    with pytest.raises(ValueError, match=r"^invalid date: "):
        parse_german(text)


def test_parse_any_takes_the_first_matching_format() -> None:
    assert parse_any("2024-08-03", ["%Y-%m-%d", "%d.%m.%Y"]) == date(2024, 8, 3)


def test_parse_any_falls_through_to_a_later_format() -> None:
    assert parse_any("03.08.2024", ["%Y-%m-%d", "%d.%m.%Y"]) == date(2024, 8, 3)


def test_parse_any_order_decides_an_ambiguous_date() -> None:
    ambiguous = "01/02/2024"

    assert parse_any(ambiguous, ["%d/%m/%Y", "%m/%d/%Y"]) == date(2024, 2, 1)
    assert parse_any(ambiguous, ["%m/%d/%Y", "%d/%m/%Y"]) == date(2024, 1, 2)


def test_parse_any_without_a_match() -> None:
    with pytest.raises(ValueError, match=r"^no format matched: nope$"):
        parse_any("nope", ["%Y-%m-%d"])


def test_parse_any_with_no_formats() -> None:
    with pytest.raises(ValueError, match=r"^no format matched: "):
        parse_any("2024-08-03", [])


def test_parse_timestamp() -> None:
    assert parse_timestamp("2024-08-03 14:30:00") == datetime(2024, 8, 3, 14, 30, 0)


@pytest.mark.parametrize("text", ["2024-08-03", "not a timestamp", ""])
def test_parse_timestamp_rejects_bad_input(text: str) -> None:
    with pytest.raises(ValueError, match=r"^invalid timestamp: "):
        parse_timestamp(text)


@pytest.mark.parametrize(
    "day, expected",
    [
        (date(2024, 1, 1), "January"),
        (date(2024, 8, 3), "August"),
        (date(2024, 12, 31), "December"),
    ],
)
def test_month_name(day: date, expected: str) -> None:
    assert month_name(day) == expected
