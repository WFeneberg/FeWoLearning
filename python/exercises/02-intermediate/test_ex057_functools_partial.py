import operator
from typing import Any

import pytest

from ex057_functools_partial import (
    Formatter,
    bind_greeting,
    compose_partials,
    describe,
    make_multiplier,
    make_power,
    with_defaults,
)


def test_make_multiplier() -> None:
    triple = make_multiplier(3)

    assert triple(4) == 12
    assert triple(0) == 0


def test_make_multiplier_instances_are_independent() -> None:
    assert make_multiplier(2)(5) == 10
    assert make_multiplier(10)(5) == 50


def test_make_power() -> None:
    square = make_power(2)

    assert square(5) == 25
    assert square(1) == 1


def test_make_power_cube() -> None:
    assert make_power(3)(2) == 8


def test_bind_greeting_uses_the_frozen_greeting() -> None:
    hello = bind_greeting("Hello")

    assert hello(name="Ada") == "Hello, Ada!"


def test_bind_greeting_allows_overriding_the_frozen_keyword() -> None:
    hello = bind_greeting("Hello")

    # A keyword frozen by partial is a default, not a lock.
    assert hello(name="Ada", greeting="Hi") == "Hi, Ada!"


def test_with_defaults_applies_them() -> None:
    def connect(host: str, port: int = 80, secure: bool = False) -> str:
        return f"{host}:{port}:{secure}"

    bound = with_defaults(connect, port=443, secure=True)

    assert bound("example.com") == "example.com:443:True"


def test_with_defaults_can_be_overridden() -> None:
    def connect(host: str, port: int = 80) -> str:
        return f"{host}:{port}"

    bound = with_defaults(connect, port=443)

    assert bound("example.com", port=8080) == "example.com:8080"


def test_with_defaults_without_defaults() -> None:
    def identity(value: int) -> int:
        return value

    assert with_defaults(identity)(5) == 5


def test_describe_reports_the_wrapped_function_and_frozen_arguments() -> None:
    import functools

    bound = functools.partial(operator.mul, 3, extra=1)  # type: ignore[call-arg]
    name, args, kwargs = describe(bound)

    assert name == "mul"
    assert args == (3,)
    assert kwargs == {"extra": 1}


def test_describe_on_a_partial_without_keywords() -> None:
    import functools

    name, args, kwargs = describe(functools.partial(operator.add, 1, 2))

    assert name == "add"
    assert args == (1, 2)
    assert kwargs == {}


def test_formatter_render() -> None:
    assert Formatter().render("x", "<", ">") == "<x>"


def test_formatter_render_defaults() -> None:
    assert Formatter().render("x") == "x"


def test_formatter_bracket() -> None:
    assert Formatter().bracket("x") == "[x]"


def test_formatter_quote() -> None:
    assert Formatter().quote("x") == '"x"'


def test_formatter_partialmethods_are_bound_per_instance() -> None:
    formatter = Formatter()

    assert formatter.bracket("a") == "[a]"
    assert formatter.quote("a") == '"a"'


def test_compose_partials_prepends_the_frozen_arguments() -> None:
    def three(a: int, b: int, c: int) -> int:
        return a * 100 + b * 10 + c

    bound = compose_partials(three, 1, 2)

    # The frozen 1 and 2 come first; the call supplies the rest.
    assert bound(3) == 123


def test_compose_partials_with_nothing_frozen() -> None:
    assert compose_partials(operator.add)(2, 3) == 5


def test_compose_partials_freezing_everything() -> None:
    assert compose_partials(operator.add, 2, 3)() == 5
