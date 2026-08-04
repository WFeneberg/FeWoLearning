import os
import threading

import pytest

from ex078_process_pool_cpu_bound import (
    count_primes_below,
    current_pid,
    is_picklable,
    pids_seen,
    primes_in_parallel,
    square,
    sum_squares_chunked,
)


@pytest.mark.parametrize(
    "limit, expected",
    [(0, 0), (1, 0), (2, 0), (3, 1), (10, 4), (100, 25), (1000, 168)],
)
def test_count_primes_below(limit: int, expected: int) -> None:
    assert count_primes_below(limit) == expected


def test_count_primes_below_rejects_a_negative_limit() -> None:
    with pytest.raises(ValueError):
        count_primes_below(-1)


def test_square() -> None:
    assert square(7) == 49


def test_current_pid_in_the_parent() -> None:
    assert current_pid() == os.getpid()


def test_primes_in_parallel_matches_the_sequential_answer() -> None:
    limits = [500, 1000, 2000]

    assert primes_in_parallel(limits) == [count_primes_below(limit) for limit in limits]


def test_primes_in_parallel_keeps_input_order() -> None:
    # 2000 takes the longest but must still come first.
    assert primes_in_parallel([2000, 10, 100]) == [303, 4, 25]


def test_primes_in_parallel_on_an_empty_input() -> None:
    assert primes_in_parallel([]) == []


@pytest.mark.parametrize("max_workers", [0, -1])
def test_primes_in_parallel_rejects_a_bad_worker_count(max_workers: int) -> None:
    with pytest.raises(ValueError):
        primes_in_parallel([10], max_workers=max_workers)


@pytest.mark.parametrize("chunk_size", [1, 3, 50])
def test_sum_squares_chunked_is_independent_of_chunk_size(chunk_size: int) -> None:
    numbers = list(range(1, 21))

    assert sum_squares_chunked(numbers, chunk_size=chunk_size) == sum(n * n for n in numbers)


def test_sum_squares_chunked_on_an_empty_input() -> None:
    assert sum_squares_chunked([]) == 0


def test_sum_squares_chunked_rejects_a_bad_chunk_size() -> None:
    with pytest.raises(ValueError):
        sum_squares_chunked([1, 2], chunk_size=0)


def test_pids_seen_are_all_children() -> None:
    pids = pids_seen(6, max_workers=2)

    assert os.getpid() not in pids
    # How many of the two workers get used depends on scheduling.
    assert 1 <= len(pids) <= 2


def test_pids_seen_with_a_single_worker() -> None:
    pids = pids_seen(3, max_workers=1)

    assert len(pids) == 1
    assert os.getpid() not in pids


@pytest.mark.parametrize("obj", [42, "text", (1, 2), [1, 2], {"a": 1}, None, {1, 2}])
def test_is_picklable_accepts_plain_data(obj: object) -> None:
    assert is_picklable(obj) is True


def test_is_picklable_accepts_a_module_level_function() -> None:
    # `square` is looked up by qualified name in the child, so it survives.
    assert is_picklable(square) is True


def test_is_picklable_rejects_a_lambda() -> None:
    assert is_picklable(lambda value: value) is False


def test_is_picklable_rejects_a_local_function() -> None:
    def helper(value: int) -> int:
        return value

    assert is_picklable(helper) is False


def test_is_picklable_rejects_an_os_level_object() -> None:
    assert is_picklable(threading.Lock()) is False


def test_is_picklable_rejects_a_generator() -> None:
    assert is_picklable(n for n in range(3)) is False
