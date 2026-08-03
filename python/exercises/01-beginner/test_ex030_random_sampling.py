import random

import pytest

from ex030_random_sampling import (
    pick_many,
    pick_one,
    pick_with_repeats,
    roll_dice,
    shuffled,
    weighted_pick,
)

VALUES = ["a", "b", "c", "d", "e"]


def test_pick_one_is_reproducible() -> None:
    assert pick_one(VALUES, 42) == pick_one(VALUES, 42)


def test_pick_one_returns_a_member() -> None:
    assert pick_one(VALUES, 7) in VALUES


def test_pick_one_differs_for_some_other_seed() -> None:
    picks = {pick_one(VALUES, seed) for seed in range(20)}
    # With five options and twenty seeds, a constant answer would mean the seed is
    # being ignored.
    assert len(picks) > 1


def test_pick_one_empty_raises() -> None:
    with pytest.raises(IndexError):
        pick_one([], 1)


def test_pick_many_is_reproducible() -> None:
    assert pick_many(VALUES, 3, 42) == pick_many(VALUES, 3, 42)


def test_pick_many_returns_distinct_members() -> None:
    result = pick_many(VALUES, 3, 42)

    assert len(result) == 3
    assert len(set(result)) == 3
    assert all(value in VALUES for value in result)


def test_pick_many_zero() -> None:
    assert pick_many(VALUES, 0, 1) == []


def test_pick_many_all_of_them() -> None:
    assert sorted(pick_many(VALUES, len(VALUES), 3)) == sorted(VALUES)


def test_pick_many_too_many_raises() -> None:
    with pytest.raises(ValueError):
        pick_many(VALUES, len(VALUES) + 1, 1)


def test_pick_with_repeats_is_reproducible() -> None:
    assert pick_with_repeats(VALUES, 5, 42) == pick_with_repeats(VALUES, 5, 42)


def test_pick_with_repeats_can_repeat() -> None:
    # Ten draws from two options must repeat.
    result = pick_with_repeats(["a", "b"], 10, 1)

    assert len(result) == 10
    assert len(set(result)) <= 2


def test_shuffled_is_a_permutation() -> None:
    values = [1, 2, 3, 4, 5]

    result = shuffled(values, 42)

    assert sorted(result) == values


def test_shuffled_does_not_modify_the_input() -> None:
    values = [1, 2, 3, 4, 5]

    shuffled(values, 42)

    assert values == [1, 2, 3, 4, 5]


def test_shuffled_is_reproducible() -> None:
    assert shuffled([1, 2, 3, 4, 5], 42) == shuffled([1, 2, 3, 4, 5], 42)


def test_shuffled_actually_reorders_for_some_seed() -> None:
    values = list(range(10))
    assert any(shuffled(values, seed) != values for seed in range(10))


def test_roll_dice_range_and_count() -> None:
    result = roll_dice(20, 42)

    assert len(result) == 20
    assert all(1 <= value <= 6 for value in result)


def test_roll_dice_is_reproducible() -> None:
    assert roll_dice(10, 42) == roll_dice(10, 42)


def test_roll_dice_zero() -> None:
    assert roll_dice(0, 1) == []


def test_roll_dice_negative_raises() -> None:
    with pytest.raises(ValueError):
        roll_dice(-1, 1)


def test_weighted_pick_is_reproducible() -> None:
    options = {"a": 1, "b": 3}
    assert weighted_pick(options, 42) == weighted_pick(options, 42)


def test_weighted_pick_returns_a_key() -> None:
    assert weighted_pick({"a": 1, "b": 3}, 7) in {"a", "b"}


def test_weighted_pick_respects_the_weights() -> None:
    # "b" is nine times as likely, so over many seeds it must dominate.
    picks = [weighted_pick({"a": 1, "b": 9}, seed) for seed in range(200)]

    assert picks.count("b") > picks.count("a")


def test_weighted_pick_with_a_single_option() -> None:
    assert weighted_pick({"only": 5}, 1) == "only"


def test_weighted_pick_empty_raises() -> None:
    with pytest.raises(ValueError):
        weighted_pick({}, 1)


@pytest.mark.parametrize("weight", [0, -1])
def test_weighted_pick_rejects_non_positive_weights(weight: int) -> None:
    with pytest.raises(ValueError):
        weighted_pick({"a": weight}, 1)


def test_functions_do_not_disturb_the_global_generator() -> None:
    random.seed(1234)
    expected = [random.random() for _ in range(3)]

    random.seed(1234)
    pick_one(VALUES, 99)
    shuffled([1, 2, 3], 99)
    roll_dice(5, 99)
    actual = [random.random() for _ in range(3)]

    # A module-level random.seed(...) inside these helpers would break this.
    assert actual == expected
