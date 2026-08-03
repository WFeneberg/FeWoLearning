"""Exercise 057 — functools.partial (reference solution)."""

import functools
import operator
from typing import Any, Callable


def make_multiplier(factor: int) -> Callable[[int], int]:
    # Multiplication is commutative, so freezing the first positional is fine.
    return functools.partial(operator.mul, factor)


def _power(exponent: int, base: int) -> int:
    return base**exponent


def make_power(exponent: int) -> Callable[[int], int]:
    # pow() takes the base first, so the exponent cannot be frozen positionally.
    # Swapping the parameter order in a helper makes partial applicable again.
    return functools.partial(_power, exponent)


def _greet(name: str, greeting: str = "Hello") -> str:
    return f"{greeting}, {name}!"


def bind_greeting(greeting: str) -> Callable[..., str]:
    # Frozen as a *keyword*, so a call site can still override it. A frozen
    # positional could not be replaced, only added to.
    return functools.partial(_greet, greeting=greeting)


def with_defaults(func: Callable[..., Any], **defaults: Any) -> Callable[..., Any]:
    return functools.partial(func, **defaults)


def describe(bound: Any) -> tuple[str, tuple[Any, ...], dict[str, Any]]:
    # A partial keeps its parts inspectable; a lambda would expose nothing.
    return bound.func.__name__, bound.args, dict(bound.keywords)


class Formatter:
    def render(self, text: str, prefix: str = "", suffix: str = "") -> str:
        return f"{prefix}{text}{suffix}"

    # partialmethod rather than partial: it goes through the descriptor protocol, so
    # `self` is still bound correctly on each instance.
    bracket = functools.partialmethod(render, prefix="[", suffix="]")
    quote = functools.partialmethod(render, prefix='"', suffix='"')


def compose_partials(func: Callable[..., int], *frozen: int) -> Callable[..., int]:
    return functools.partial(func, *frozen)
