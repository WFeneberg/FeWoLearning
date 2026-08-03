import pytest

from ex015_args_kwargs import (
    apply_twice,
    call_with,
    collect,
    describe,
    largest,
    merge_all,
    total,
)


@pytest.mark.parametrize(
    "values, expected",
    [((1, 2, 3), 6), ((), 0), ((5,), 5), ((-1, 1), 0)],
)
def test_total(values: tuple[int, ...], expected: int) -> None:
    assert total(*values) == expected


def test_largest() -> None:
    assert largest(1, 9, 3) == 9


def test_largest_without_values_uses_the_default() -> None:
    assert largest() == 0
    assert largest(default=-1) == -1


def test_largest_default_is_keyword_only() -> None:
    # A second positional would be another value, not the default.
    assert largest(1, 2) == 2


def test_describe() -> None:
    assert describe(name="ada", role="pioneer") == "name=ada, role=pioneer"


def test_describe_preserves_insertion_order() -> None:
    assert describe(b="2", a="1") == "b=2, a=1"


def test_describe_without_arguments() -> None:
    assert describe() == ""


def test_collect_returns_a_tuple_and_a_dict() -> None:
    args, kwargs = collect(1, 2, a=3)

    assert args == (1, 2)
    assert kwargs == {"a": 3}
    assert isinstance(args, tuple)
    assert isinstance(kwargs, dict)


def test_collect_empty() -> None:
    assert collect() == ((), {})


def test_call_with_forwards_positional_arguments() -> None:
    assert call_with(max, 1, 5, 3) == 5


def test_call_with_forwards_keyword_arguments() -> None:
    def join(*parts: str, sep: str = "-") -> str:
        return sep.join(parts)

    assert call_with(join, "a", "b", sep="+") == "a+b"


def test_call_with_no_arguments() -> None:
    assert call_with(dict) == {}


def test_apply_twice() -> None:
    def add(a: int, b: int = 0) -> int:
        return a + b

    assert apply_twice(add, 3) == 6
    assert apply_twice(add, 3, b=1) == 8


def test_apply_twice_really_calls_twice() -> None:
    calls: list[int] = []

    def record(value: int) -> int:
        calls.append(value)
        return value

    apply_twice(record, 7)

    assert calls == [7, 7]


def test_merge_all() -> None:
    assert merge_all({"a": 1}, {"b": 2}) == {"a": 1, "b": 2}


def test_merge_all_later_values_win() -> None:
    assert merge_all({"a": 1}, {"a": 2}) == {"a": 2}


def test_merge_all_extra_wins_over_mappings() -> None:
    assert merge_all({"a": 1}, a=9) == {"a": 9}


def test_merge_all_empty() -> None:
    assert merge_all() == {}


def test_merge_all_does_not_modify_its_inputs() -> None:
    first = {"a": 1}
    second = {"a": 2}

    merge_all(first, second)

    assert first == {"a": 1}
    assert second == {"a": 2}
