"""Exercise 073 — Timeouts and cancellation (reference solution)."""

import asyncio
import contextlib
from typing import Any, Awaitable, Callable


async def with_timeout(coro: Awaitable[Any], seconds: float) -> Any | None:
    try:
        # asyncio.timeout cancels the inner await and surfaces TimeoutError.
        async with asyncio.timeout(seconds):
            return await coro
    except TimeoutError:
        return None


async def with_timeout_default(coro: Awaitable[Any], seconds: float, default: Any) -> Any:
    result = await with_timeout(coro, seconds)
    return default if result is None else result


async def cancel_after_first(
    slow: Callable[[], Awaitable[Any]], quick: Callable[[], Awaitable[Any]]
) -> Any:
    slow_task = asyncio.create_task(slow())
    try:
        return await quick()
    finally:
        slow_task.cancel()
        # Awaiting the cancelled task lets it finish unwinding; without this the loop
        # would warn about a task whose exception was never retrieved.
        with contextlib.suppress(asyncio.CancelledError):
            await slow_task


async def cleanup_on_cancel(log: list[str]) -> None:
    log.append("start")
    try:
        await asyncio.Event().wait()
    finally:
        # finally runs during cancellation, and *not* catching CancelledError here is
        # what lets it propagate. Swallowing it would report a normal completion.
        log.append("cleanup")


async def run_and_cancel(log: list[str]) -> str:
    task = asyncio.create_task(cleanup_on_cancel(log))
    # One loop turn so the coroutine reaches its first await.
    await asyncio.sleep(0)
    task.cancel()
    try:
        await task
    except asyncio.CancelledError:
        return "cancelled"
    return "finished"


async def shielded(log: list[str]) -> str:
    async def inner() -> str:
        await asyncio.sleep(0.05)
        log.append("inner-done")
        return "inner-done"

    task = asyncio.create_task(inner())
    # shield gives the timeout a throwaway future to cancel, leaving `task` running.
    # The shield future itself ends up cancelled, so it must not be awaited again —
    # await the underlying task instead.
    with contextlib.suppress(TimeoutError):
        async with asyncio.timeout(0.001):
            await asyncio.shield(task)
    return await task


def run(coro: Awaitable[Any]) -> Any:
    return asyncio.run(coro)  # type: ignore[arg-type]
