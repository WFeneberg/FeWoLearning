import random

import pytest

from ex100_property_based_tests import FalsifiedError, for_all, ints, lists, shrink, shrink_int, shrink_list


def constant(value):
    def _gen(_rng: random.Random):
        return value

    return _gen


def test_ints_stay_within_the_requested_bounds():
    gen = ints(0, 10)
    rng = random.Random(1)

    values = [gen(rng) for _ in range(200)]

    assert all(0 <= v <= 10 for v in values)
    assert min(values) == 0  # with 200 draws over a small range, both ends show up
    assert max(values) == 10


def test_lists_stay_within_the_requested_max_size():
    gen = lists(ints(0, 5), max_size=4)
    rng = random.Random(2)

    values = [gen(rng) for _ in range(100)]

    assert all(len(v) <= 4 for v in values)
    assert all(0 <= item <= 5 for v in values for item in v)


def test_shrink_int_of_zero_yields_nothing():
    assert list(shrink_int(0)) == []


def test_shrink_int_includes_zero_and_stays_smaller_in_magnitude():
    candidates = list(shrink_int(37))

    assert 0 in candidates
    assert all(abs(c) < 37 for c in candidates)


def test_shrink_list_of_empty_yields_nothing():
    assert list(shrink_list([])) == []


def test_shrink_list_includes_the_empty_list():
    assert [] in list(shrink_list([1, 2, 3]))


def test_shrink_list_includes_one_element_removed_variants():
    candidates = list(shrink_list([1, 2, 3]))

    assert [2, 3] in candidates
    assert [1, 3] in candidates
    assert [1, 2] in candidates


def test_shrink_dispatches_on_type():
    assert list(shrink(5)) == list(shrink_int(5))
    assert list(shrink([1, 2])) == list(shrink_list([1, 2]))
    assert list(shrink("unsupported")) == []


def test_for_all_passes_silently_when_the_property_always_holds():
    assert for_all(ints(0, 10), lambda x: x >= 0) is None


def test_for_all_raises_falsified_error_and_shrinks_to_the_exact_boundary():
    with pytest.raises(FalsifiedError) as exc_info:
        for_all(constant(37), lambda x: x < 5)

    assert exc_info.value.original == 37
    assert exc_info.value.shrunk == 5


def test_for_all_is_reproducible_given_the_same_seed():
    def run():
        with pytest.raises(FalsifiedError) as exc_info:
            for_all(ints(0, 1000), lambda x: x < 500, seed=42)
        return exc_info.value.shrunk

    assert run() == run()


def test_for_all_shrinks_a_list_counterexample_toward_empty():
    with pytest.raises(FalsifiedError) as exc_info:
        for_all(constant([1, 2, 3, 4, 5, 6, 7, 8]), lambda xs: len(xs) < 3)

    assert len(exc_info.value.shrunk) == 3
