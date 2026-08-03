import asyncio
from typing import Any, Awaitable, Callable

import pytest

from ex072_asyncio_gather import (
    bounded_gather,
    fetch_all,
    fetch_all_sequential,
    first_error,
    gather_with_errors,
    map_concurrently,
    run,
)


def make_fetcher(value: Any, events: list[str] | None = None) -> Callable[[], Awaitable[Any]]:
    """A fetcher that yields control once, so concurrency is observable."""

    async def fetcher() -> Any:
        if events is not None:
            events.append(f"start:{value}")
        # Yielding to the loop is what lets a sibling task interleave.
        await asyncio.sleep(0)
        if events is not None:
            events.append(f"end:{value}")
        return value

    return fetcher


def make_failing(error: BaseException) -> Callable[[], Awaitable[Any]]:
    async def fetcher() -> Any:
        await asyncio.sleep(0)
        raise error

    return fetcher


def test_run_executes_a_coroutine() -> None:
    async def answer() -> int:
        return 42

    assert run(answer()) == 42


def test_fetch_all_returns_results_in_argument_order() -> None:
    fetchers = [make_fetcher(1), make_fetcher(2), make_fetcher(3)]

    assert run(fetch_all(fetchers)) == [1, 2, 3]


def test_fetch_all_runs_concurrently() -> None:
    events: list[str] = []
    fetchers = [make_fetcher("a", events), make_fetcher("b", events)]

    run(fetch_all(fetchers))

    # Both started before either finished — that is concurrency.
    assert events == ["start:a", "start:b", "end:a", "end:b"]


def test_fetch_all_empty() -> None:
    assert run(fetch_all([])) == []


def test_fetch_all_sequential_does_not_interleave() -> None:
    events: list[str] = []
    fetchers = [make_fetcher("a", events), make_fetcher("b", events)]

    run(fetch_all_sequential(fetchers))

    # Each one finished before the next began.
    assert events == ["start:a", "end:a", "start:b", "end:b"]


def test_fetch_all_sequential_results() -> None:
    assert run(fetch_all_sequential([make_fetcher(1), make_fetcher(2)])) == [1, 2]


def test_gather_with_errors_returns_exceptions_in_place() -> None:
    boom = ValueError("boom")
    fetchers = [make_fetcher(1), make_failing(boom), make_fetcher(3)]

    results = run(gather_with_errors(fetchers))

    assert results[0] == 1
    assert results[1] is boom
    assert results[2] == 3


def test_gather_with_errors_lets_the_others_finish() -> None:
    fetchers = [make_failing(ValueError()), make_fetcher("survived")]

    results = run(gather_with_errors(fetchers))

    assert results[1] == "survived"


def test_gather_with_errors_all_succeed() -> None:
    assert run(gather_with_errors([make_fetcher(1)])) == [1]


def test_first_error_returns_the_exception() -> None:
    boom = KeyError("k")

    result = run(first_error([make_fetcher(1), make_failing(boom)]))

    assert result is boom


def test_first_error_returns_none_when_all_succeed() -> None:
    assert run(first_error([make_fetcher(1), make_fetcher(2)])) is None


def test_first_error_empty() -> None:
    assert run(first_error([])) is None


def test_map_concurrently() -> None:
    async def double(n: int) -> int:
        await asyncio.sleep(0)
        return n * 2

    assert run(map_concurrently(double, [1, 2, 3])) == [2, 4, 6]


def test_map_concurrently_empty() -> None:
    async def double(n: int) -> int:
        return n * 2

    assert run(map_concurrently(double, [])) == []


def test_bounded_gather_results_in_order() -> None:
    fetchers = [make_fetcher(n) for n in range(5)]

    assert run(bounded_gather(fetchers, 2)) == [0, 1, 2, 3, 4]


def test_bounded_gather_respects_the_limit() -> None:
    running = 0
    peak = 0

    def make_tracked(value: int) -> Callable[[], Awaitable[int]]:
        async def fetcher() -> int:
            nonlocal running, peak
            running += 1
            peak = max(peak, running)
            await asyncio.sleep(0)
            running -= 1
            return value

        return fetcher

    run(bounded_gather([make_tracked(n) for n in range(6)], 2))

    assert peak == 2


def test_bounded_gather_limit_above_the_count() -> None:
    assert run(bounded_gather([make_fetcher(1), make_fetcher(2)], 10)) == [1, 2]


@pytest.mark.parametrize("limit", [0, -1])
def test_bounded_gather_rejects_a_bad_limit(limit: int) -> None:
    with pytest.raises(ValueError):
        run(bounded_gather([make_fetcher(1)], limit))
