"""Exercise 078 — ProcessPoolExecutor and CPU-bound work (advanced).

Goal:   Move CPU-bound work off a single interpreter and learn what that costs.
Drills: ProcessPoolExecutor vs ThreadPoolExecutor, why the GIL makes threads useless
        for pure Python arithmetic, `chunksize`, and the picklability rule that every
        argument, return value and callable crossing a process boundary must obey.
Passes: when `pytest exercises/03-advanced/test_ex078_process_pool_cpu_bound.py` is green.

Note:   the worker functions here are deliberately *module-level*. On Windows (and on
        macOS since 3.8) a pool starts children with `spawn`, which re-imports this
        module and looks the function up by qualified name — so a lambda, a closure or
        a method of a local class cannot be a worker.
"""

import os
from typing import Any


def count_primes_below(limit: int) -> int:
    """Count the primes strictly below `limit` by trial division.

    Deliberately unoptimised: this is the CPU-bound kernel the pool distributes.
    A negative `limit` raises ValueError.
    """
    raise NotImplementedError


def square(value: int) -> int:
    """Square a value. A module-level function, hence picklable."""
    raise NotImplementedError


def current_pid(_ignored: Any = None) -> int:
    """Return the PID of the process this runs in.

    Takes (and ignores) an argument so it can be used with `Executor.map`.
    """
    raise NotImplementedError


def primes_in_parallel(limits: list[int], max_workers: int = 2) -> list[int]:
    """Count primes for every limit on a process pool, in input order.

    A `max_workers` below 1 raises ValueError.
    """
    raise NotImplementedError


def sum_squares_chunked(numbers: list[int], max_workers: int = 2, chunk_size: int = 1) -> int:
    """Sum the squares of `numbers` on a process pool, passing `chunk_size` to `map`.

    `chunksize` batches items into a single pickled message. It changes throughput
    only — the answer must not depend on it. A `chunk_size` below 1 raises ValueError.
    """
    raise NotImplementedError


def pids_seen(task_count: int, max_workers: int = 2) -> set[int]:
    """Run `task_count` trivial tasks on a pool and return the PIDs that served them.

    The parent's own PID must never appear: unlike threads, pool workers are separate
    processes. How many distinct PIDs show up depends on scheduling, so assert bounds
    rather than an exact count.
    """
    raise NotImplementedError


def is_picklable(obj: object) -> bool:
    """Report whether `obj` survives `pickle.dumps`.

    Anything sent to a worker — arguments, return values, the callable itself — is
    pickled first, so this is the gate a value has to pass. Catch the pickling
    failures (`pickle.PicklingError`, TypeError, AttributeError) and return False.
    """
    raise NotImplementedError
