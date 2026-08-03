import pytest

from ex017_exception_handling import (
    first_int,
    lookup_or_raise,
    parse_pair,
    run_with_cleanup,
    safe_divide,
    safe_int,
)


@pytest.mark.parametrize(
    "text, expected",
    [("42", 42), ("-7", -7), ("  8  ", 8), ("nope", 0), ("", 0), ("3.5", 0)],
)
def test_safe_int(text: str, expected: int) -> None:
    assert safe_int(text) == expected


def test_safe_int_custom_default() -> None:
    assert safe_int("nope", -1) == -1


def test_safe_int_does_not_swallow_other_errors() -> None:
    # int(None) raises TypeError, which is not a "not a number" answer.
    with pytest.raises(TypeError):
        safe_int(None)  # type: ignore[arg-type]


@pytest.mark.parametrize(
    "a, b, expected",
    [(6, 3, 2.0), (1, 0, None), (0, 5, 0.0), (-4, 2, -2.0)],
)
def test_safe_divide(a: float, b: float, expected: float | None) -> None:
    assert safe_divide(a, b) == expected


@pytest.mark.parametrize(
    "values, expected",
    [
        (["a", "2", "3"], 2),
        (["1"], 1),
        (["a", "b"], None),
        ([], None),
    ],
)
def test_first_int(values: list[str], expected: int | None) -> None:
    assert first_int(values) == expected


@pytest.mark.parametrize(
    "text, expected",
    [("3,4", (3, 4)), ("0,0", (0, 0)), (" 1 , 2 ", (1, 2)), ("-1,-2", (-1, -2))],
)
def test_parse_pair(text: str, expected: tuple[int, int]) -> None:
    assert parse_pair(text) == expected


@pytest.mark.parametrize("text", ["", "3", "a,b", "1,2,3", "1,"])
def test_parse_pair_rejects_malformed_input(text: str) -> None:
    with pytest.raises(ValueError, match=r"^invalid pair: "):
        parse_pair(text)


def test_parse_pair_keeps_the_original_error_as_cause() -> None:
    with pytest.raises(ValueError) as info:
        parse_pair("a,b")

    assert info.value.__cause__ is not None


def test_run_with_cleanup_on_success() -> None:
    log: list[str] = []

    result = run_with_cleanup(lambda: 42, log)

    assert result == "42"
    assert log == ["ok", "cleanup"]


def test_run_with_cleanup_on_failure() -> None:
    log: list[str] = []

    def boom() -> int:
        raise RuntimeError("boom")

    with pytest.raises(RuntimeError, match="boom"):
        run_with_cleanup(boom, log)

    assert log == ["error", "cleanup"]


def test_lookup_or_raise_returns_the_value() -> None:
    assert lookup_or_raise({"a": 1}, "a") == 1


def test_lookup_or_raise_message_and_cause() -> None:
    with pytest.raises(KeyError) as info:
        lookup_or_raise({}, "missing")

    # KeyError's str() adds quotes around the message.
    assert info.value.args[0] == "unknown key: missing"
    assert isinstance(info.value.__cause__, KeyError)
