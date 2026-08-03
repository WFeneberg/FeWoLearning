"""Exercise 041 — Decorator factories (reference solution)."""

import functools
from typing import Any, Callable

Func = Callable[..., Any]


def repeat(times: int) -> Callable[[Func], Func]:
    # Validating out here means a bad configuration is caught when the decorator is
    # applied, not on the first call.
    if times < 1:
        raise ValueError("repeat() times must be at least 1")

    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            result = None
            for _ in range(times):
                result = func(*args, **kwargs)
            return result

        return wrapper

    return decorator


def prefix_result(prefix: str) -> Callable[[Func], Func]:
    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            return f"{prefix}{func(*args, **kwargs)}"

        return wrapper

    return decorator


def clamp_result(low: int, high: int) -> Callable[[Func], Func]:
    if low > high:
        raise ValueError("clamp_result() low must not exceed high")

    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            return max(low, min(func(*args, **kwargs), high))

        return wrapper

    return decorator


def tag(**attributes: Any) -> Callable[[Func], Func]:
    def decorator(func: Func) -> Func:
        for key, value in attributes.items():
            setattr(func, key, value)
        # No wrapper at all: the behaviour is untouched, only metadata is added.
        return func

    return decorator


def retry(attempts: int = 3, catch: type[BaseException] = Exception) -> Callable[[Func], Func]:
    if attempts < 1:
        raise ValueError("retry() attempts must be at least 1")

    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            wrapper.attempts_made = 0  # type: ignore[attr-defined]
            last: BaseException | None = None
            for _ in range(attempts):
                wrapper.attempts_made += 1  # type: ignore[attr-defined]
                try:
                    return func(*args, **kwargs)
                except catch as error:
                    last = error
            # Every attempt failed: surface the final error rather than inventing one.
            assert last is not None
            raise last

        wrapper.attempts_made = 0  # type: ignore[attr-defined]
        return wrapper

    return decorator


def logged(func: Func | None = None, *, label: str = "") -> Any:
    def decorator(inner: Func) -> Func:
        name = label or inner.__name__

        @functools.wraps(inner)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            wrapper.log.append(f"{name}:called")  # type: ignore[attr-defined]
            return inner(*args, **kwargs)

        wrapper.log = []  # type: ignore[attr-defined]
        return wrapper

    # Bare @logged hands the function straight in; @logged(...) leaves func as None
    # and expects a decorator back.
    if func is None:
        return decorator
    return decorator(func)
