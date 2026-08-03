"""Exercise 042 — Retrying wrappers (reference solution)."""

import functools
from typing import Any, Callable

Func = Callable[..., Any]
Sleeper = Callable[[float], None]


def retry_with_backoff(
    attempts: int = 3,
    base_delay: float = 1.0,
    factor: float = 2.0,
    sleep: Sleeper | None = None,
) -> Callable[[Func], Func]:
    if attempts < 1:
        raise ValueError("retry_with_backoff() attempts must be at least 1")
    if factor < 1:
        raise ValueError("retry_with_backoff() factor must be at least 1")

    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            wrapper.delays = []  # type: ignore[attr-defined]
            last: BaseException | None = None
            for attempt in range(attempts):
                try:
                    return func(*args, **kwargs)
                except Exception as error:
                    last = error
                    # No wait after the final attempt: nothing follows it.
                    if attempt == attempts - 1:
                        break
                    delay = base_delay * (factor**attempt)
                    wrapper.delays.append(delay)  # type: ignore[attr-defined]
                    if sleep is not None:
                        sleep(delay)
            assert last is not None
            raise last

        wrapper.delays = []  # type: ignore[attr-defined]
        return wrapper

    return decorator


def retry_on(*exceptions: type[BaseException]) -> Callable[[Func], Func]:
    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            if not exceptions:
                # Nothing is retryable, so do not even loop.
                return func(*args, **kwargs)
            try:
                return func(*args, **kwargs)
            except exceptions:
                # One retry; an unlisted type never reaches this handler.
                return func(*args, **kwargs)

        return wrapper

    return decorator


def retry_until(predicate: Callable[[Any], bool], attempts: int = 3) -> Callable[[Func], Func]:
    def decorator(func: Func) -> Func:
        @functools.wraps(func)
        def wrapper(*args: Any, **kwargs: Any) -> Any:
            result = None
            for _ in range(attempts):
                result = func(*args, **kwargs)
                if predicate(result):
                    return result
            # Out of attempts: hand back the last thing obtained rather than raising.
            return result

        return wrapper

    return decorator


def fallback_chain(*funcs: Func) -> Func:
    def call(*args: Any, **kwargs: Any) -> Any:
        if not funcs:
            raise ValueError("fallback_chain() needs at least one function")
        last: BaseException | None = None
        for func in funcs:
            try:
                return func(*args, **kwargs)
            except Exception as error:
                last = error
        assert last is not None
        raise last

    return call
