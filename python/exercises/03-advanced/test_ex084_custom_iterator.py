import pytest

from ex084_custom_iterator import Batched, CountUpTo


def test_count_up_to_basic_range():
    assert list(CountUpTo(1, 5)) == [1, 2, 3, 4, 5]


def test_count_up_to_empty_when_start_after_end():
    assert list(CountUpTo(5, 1)) == []


def test_count_up_to_single_value():
    assert list(CountUpTo(3, 3)) == [3]


def test_count_up_to_is_its_own_iterator():
    counter = CountUpTo(1, 3)
    assert iter(counter) is counter


def test_count_up_to_supports_manual_next_calls():
    counter = CountUpTo(1, 2)
    assert next(counter) == 1
    assert next(counter) == 2
    with pytest.raises(StopIteration):
        next(counter)


def test_count_up_to_stays_exhausted():
    counter = CountUpTo(1, 2)
    assert list(counter) == [1, 2]
    # A second pass over the same (already-drained) iterator yields nothing more.
    assert list(counter) == []


def test_batched_splits_evenly():
    assert list(Batched([1, 2, 3, 4], 2)) == [[1, 2], [3, 4]]


def test_batched_final_chunk_is_shorter():
    assert list(Batched([1, 2, 3, 4, 5], 2)) == [[1, 2], [3, 4], [5]]


def test_batched_of_empty_data():
    assert list(Batched([], 3)) == []


def test_batched_rejects_non_positive_size():
    with pytest.raises(ValueError):
        Batched([1, 2, 3], 0)
    with pytest.raises(ValueError):
        Batched([1, 2, 3], -1)


def test_batched_is_reusable_across_multiple_passes():
    batched = Batched([1, 2, 3], 2)
    assert list(batched) == [[1, 2], [3]]
    assert list(batched) == [[1, 2], [3]]


def test_batched_iter_returns_a_fresh_iterator_each_time():
    batched = Batched([1, 2, 3], 2)
    first = iter(batched)
    second = iter(batched)
    assert first is not second
    assert next(first) == [1, 2]
    # Advancing `first` must not disturb an independent `second` iterator.
    assert next(second) == [1, 2]
