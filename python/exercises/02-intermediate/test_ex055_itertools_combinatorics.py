import pytest

from ex055_itertools_combinatorics import (
    all_pairs,
    choose,
    choose_with_repeats,
    count_choices,
    dice_sums,
    orderings,
    orderings_of_length,
    repeat_product,
)


def test_all_pairs_varies_the_first_input_slowest() -> None:
    assert all_pairs([1, 2], "ab") == [(1, "a"), (1, "b"), (2, "a"), (2, "b")]


def test_all_pairs_with_an_empty_side() -> None:
    assert all_pairs([], "ab") == []
    assert all_pairs([1], "") == []


def test_repeat_product() -> None:
    assert repeat_product("ab", 2) == [
        ("a", "a"),
        ("a", "b"),
        ("b", "a"),
        ("b", "b"),
    ]


def test_repeat_product_of_zero_is_one_empty_tuple() -> None:
    assert repeat_product("ab", 0) == [()]


def test_repeat_product_size_grows_exponentially() -> None:
    assert len(repeat_product("abc", 3)) == 27


def test_repeat_product_rejects_a_negative_length() -> None:
    with pytest.raises(ValueError):
        repeat_product("ab", -1)


def test_orderings() -> None:
    assert orderings("ab") == [("a", "b"), ("b", "a")]


def test_orderings_count() -> None:
    assert len(orderings("abcd")) == 24


def test_orderings_of_an_empty_input() -> None:
    assert orderings("") == [()]


def test_orderings_of_length() -> None:
    assert orderings_of_length("abc", 2) == [
        ("a", "b"),
        ("a", "c"),
        ("b", "a"),
        ("b", "c"),
        ("c", "a"),
        ("c", "b"),
    ]


def test_orderings_of_length_beyond_the_input_is_empty() -> None:
    assert orderings_of_length("ab", 5) == []


def test_choose_ignores_order() -> None:
    assert choose("abc", 2) == [("a", "b"), ("a", "c"), ("b", "c")]


def test_choose_everything() -> None:
    assert choose("abc", 3) == [("a", "b", "c")]


def test_choose_zero_is_one_empty_tuple() -> None:
    assert choose("abc", 0) == [()]


def test_choose_more_than_available_is_empty() -> None:
    assert choose("ab", 5) == []


def test_choose_with_repeats() -> None:
    assert choose_with_repeats("ab", 2) == [("a", "a"), ("a", "b"), ("b", "b")]


def test_choose_with_repeats_allows_more_than_the_input_size() -> None:
    assert len(choose_with_repeats("ab", 3)) == 4


@pytest.mark.parametrize(
    "total, count, expected",
    [(5, 2, 10), (5, 0, 1), (5, 5, 1), (5, 6, 0), (52, 5, 2598960)],
)
def test_count_choices(total: int, count: int, expected: int) -> None:
    assert count_choices(total, count) == expected


@pytest.mark.parametrize("total, count", [(-1, 2), (5, -1)])
def test_count_choices_rejects_negatives(total: int, count: int) -> None:
    with pytest.raises(ValueError):
        count_choices(total, count)


def test_dice_sums_two_dice() -> None:
    sums = dice_sums(2)

    assert len(sums) == 11
    assert min(sums) == 2
    assert max(sums) == 12
    assert sums[7] == 6
    assert sums[2] == 1
    assert sums[12] == 1


def test_dice_sums_totals_to_all_outcomes() -> None:
    assert sum(dice_sums(3).values()) == 6**3


def test_dice_sums_one_die() -> None:
    assert dice_sums(1) == {1: 1, 2: 1, 3: 1, 4: 1, 5: 1, 6: 1}


def test_dice_sums_custom_sides() -> None:
    sums = dice_sums(2, sides=4)

    assert min(sums) == 2
    assert max(sums) == 8
    assert sums[5] == 4


@pytest.mark.parametrize("dice, sides", [(0, 6), (-1, 6), (2, 0), (2, -3)])
def test_dice_sums_rejects_bad_input(dice: int, sides: int) -> None:
    with pytest.raises(ValueError):
        dice_sums(dice, sides)
