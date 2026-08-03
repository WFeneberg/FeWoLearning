import asyncio

import pytest

from ex073_asyncio_timeout_cancel import (
    cancel_after_first,
    cleanup_on_cancel,
    run,
    run_and_cancel,
    shielded,
    with_timeout,
    with_timeout_default,
)


async def quick(value: str = "quick") -> str:
    await asyncio.sleep(0)
    return value


async def slow(value: str = "slow") -> str:
    await asyncio.sleep(3600)
    return value


def test_run_executes_a_coroutine() -> None:
    assert run(quick("x")) == "x"


def test_with_timeout_returns_the_value_when_fast_enough() -> None:
    assert run(with_timeout(quick(), 10)) == "quick"


def test_with_timeout_returns_none_on_expiry() -> None:
    assert run(with_timeout(slow(), 0.01)) is None


def test_with_timeout_default_returns_the_value() -> None:
    assert run(with_timeout_default(quick(), 10, "fallback")) == "quick"


def test_with_timeout_default_returns_the_default_on_expiry() -> None:
    assert run(with_timeout_default(slow(), 0.01, "fallback")) == "fallback"


def test_cancel_after_first_returns_the_quick_result() -> None:
    assert run(cancel_after_first(slow, quick)) == "quick"


def test_cancel_after_first_does_not_hang() -> None:
    # The slow coroutine sleeps for an hour; finishing at all proves it was cancelled.
    assert run(cancel_after_first(slow, quick)) == "quick"


def test_run_and_cancel_reports_cancellation() -> None:
    log: list[str] = []

    assert run(run_and_cancel(log)) == "cancelled"


def test_run_and_cancel_runs_the_cleanup() -> None:
    log: list[str] = []

    run(run_and_cancel(log))

    assert log == ["start", "cleanup"]


def test_cleanup_on_cancel_reraises_cancelled_error() -> None:
    async def scenario() -> bool:
        log: list[str] = []
        task = asyncio.create_task(cleanup_on_cancel(log))
        await asyncio.sleep(0)
        task.cancel()
        try:
            await task
        except asyncio.CancelledError:
            # Swallowing CancelledError inside the coroutine would make this
            # unreachable, and the task would look like it completed normally.
            return True
        return False

    assert run(scenario()) is True


def test_cleanup_on_cancel_marks_the_task_cancelled() -> None:
    async def scenario() -> bool:
        log: list[str] = []
        task = asyncio.create_task(cleanup_on_cancel(log))
        await asyncio.sleep(0)
        task.cancel()
        with pytest.raises(asyncio.CancelledError):
            await task
        return task.cancelled()

    assert run(scenario()) is True


def test_shielded_completes_the_inner_work() -> None:
    log: list[str] = []

    assert run(shielded(log)) == "inner-done"


def test_shielded_logged_the_inner_completion() -> None:
    log: list[str] = []

    run(shielded(log))

    # The timeout cancelled the *wait*, not the shielded coroutine.
    assert log == ["inner-done"]
