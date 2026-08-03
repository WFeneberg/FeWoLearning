import pytest

from ex042_decorator_retry import (
    fallback_chain,
    retry_on,
    retry_until,
    retry_with_backoff,
)


def test_retry_with_backoff_sleeps_between_attempts_only() -> None:
    slept: list[float] = []

    @retry_with_backoff(attempts=3, base_delay=1.0, factor=2.0, sleep=slept.append)
    def always_fails() -> None:
        raise ValueError("nope")

    with pytest.raises(ValueError):
        always_fails()

    # Three attempts means two waits: after #1 and after #2, never after the last.
    assert slept == [1.0, 2.0]
    assert always_fails.delays == [1.0, 2.0]


def test_retry_with_backoff_stops_sleeping_once_it_succeeds() -> None:
    slept: list[float] = []
    state = {"calls": 0}

    @retry_with_backoff(attempts=4, base_delay=0.5, factor=3.0, sleep=slept.append)
    def flaky() -> str:
        state["calls"] += 1
        if state["calls"] < 2:
            raise ValueError("not yet")
        return "ok"

    assert flaky() == "ok"
    assert slept == [0.5]


def test_retry_with_backoff_does_not_sleep_on_immediate_success() -> None:
    slept: list[float] = []

    @retry_with_backoff(sleep=slept.append)
    def fine() -> int:
        return 1

    assert fine() == 1
    assert slept == []


def test_retry_with_backoff_without_a_sleeper_still_retries() -> None:
    state = {"calls": 0}

    @retry_with_backoff(attempts=2)
    def flaky() -> str:
        state["calls"] += 1
        if state["calls"] < 2:
            raise ValueError("not yet")
        return "ok"

    assert flaky() == "ok"
    assert state["calls"] == 2


def test_retry_with_backoff_factor_of_one_keeps_a_constant_delay() -> None:
    slept: list[float] = []

    @retry_with_backoff(attempts=3, base_delay=2.0, factor=1.0, sleep=slept.append)
    def always_fails() -> None:
        raise ValueError

    with pytest.raises(ValueError):
        always_fails()

    assert slept == [2.0, 2.0]


@pytest.mark.parametrize("attempts, factor", [(0, 2.0), (-1, 2.0), (3, 0.5), (3, 0.0)])
def test_retry_with_backoff_rejects_bad_configuration(attempts: int, factor: float) -> None:
    with pytest.raises(ValueError):

        @retry_with_backoff(attempts=attempts, factor=factor)
        def unused() -> None:
            return None


def test_retry_on_retries_a_listed_exception() -> None:
    state = {"calls": 0}

    @retry_on(ValueError)
    def flaky() -> str:
        state["calls"] += 1
        if state["calls"] < 2:
            raise ValueError("first")
        return "ok"

    assert flaky() == "ok"
    assert state["calls"] == 2


def test_retry_on_ignores_an_unlisted_exception() -> None:
    state = {"calls": 0}

    @retry_on(ValueError)
    def wrong() -> None:
        state["calls"] += 1
        raise TypeError("different")

    with pytest.raises(TypeError):
        wrong()

    assert state["calls"] == 1


def test_retry_on_accepts_several_types() -> None:
    state = {"calls": 0}

    @retry_on(ValueError, KeyError)
    def flaky() -> str:
        state["calls"] += 1
        if state["calls"] == 1:
            raise KeyError("first")
        return "ok"

    assert flaky() == "ok"


def test_retry_on_with_no_types_does_not_retry() -> None:
    state = {"calls": 0}

    @retry_on()
    def fails() -> None:
        state["calls"] += 1
        raise ValueError

    with pytest.raises(ValueError):
        fails()

    assert state["calls"] == 1


def test_retry_until_returns_the_first_acceptable_result() -> None:
    results = iter([1, 2, 7])

    @retry_until(lambda value: value > 5, attempts=5)
    def next_value() -> int:
        return next(results)

    assert next_value() == 7


def test_retry_until_gives_up_and_returns_the_last_result() -> None:
    results = iter([1, 2, 3])

    @retry_until(lambda value: value > 100, attempts=3)
    def next_value() -> int:
        return next(results)

    assert next_value() == 3


def test_retry_until_accepts_the_first_result_immediately() -> None:
    calls: list[int] = []

    @retry_until(lambda value: True, attempts=3)
    def once() -> int:
        calls.append(1)
        return 42

    assert once() == 42
    assert len(calls) == 1


def test_fallback_chain_uses_the_first_success() -> None:
    def fails() -> str:
        raise ValueError

    def works() -> str:
        return "second"

    assert fallback_chain(fails, works)() == "second"


def test_fallback_chain_forwards_arguments() -> None:
    def fails(a: int) -> int:
        raise ValueError

    def works(a: int) -> int:
        return a * 2

    assert fallback_chain(fails, works)(21) == 42


def test_fallback_chain_reraises_the_last_error() -> None:
    def first() -> None:
        raise ValueError("first")

    def second() -> None:
        raise KeyError("second")

    with pytest.raises(KeyError):
        fallback_chain(first, second)()


def test_fallback_chain_with_no_functions() -> None:
    with pytest.raises(ValueError):
        fallback_chain()()
