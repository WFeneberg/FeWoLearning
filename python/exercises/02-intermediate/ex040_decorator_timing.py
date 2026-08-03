"""Exercise 040 — Decorators (intermediate).

Goal:   Wrap a function without losing its identity or its signature.
Drills: closures over the wrapped function, functools.wraps, *args/**kwargs
        forwarding, recording calls, decorators that must not swallow exceptions.
Passes: when `pytest exercises/02-intermediate/test_ex040_decorator_timing.py` is green.
"""

from typing import Any, Callable

Func = Callable[..., Any]


def count_calls(func: Func) -> Func:
    """Count invocations, exposing the total as ``wrapper.calls``.

    The wrapper must keep the wrapped function's ``__name__`` and ``__doc__`` —
    ``functools.wraps`` does that. Counting must also happen for a call that raises.
    """
    raise NotImplementedError


def record_args(func: Func) -> Func:
    """Record every call's arguments in ``wrapper.history`` as ``(args, kwargs)``.

    Arguments are stored exactly as received, positional and keyword kept apart.
    """
    raise NotImplementedError


def measure(func: Func) -> Func:
    """Record the duration of each call in ``wrapper.durations`` (seconds, float).

    Use ``time.perf_counter``, and record the duration even when the call raises —
    that is what ``finally`` is for. Do not swallow the exception.
    """
    raise NotImplementedError


def cache_result(func: Func) -> Func:
    """Memoise on the positional arguments only.

    Keyword arguments bypass the cache entirely (they are forwarded, never used as
    part of the key). Expose the hit count as ``wrapper.hits``. A cached call must not
    invoke the wrapped function again.
    """
    raise NotImplementedError


def default_on_error(default: Any) -> Callable[[Func], Func]:
    """Build a decorator that returns `default` when the call raises Exception.

    This is a decorator *factory*: it takes configuration and returns the decorator.
    A BaseException such as KeyboardInterrupt must still propagate.
    """
    raise NotImplementedError
