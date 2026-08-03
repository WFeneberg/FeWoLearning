import pytest

from ex002_string_formatting import align_columns, format_percent, format_price, thousands


@pytest.mark.parametrize(
    "amount, expected",
    [(3.5, "3.50 EUR"), (0, "0.00 EUR"), (2.345, "2.35 EUR"), (1234.5, "1234.50 EUR")],
)
def test_format_price_default_currency(amount: float, expected: str) -> None:
    assert format_price(amount) == expected


def test_format_price_custom_currency() -> None:
    assert format_price(9.9, "CHF") == "9.90 CHF"


@pytest.mark.parametrize(
    "fraction, decimals, expected",
    [(0.1234, 1, "12.3%"), (0.5, 0, "50%"), (1.0, 2, "100.00%"), (0.0, 1, "0.0%")],
)
def test_format_percent(fraction: float, decimals: int, expected: str) -> None:
    assert format_percent(fraction, decimals) == expected


def test_format_percent_default_is_one_decimal() -> None:
    assert format_percent(0.256) == "25.6%"


def test_align_columns_pads_name_and_right_aligns_number() -> None:
    assert align_columns([("ab", 7)], 4) == ["ab      7"]


def test_align_columns_multiple_rows() -> None:
    rows = [("alpha", 1), ("b", 42), ("ccc", 12345)]
    assert align_columns(rows, 6) == [
        "alpha     1",
        "b        42",
        "ccc   12345",
    ]


def test_align_columns_does_not_truncate_long_names() -> None:
    assert align_columns([("verylongname", 3)], 4) == ["verylongname    3"]


@pytest.mark.parametrize(
    "value, expected",
    [(1234567, "1,234,567"), (999, "999"), (1000, "1,000"), (0, "0"), (-4500, "-4,500")],
)
def test_thousands(value: int, expected: str) -> None:
    assert thousands(value) == expected
