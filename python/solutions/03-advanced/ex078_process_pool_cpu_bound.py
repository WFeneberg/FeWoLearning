"""Exercise 078 — ProcessPoolExecutor and CPU-bound work (reference solution)."""

import os
import pickle
from concurrent.futures import ProcessPoolExecutor
from typing import Any


def _is_prime(candidate: int) -> bool:
    if candidate < 2:
        return False
    if candidate < 4:
        return True
    if candidate % 2 == 0:
        return False
    divisor = 3
    while divisor * divisor <= candidate:
        if candidate % divisor == 0:
            return False
        divisor += 2
    return True


def count_primes_below(limit: int) -> int:
    if limit < 0:
        raise ValueError("count_primes_below() limit must not be negative")
    return sum(1 for candidate in range(2, limit) if _is_prime(candidate))


def square(value: int) -> int:
    return value * value


def current_pid(_ignored: Any = None) -> int:
    return os.getpid()


def primes_in_parallel(limits: list[int], max_workers: int = 2) -> list[int]:
    if max_workers < 1:
        raise ValueError("primes_in_parallel() max_workers must be at least 1")
    if not limits:
        # Skip the pool entirely: spawning workers costs far more than the work does.
        return []
    with ProcessPoolExecutor(max_workers=max_workers) as pool:
        # Like ThreadPoolExecutor.map, this yields in input order.
        return list(pool.map(count_primes_below, limits))


def sum_squares_chunked(numbers: list[int], max_workers: int = 2, chunk_size: int = 1) -> int:
    if max_workers < 1:
        raise ValueError("sum_squares_chunked() max_workers must be at least 1")
    if chunk_size < 1:
        raise ValueError("sum_squares_chunked() chunk_size must be at least 1")
    if not numbers:
        return 0
    with ProcessPoolExecutor(max_workers=max_workers) as pool:
        # chunksize batches items into one pickled message per batch. For work this
        # trivial the messaging dominates, which is exactly when a big chunksize pays.
        return sum(pool.map(square, numbers, chunksize=chunk_size))


def pids_seen(task_count: int, max_workers: int = 2) -> set[int]:
    with ProcessPoolExecutor(max_workers=max_workers) as pool:
        return set(pool.map(current_pid, range(task_count)))


def is_picklable(obj: object) -> bool:
    try:
        pickle.dumps(obj)
    except (pickle.PicklingError, TypeError, AttributeError):
        # PicklingError: no qualified name to look the object up by (lambdas, closures).
        # TypeError: an OS handle or a live generator that cannot be serialised at all.
        # AttributeError: the name exists but does not resolve back to this object.
        return False
    return True
