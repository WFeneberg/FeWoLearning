"""Exercise 087 — functools.singledispatch (advanced).

Goal:   Replace a chain of `isinstance` checks with type-based dispatch: one
        function per type, all sharing one call-site.
Drills: `@singledispatch`, `@<func>.register` (decorating a function with the type
        taken from its own annotation), dispatch picking the *most specific*
        registered type along the MRO, and `singledispatchmethod` for doing the
        same thing to an instance method.
Passes: when `pytest exercises/03-advanced/test_ex087_singledispatch_dispatch.py` is green.

Note:   `bool` is a subclass of `int`. Register a description for `int` but not for
        `bool`, and a `True` argument dispatches to the `int` implementation —
        singledispatch walks the MRO to find the closest registered ancestor. That
        is exactly why `describe_bool` below needs its own registration, even
        though "an int implementation already exists" sounds like it should cover it.
"""

from functools import singledispatch, singledispatchmethod


@singledispatch
def describe(value: object) -> str:
    """Fallback for any type without a more specific registration."""
    return f"a {type(value).__name__}"


# TODO: decorate this with `@describe.register` (the type comes from the annotation)
# and implement it to return "yes" for True, "no" for False.
def describe_bool(value: bool) -> str:
    raise NotImplementedError


# TODO: decorate with `@describe.register` and return f"the integer {value}".
def describe_int(value: int) -> str:
    raise NotImplementedError


# TODO: decorate with `@describe.register` and return f"the float {value}".
def describe_float(value: float) -> str:
    raise NotImplementedError


# TODO: decorate with `@describe.register` and return f'the string "{value}"'.
def describe_str(value: str) -> str:
    raise NotImplementedError


# TODO: decorate with `@describe.register` and return f"a list of {len(value)} items".
def describe_list(value: list) -> str:
    raise NotImplementedError


class Formatter:
    """Wraps the same idea in a method — one dispatcher, shared by every instance."""

    @singledispatchmethod
    def format(self, value: object) -> str:
        """Fallback: angle-bracket repr."""
        return f"<{value!r}>"

    # TODO: decorate with `@format.register` and return f"#{value}".
    def format_int(self, value: int) -> str:
        raise NotImplementedError

    # TODO: decorate with `@format.register` and return f"'{value}'".
    def format_str(self, value: str) -> str:
        raise NotImplementedError
