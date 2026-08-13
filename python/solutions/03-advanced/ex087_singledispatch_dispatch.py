"""Exercise 087 — functools.singledispatch (reference solution)."""

from functools import singledispatch, singledispatchmethod


@singledispatch
def describe(value: object) -> str:
    return f"a {type(value).__name__}"


@describe.register
def describe_bool(value: bool) -> str:
    return "yes" if value else "no"


@describe.register
def describe_int(value: int) -> str:
    return f"the integer {value}"


@describe.register
def describe_float(value: float) -> str:
    return f"the float {value}"


@describe.register
def describe_str(value: str) -> str:
    return f'the string "{value}"'


@describe.register
def describe_list(value: list) -> str:
    return f"a list of {len(value)} items"


class Formatter:
    @singledispatchmethod
    def format(self, value: object) -> str:
        return f"<{value!r}>"

    @format.register
    def format_int(self, value: int) -> str:
        return f"#{value}"

    @format.register
    def format_str(self, value: str) -> str:
        return f"'{value}'"
