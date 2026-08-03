import pytest

from ex040_decorator_timing import (
    cache_result,
    count_calls,
    default_on_error,
    measure,
    record_args,
)


# NotImplementedError subclasses RuntimeError, so a test expecting RuntimeError
# would be satisfied by an unimplemented stub. This type cannot be confused with it.
class Boom(Exception):
    pass


def test_count_calls_counts() -> None:
    @count_calls
    def add(a: int, b: int) -> int:
        return a + b

    assert add(1, 2) == 3
    assert add(3, 4) == 7
    assert add.calls == 2


def test_count_calls_starts_at_zero() -> None:
    @count_calls
    def noop() -> None:
        return None

    assert noop.calls == 0


def test_count_calls_preserves_identity() -> None:
    @count_calls
    def documented() -> None:
        """The original docstring."""

    assert documented.__name__ == "documented"
    assert documented.__doc__ == "The original docstring."


def test_count_calls_counts_a_failing_call_too() -> None:
    @count_calls
    def boom() -> None:
        raise Boom("boom")

    with pytest.raises(Boom):
        boom()

    assert boom.calls == 1


def test_record_args_keeps_positional_and_keyword_apart() -> None:
    @record_args
    def f(a: int, b: int = 0) -> int:
        return a + b

    f(1)
    f(2, b=3)

    assert f.history == [((1,), {}), ((2,), {"b": 3})]


def test_record_args_starts_empty() -> None:
    @record_args
    def f() -> None:
        return None

    assert f.history == []


def test_measure_records_a_duration_per_call() -> None:
    @measure
    def work() -> int:
        return sum(range(1000))

    work()
    work()

    assert len(work.durations) == 2
    assert all(isinstance(d, float) and d >= 0 for d in work.durations)


def test_measure_records_even_when_the_call_raises() -> None:
    @measure
    def boom() -> None:
        raise ValueError("nope")

    with pytest.raises(ValueError):
        boom()

    assert len(boom.durations) == 1


def test_cache_result_avoids_recomputation() -> None:
    calls: list[int] = []

    @cache_result
    def square(n: int) -> int:
        calls.append(n)
        return n * n

    assert square(4) == 16
    assert square(4) == 16
    assert calls == [4]
    assert square.hits == 1


def test_cache_result_distinguishes_arguments() -> None:
    @cache_result
    def identity(n: int) -> int:
        return n

    identity(1)
    identity(2)

    assert identity.hits == 0


def test_cache_result_bypasses_the_cache_for_keyword_calls() -> None:
    calls: list[int] = []

    @cache_result
    def add(a: int, b: int = 0) -> int:
        calls.append(a)
        return a + b

    add(1, b=1)
    add(1, b=2)

    # Keyword arguments are not part of the key, so neither call was cached.
    assert calls == [1, 1]


def test_default_on_error_returns_the_default() -> None:
    @default_on_error(-1)
    def divide(a: int, b: int) -> float:
        return a / b

    assert divide(6, 3) == 2
    assert divide(1, 0) == -1


def test_default_on_error_preserves_identity() -> None:
    @default_on_error(None)
    def named() -> None:
        """Doc."""

    assert named.__name__ == "named"


def test_default_on_error_lets_base_exceptions_through() -> None:
    @default_on_error(0)
    def interrupted() -> None:
        raise KeyboardInterrupt

    with pytest.raises(KeyboardInterrupt):
        interrupted()
