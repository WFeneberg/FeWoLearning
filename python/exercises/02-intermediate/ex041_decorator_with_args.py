"""Exercise 041 — Decorator factories (intermediate).

Goal:   Write decorators that take configuration, and understand the three nested
        layers that requires.
Drills: decorator factories, closures over configuration, stacking decorators and
        the order that implies, a decorator usable both with and without arguments.
Passes: when `pytest exercises/02-intermediate/test_ex041_decorator_with_args.py` is green.
"""

from typing import Any, Callable

Func = Callable[..., Any]


def repeat(times: int) -> Callable[[Func], Func]:
    """Call the wrapped function `times` times, returning the **last** result.

    ``times`` below 1 raises ValueError at *decoration* time, not at call time — the
    factory runs when the decorator is applied.
    """
    raise NotImplementedError


def prefix_result(prefix: str) -> Callable[[Func], Func]:
    """Prepend `prefix` to the wrapped function's string result."""
    raise NotImplementedError


def clamp_result(low: int, high: int) -> Callable[[Func], Func]:
    """Clamp the numeric result into ``[low, high]``.

    ``low > high`` raises ValueError at decoration time.
    """
    raise NotImplementedError


def tag(**attributes: Any) -> Callable[[Func], Func]:
    """Attach `attributes` to the wrapped function as real attributes.

    The function's behaviour is unchanged; only metadata is added.
    """
    raise NotImplementedError


def retry(attempts: int = 3, catch: type[BaseException] = Exception) -> Callable[[Func], Func]:
    """Retry a failing call up to `attempts` times in total.

    Re-raises the last exception when every attempt fails. Only `catch` (and its
    subclasses) is retried; anything else propagates immediately. Expose the number
    of attempts actually made as ``wrapper.attempts_made``.
    An `attempts` below 1 raises ValueError at decoration time.
    """
    raise NotImplementedError


def logged(func: Func | None = None, *, label: str = "") -> Any:
    """Usable both bare (``@logged``) and configured (``@logged(label="x")``).

    Appends ``"<label or func name>:called"`` to ``wrapper.log`` on each call. The
    trick: when used bare, `func` is the function; when used with arguments, `func`
    is None and a decorator has to be returned instead.
    """
    raise NotImplementedError
