import pytest

from ex018_input_validation import (
    average,
    parse_age,
    require_in_range,
    require_non_empty,
    require_positive,
    require_str,
)


def test_require_positive_passes_through() -> None:
    assert require_positive(5) == 5


@pytest.mark.parametrize("value", [0, -1, -100])
def test_require_positive_rejects(value: int) -> None:
    with pytest.raises(ValueError, match=rf"^value must be positive, got {value}$"):
        require_positive(value)


def test_require_positive_uses_the_given_name() -> None:
    with pytest.raises(ValueError, match=r"^count must be positive, got 0$"):
        require_positive(0, "count")


@pytest.mark.parametrize("value", [0, 5, 10])
def test_require_in_range_accepts_the_inclusive_bounds(value: int) -> None:
    assert require_in_range(value, 0, 10) == value


@pytest.mark.parametrize("value", [-1, 11])
def test_require_in_range_rejects_outside(value: int) -> None:
    with pytest.raises(ValueError, match=r"^value must be between 0 and 10, got"):
        require_in_range(value, 0, 10)


def test_require_in_range_rejects_an_inverted_range_first() -> None:
    # Checked before the value, so even a "valid" value reports the range problem.
    with pytest.raises(ValueError, match=r"^invalid range: 10 > 0$"):
        require_in_range(5, 10, 0)


def test_require_str_passes_through() -> None:
    assert require_str("hi") == "hi"


@pytest.mark.parametrize(
    "value, typename",
    [(1, "int"), (1.5, "float"), (None, "NoneType"), ([], "list")],
)
def test_require_str_rejects_other_types(value: object, typename: str) -> None:
    with pytest.raises(TypeError, match=rf"^expected str, got {typename}$"):
        require_str(value)


def test_require_non_empty_passes_through() -> None:
    values = [1]
    assert require_non_empty(values) is values


def test_require_non_empty_rejects_empty() -> None:
    with pytest.raises(ValueError, match=r"^must not be empty$"):
        require_non_empty([])


@pytest.mark.parametrize("text, expected", [("0", 0), ("42", 42), ("150", 150)])
def test_parse_age_accepts(text: str, expected: int) -> None:
    assert parse_age(text) == expected


@pytest.mark.parametrize("text", ["abc", "", "4.5"])
def test_parse_age_rejects_non_numbers(text: str) -> None:
    with pytest.raises(ValueError, match=rf"^not a number: {text}$"):
        parse_age(text)


@pytest.mark.parametrize("text", ["-1", "151", "9999"])
def test_parse_age_rejects_out_of_range(text: str) -> None:
    with pytest.raises(ValueError, match=r"^age out of range: "):
        parse_age(text)


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], 2.0), ([5], 5.0), ([1, 2], 1.5), ([-1, 1], 0.0)],
)
def test_average(values: list[float], expected: float) -> None:
    assert average(values) == pytest.approx(expected)


def test_average_rejects_an_empty_list() -> None:
    with pytest.raises(ValueError, match=r"^cannot average an empty list$"):
        average([])
