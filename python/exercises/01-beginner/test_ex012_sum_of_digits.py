import pytest

from ex012_sum_of_digits import (
    count_digits,
    digital_root,
    digits,
    reverse_number,
    sum_of_digits,
    to_base,
)


@pytest.mark.parametrize(
    "number, expected",
    [(123, 6), (-123, 6), (0, 0), (9, 9), (1000, 1)],
)
def test_sum_of_digits(number: int, expected: int) -> None:
    assert sum_of_digits(number) == expected


@pytest.mark.parametrize(
    "number, expected",
    [(123, [1, 2, 3]), (0, [0]), (-105, [1, 0, 5]), (7, [7])],
)
def test_digits(number: int, expected: list[int]) -> None:
    assert digits(number) == expected


@pytest.mark.parametrize(
    "number, expected",
    [(9875, 2), (0, 0), (9, 9), (10, 1), (-9875, 2), (99, 9)],
)
def test_digital_root(number: int, expected: int) -> None:
    assert digital_root(number) == expected


@pytest.mark.parametrize(
    "number, expected",
    [(0, 1), (9, 1), (10, 2), (-1234, 4), (100000, 6)],
)
def test_count_digits(number: int, expected: int) -> None:
    assert count_digits(number) == expected


@pytest.mark.parametrize(
    "number, expected",
    [(123, 321), (-1230, -321), (0, 0), (5, 5), (100, 1)],
)
def test_reverse_number(number: int, expected: int) -> None:
    assert reverse_number(number) == expected


@pytest.mark.parametrize(
    "number, base, expected",
    [
        (255, 16, "ff"),
        (0, 2, "0"),
        (5, 2, "101"),
        (10, 10, "10"),
        (4095, 16, "fff"),
        (7, 8, "7"),
    ],
)
def test_to_base(number: int, base: int, expected: str) -> None:
    assert to_base(number, base) == expected


@pytest.mark.parametrize("number, base", [(-1, 2), (10, 1), (10, 17), (10, 0)])
def test_to_base_rejects_bad_input(number: int, base: int) -> None:
    with pytest.raises(ValueError):
        to_base(number, base)
