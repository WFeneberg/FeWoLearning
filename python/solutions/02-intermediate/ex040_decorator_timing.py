"""Exercise 040 — Decorators (reference solution)."""

import functools
import time
from typing import Any, Callable

Func = Callable[..., Any]


def count_calls(func: Func) -> Func:
    @functools.wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        # Increment before the call, so a raising call is still counted.
        wrapper.calls += 1  # type: ignore[attr-defined]
        return func(*args, **kwargs)

    wrapper.calls = 0  # type: ignore[attr-defined]
    return wrapper


def record_args(func: Func) -> Func:
    @functools.wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        wrapper.history.append((args, dict(kwargs)))  # type: ignore[attr-defined]
        return func(*args, **kwargs)

    wrapper.history = []  # type: ignore[attr-defined]
    return wrapper


def measure(func: Func) -> Func:
    @functools.wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        started = time.perf_counter()
        try:
            return func(*args, **kwargs)
        finally:
            # finally runs on both paths, so a failing call is timed too and the
            # exception still propagates.
            wrapper.durations.append(time.perf_counter() - started)  # type: ignore[attr-defined]

    wrapper.durations = []  # type: ignore[attr-defined]
    return wrapper


def cache_result(func: Func) -> Func:
    cache: dict[tuple[Any, ...], Any] = {}

    @functools.wraps(func)
    def wrapper(*args: Any, **kwargs: Any) -> Any:
        if kwargs:
            # Keyword arguments are not part of the key, so caching them would risk
            # returning a value computed with different keywords.
            return func(*args, **kwargs)
        if args in cache:
            wrapper.hits += 1  # type: ignore[attr-defined]
            return cache[args]
        result = func(*args)
        cache[args] = result
        return result

    wrapper.hits = 0  # type: ignore[attr-defined]
    return wrapper


def default_on_error(default: Any) -> Callable[[Func], Func]:
    # The outer call captures the configuration; the middle one is the decorator.
    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            try:
                return func(*args, **kwargs)
            except Exception:
                # Exception, not BaseException: KeyboardInterrupt and SystemExit
                # must not be turned into a return value.
                return default

        return wrapper

    return decorator
