"""Exercise 042 — Retrying wrappers (intermediate).

Goal:   Build retry logic that is testable — no real sleeping, no wall clock.
Drills: injecting the sleep function so backoff is observable, exception filtering,
        exhaustion behaviour, retry predicates on the *result* rather than an error.
Passes: when `pytest exercises/02-intermediate/test_ex042_decorator_retry.py` is green.
"""

from typing import Any, Callable

Func = Callable[..., Any]
Sleeper = Callable[[float], None]


def retry_with_backoff(
    attempts: int = 3,
    base_delay: float = 1.0,
    factor: float = 2.0,
    sleep: Sleeper | None = None,
) -> Callable[[Func], Func]:
    """Retry with exponentially growing delays.

    Delays are ``base_delay * factor**n`` for n = 0, 1, … and there is **no sleep
    after the final attempt** — nobody is waiting for a retry that will not happen.

    `sleep` is injected so tests can record the delays instead of waiting; when it is
    None nothing is slept at all. Expose the delays actually requested as
    ``wrapper.delays``.

    An `attempts` below 1, or a `factor` below 1, raises ValueError at decoration time.
    """
    raise NotImplementedError


def retry_on(*exceptions: type[BaseException]) -> Callable[[Func], Func]:
    """Retry (twice in total) only for the listed exception types.

    With no types given, nothing is retried. Anything unlisted propagates on the
    first failure.
    """
    raise NotImplementedError


def retry_until(predicate: Callable[[Any], bool], attempts: int = 3) -> Callable[[Func], Func]:
    """Retry while the *result* fails `predicate`.

    Returns the first result that satisfies it, or the last one obtained when the
    attempts run out — this retries on values, not on exceptions.
    """
    raise NotImplementedError


def fallback_chain(*funcs: Func) -> Func:
    """Return a callable that tries each function in turn until one does not raise.

    The result of the first success is returned. When all of them raise, the **last**
    exception propagates. With no functions, calling it raises ValueError.
    """
    raise NotImplementedError
