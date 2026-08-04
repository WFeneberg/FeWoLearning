import time

import pytest

from ex077_futures_as_completed import (
    completion_order,
    first_completed,
    gather_with_timeout,
    map_in_order,
    partition_results,
    results_by_key,
)


def double(value: int) -> int:
    return value * 2


def test_map_in_order_transforms_every_item() -> None:
    assert map_in_order(double, [1, 2, 3]) == [2, 4, 6]


def test_map_in_order_keeps_input_order_not_completion_order() -> None:
    def slow_for_small(value: int) -> int:
        # The smallest value takes the longest, so completion order is reversed.
        time.sleep(0.05 * (4 - value))
        return value

    assert map_in_order(slow_for_small, [1, 2, 3]) == [1, 2, 3]


def test_map_in_order_on_an_empty_input() -> None:
    assert map_in_order(double, []) == []


def test_map_in_order_propagates_the_first_exception() -> None:
    def boom(value: int) -> int:
        if value == 2:
            raise ValueError(f"bad value {value}")
        return value

    with pytest.raises(ValueError, match="bad value 2"):
        map_in_order(boom, [1, 2, 3])


@pytest.mark.parametrize("max_workers", [0, -1])
def test_map_in_order_rejects_a_bad_worker_count(max_workers: int) -> None:
    with pytest.raises(ValueError):
        map_in_order(double, [1], max_workers=max_workers)


def test_completion_order_is_by_duration() -> None:
    assert completion_order([0.20, 0.05, 0.12]) == [1, 2, 0]


def test_completion_order_with_a_single_task() -> None:
    assert completion_order([0.01]) == [0]


def test_results_by_key_maps_each_future_back_to_its_key() -> None:
    assert results_by_key(len, ["a", "bb", "ccc"]) == {"a": 1, "bb": 2, "ccc": 3}


def test_results_by_key_on_an_empty_input() -> None:
    assert results_by_key(len, []) == {}


def test_results_by_key_does_not_confuse_equal_results() -> None:
    # Two keys share a result; a wrong future-to-key mapping shows up here.
    assert results_by_key(len, ["ab", "cd"]) == {"ab": 2, "cd": 2}


def test_partition_results_collects_successes() -> None:
    results, errors = partition_results(double, [3, 1, 2])

    assert results == [2, 4, 6]
    assert errors == []


def test_partition_results_collects_failures_without_stopping() -> None:
    def boom(value: int) -> int:
        if value % 2 == 0:
            raise ValueError(f"even: {value}")
        return value

    results, errors = partition_results(boom, [1, 2, 3, 4])

    assert results == [1, 3]
    assert errors == ["even: 2", "even: 4"]


def test_partition_results_when_everything_fails() -> None:
    def always_boom(value: int) -> int:
        raise RuntimeError(f"no {value}")

    results, errors = partition_results(always_boom, [1, 2])

    assert results == []
    assert errors == ["no 1", "no 2"]


def test_first_completed_picks_the_shortest_task() -> None:
    assert first_completed([0.20, 0.05, 0.30]) == 1


def test_first_completed_with_a_single_task() -> None:
    assert first_completed([0.01]) == 0


def test_gather_with_timeout_returns_everything_in_time() -> None:
    assert sorted(gather_with_timeout([0.05, 0.02, 0.08], timeout=5.0)) == [0, 1, 2]


def test_gather_with_timeout_yields_in_completion_order() -> None:
    assert gather_with_timeout([0.18, 0.04, 0.11], timeout=5.0) == [1, 2, 0]


def test_gather_with_timeout_raises_when_the_batch_is_too_slow() -> None:
    with pytest.raises(TimeoutError):
        gather_with_timeout([0.02, 0.60], timeout=0.15)
