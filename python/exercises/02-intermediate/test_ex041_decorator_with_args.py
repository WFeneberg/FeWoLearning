import pytest

from ex041_decorator_with_args import (
    clamp_result,
    logged,
    prefix_result,
    repeat,
    retry,
    tag,
)


def test_repeat_calls_the_function_repeatedly() -> None:
    calls: list[int] = []

    @repeat(3)
    def bump() -> int:
        calls.append(1)
        return len(calls)

    assert bump() == 3
    assert len(calls) == 3


def test_repeat_once() -> None:
    @repeat(1)
    def value() -> str:
        return "x"

    assert value() == "x"


def test_repeat_forwards_arguments() -> None:
    @repeat(2)
    def add(a: int, b: int = 0) -> int:
        return a + b

    assert add(1, b=2) == 3


@pytest.mark.parametrize("times", [0, -1])
def test_repeat_rejects_a_bad_count_at_decoration_time(times: int) -> None:
    with pytest.raises(ValueError):

        @repeat(times)
        def unused() -> None:
            return None


def test_prefix_result() -> None:
    @prefix_result(">> ")
    def greet(name: str) -> str:
        return f"hi {name}"

    assert greet("ada") == ">> hi ada"


def test_prefix_result_preserves_the_name() -> None:
    @prefix_result("x")
    def named() -> str:
        return ""

    assert named.__name__ == "named"


@pytest.mark.parametrize(
    "value, expected",
    [(5, 5), (-10, 0), (500, 100), (0, 0), (100, 100)],
)
def test_clamp_result(value: int, expected: int) -> None:
    @clamp_result(0, 100)
    def identity() -> int:
        return value

    assert identity() == expected


def test_clamp_result_rejects_an_inverted_range_at_decoration_time() -> None:
    with pytest.raises(ValueError):

        @clamp_result(10, 0)
        def unused() -> int:
            return 0


def test_tag_attaches_attributes() -> None:
    @tag(role="admin", level=3)
    def handler() -> str:
        return "ok"

    assert handler.role == "admin"
    assert handler.level == 3


def test_tag_leaves_behaviour_alone() -> None:
    @tag(x=1)
    def double(n: int) -> int:
        return n * 2

    assert double(4) == 8


def test_retry_succeeds_after_failures() -> None:
    state = {"calls": 0}

    @retry(attempts=3)
    def flaky() -> str:
        state["calls"] += 1
        if state["calls"] < 3:
            raise ValueError("not yet")
        return "ok"

    assert flaky() == "ok"
    assert state["calls"] == 3
    assert flaky.attempts_made == 3


def test_retry_reraises_after_exhausting_attempts() -> None:
    @retry(attempts=2)
    def always_fails() -> None:
        raise ValueError("nope")

    with pytest.raises(ValueError, match="nope"):
        always_fails()

    assert always_fails.attempts_made == 2


def test_retry_does_not_retry_an_unlisted_exception() -> None:
    state = {"calls": 0}

    @retry(attempts=3, catch=ValueError)
    def wrong_error() -> None:
        state["calls"] += 1
        raise TypeError("different")

    with pytest.raises(TypeError):
        wrong_error()

    assert state["calls"] == 1


def test_retry_succeeds_on_the_first_try() -> None:
    @retry()
    def fine() -> int:
        return 1

    assert fine() == 1
    assert fine.attempts_made == 1


@pytest.mark.parametrize("attempts", [0, -1])
def test_retry_rejects_a_bad_attempt_count(attempts: int) -> None:
    with pytest.raises(ValueError):

        @retry(attempts=attempts)
        def unused() -> None:
            return None


def test_logged_used_bare() -> None:
    @logged
    def action() -> str:
        return "done"

    assert action() == "done"
    assert action.log == ["action:called"]


def test_logged_used_with_a_label() -> None:
    @logged(label="custom")
    def action() -> str:
        return "done"

    action()
    action()

    assert action.log == ["custom:called", "custom:called"]


def test_logged_with_empty_parentheses() -> None:
    @logged()
    def action() -> None:
        return None

    action()

    assert action.log == ["action:called"]
