"""Exercise 077 — ThreadPoolExecutor and as_completed (advanced).

Goal:   Run I/O-bound work on a thread pool and consume the results two ways —
        in submission order, and in completion order.
Drills: ThreadPoolExecutor as a context manager, `executor.map` vs `submit`, the
        future-to-key dictionary idiom, `as_completed`, `future.exception()`,
        `wait(return_when=FIRST_COMPLETED)`, and `as_completed(timeout=…)`.
Passes: when `pytest exercises/03-advanced/test_ex077_futures_as_completed.py` is green.

Note:   `executor.map` yields results in *input* order and re-raises the first
        exception; `as_completed` yields futures in *completion* order and hands you
        the exception as data. Which one you want is the whole point of the exercise.
"""

from typing import Any, Callable, Iterable


def map_in_order(func: Callable[[Any], Any], items: Iterable[Any], max_workers: int = 4) -> list[Any]:
    """Apply `func` to every item on a thread pool, returning results in input order.

    A `max_workers` below 1 raises ValueError.
    """
    raise NotImplementedError


def completion_order(delays: list[float]) -> list[int]:
    """Submit one sleeping task per delay and return their indices in completion order.

    Task `i` sleeps ``delays[i]`` and returns ``i``. Because the pool is wide enough
    for all of them, the result is the indices sorted by delay — which is exactly what
    `as_completed` gives you and `map` would not.
    """
    raise NotImplementedError


def results_by_key(func: Callable[[str], Any], keys: Iterable[str]) -> dict[str, Any]:
    """Run `func` once per key and return ``{key: result}``.

    A Future does not remember what it was submitted for, so keep a ``{future: key}``
    dict on the side and look the key up as each future completes.
    """
    raise NotImplementedError


def partition_results(
    func: Callable[[Any], Any], items: Iterable[Any]
) -> tuple[list[Any], list[str]]:
    """Split successes from failures instead of letting the first error win.

    Returns ``(results, errors)`` where `errors` holds ``str(exception)`` for the tasks
    that raised. Both lists are sorted, since completion order varies between runs.
    """
    raise NotImplementedError


def first_completed(delays: list[float]) -> int:
    """Return the index of the task that finishes first.

    Task `i` sleeps ``delays[i]`` and returns ``i``. Use
    ``concurrent.futures.wait(..., return_when=FIRST_COMPLETED)``.

    Cancelling the losers is best-effort: `Future.cancel` only works on futures that
    have not started running yet, and here they all have.
    """
    raise NotImplementedError


def gather_with_timeout(delays: list[float], timeout: float) -> list[int]:
    """Collect every task's index, giving the whole batch at most `timeout` seconds.

    Task `i` sleeps ``delays[i]`` and returns ``i``; results come back in completion
    order. If the batch does not finish in time, `as_completed` raises TimeoutError —
    let it propagate, but shut the pool down without waiting first so a slow task
    cannot hold the caller hostage.
    """
    raise NotImplementedError
