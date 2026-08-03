import pytest

from ex005_tuple_unpacking import (
    divmod_pairs,
    first_last,
    head_tail,
    min_max,
    swap,
    unpack_record,
)


@pytest.mark.parametrize("pair, expected", [((1, 2), (2, 1)), ((0, 0), (0, 0)), ((-1, 5), (5, -1))])
def test_swap(pair: tuple[int, int], expected: tuple[int, int]) -> None:
    assert swap(pair) == expected


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], (1, [2, 3])), ([7], (7, [])), ([1, 2], (1, [2]))],
)
def test_head_tail(values: list[int], expected: tuple[int, list[int]]) -> None:
    assert head_tail(values) == expected


def test_head_tail_empty_raises() -> None:
    with pytest.raises(ValueError):
        head_tail([])


@pytest.mark.parametrize(
    "values, expected",
    [([1, 2, 3], (1, 3)), ([5], (5, 5)), ([2, 9], (2, 9))],
)
def test_first_last(values: list[int], expected: tuple[int, int]) -> None:
    assert first_last(values) == expected


def test_first_last_empty_raises() -> None:
    with pytest.raises(ValueError):
        first_last([])


@pytest.mark.parametrize(
    "values, expected",
    [([3, 1, 2], (1, 3)), ([5], (5, 5)), ([-4, 0, 4], (-4, 4))],
)
def test_min_max(values: list[int], expected: tuple[int, int]) -> None:
    assert min_max(values) == expected


def test_min_max_empty_raises() -> None:
    with pytest.raises(ValueError):
        min_max([])


def test_unpack_record() -> None:
    assert unpack_record(("p", (3, 4))) == ("p", 3, 4)


def test_divmod_pairs() -> None:
    assert divmod_pairs([10, 7, 3], 3) == [(3, 1), (2, 1), (1, 0)]


def test_divmod_pairs_empty() -> None:
    assert divmod_pairs([], 3) == []


def test_divmod_pairs_zero_divisor_raises() -> None:
    with pytest.raises(ZeroDivisionError):
        divmod_pairs([1], 0)
