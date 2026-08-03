"""Exercise 057 — functools.partial (intermediate).

Goal:   Freeze some arguments now and supply the rest later.
Drills: partial with positional vs keyword arguments, why frozen positionals cannot
        be overridden but frozen keywords can, partial as a cheap alternative to a
        lambda, partialmethod, inspecting .func/.args/.keywords.
Passes: when `pytest exercises/02-intermediate/test_ex057_functools_partial.py` is green.
"""

from typing import Any, Callable


def make_multiplier(factor: int) -> Callable[[int], int]:
    """Return a function multiplying its argument by `factor`, built with partial.

    ``operator.mul`` plus a frozen first argument does it without a lambda.
    """
    raise NotImplementedError


def make_power(exponent: int) -> Callable[[int], int]:
    """Return a function raising its argument to `exponent`.

    Note the argument order: ``pow(base, exp)`` takes the base first, so the exponent
    cannot simply be frozen positionally — freeze it by keyword or wrap it.
    """
    raise NotImplementedError


def bind_greeting(greeting: str) -> Callable[..., str]:
    """Freeze the `greeting` of ``f"{greeting}, {name}!"`` by keyword.

    A **keyword** frozen by partial can still be overridden at the call site, unlike a
    frozen positional. The returned callable must therefore accept a new `greeting`.
    """
    raise NotImplementedError


def with_defaults(func: Callable[..., Any], **defaults: Any) -> Callable[..., Any]:
    """Return `func` with `defaults` pre-applied as overridable keywords."""
    raise NotImplementedError


def describe(bound: Any) -> tuple[str, tuple[Any, ...], dict[str, Any]]:
    """Return ``(wrapped_function_name, frozen_positionals, frozen_keywords)``.

    A partial exposes exactly this via ``.func``, ``.args`` and ``.keywords``, which is
    why it introspects better than a lambda.
    """
    raise NotImplementedError


class Formatter:
    """A formatter with partialmethod-based shortcuts.

    ``render(text, prefix, suffix)`` wraps the text; `bracket` and `quote` are
    partialmethods that freeze the wrapping characters.
    """

    def render(self, text: str, prefix: str = "", suffix: str = "") -> str:
        """Return ``prefix + text + suffix``."""
        raise NotImplementedError

    # TODO: define `bracket` and `quote` as functools.partialmethod over render,
    # freezing ("[", "]") and ('"', '"') respectively.


def compose_partials(func: Callable[..., int], *frozen: int) -> Callable[..., int]:
    """Freeze `frozen` as the leading positional arguments of `func`.

    Frozen positionals are prepended, so a later call's arguments follow them — they
    cannot replace them.
    """
    raise NotImplementedError
