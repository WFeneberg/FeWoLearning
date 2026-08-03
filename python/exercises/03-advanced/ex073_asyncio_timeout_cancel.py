"""Exercise 073 — Timeouts and cancellation (advanced).

Goal:   Give an await a deadline, and clean up when it is cut short.
Drills: asyncio.timeout, wait_for, CancelledError, why CancelledError must be
        re-raised rather than swallowed, cleanup in finally, shielding.
Passes: when `pytest exercises/03-advanced/test_ex073_asyncio_timeout_cancel.py` is green.
"""

import asyncio
from typing import Any, Awaitable, Callable


async def with_timeout(coro: Awaitable[Any], seconds: float) -> Any | None:
    """Await `coro`, returning None when it takes longer than `seconds`.

    ``asyncio.timeout`` cancels the inner await and raises TimeoutError; catching that
    is what turns a deadline into a None.
    """
    raise NotImplementedError


async def with_timeout_default(coro: Awaitable[Any], seconds: float, default: Any) -> Any:
    """Like `with_timeout`, but returning `default` on expiry."""
    raise NotImplementedError


async def cancel_after_first(
    slow: Callable[[], Awaitable[Any]], quick: Callable[[], Awaitable[Any]]
) -> Any:
    """Start both, return the quick result and cancel the slow task.

    A task that is cancelled and then awaited raises CancelledError, so the cancel has
    to be followed by a suppressed await — otherwise the loop complains about a task
    that was never retrieved.
    """
    raise NotImplementedError


async def cleanup_on_cancel(log: list[str]) -> None:
    """Append "start", then await forever, appending "cleanup" when cancelled.

    The cleanup belongs in ``finally``, and CancelledError **must** be re-raised: a
    coroutine that swallows it claims to have finished normally, and the framework
    cancelling it has no way to know otherwise.
    """
    raise NotImplementedError


async def run_and_cancel(log: list[str]) -> str:
    """Start `cleanup_on_cancel`, let it begin, cancel it, and report what happened.

    Returns "cancelled" when awaiting the cancelled task raised CancelledError, else
    "finished". The log must end up ``["start", "cleanup"]``.
    """
    raise NotImplementedError


async def shielded(log: list[str]) -> str:
    """Demonstrate ``asyncio.shield``: the outer wait is cancelled, the inner work is not.

    Append "inner-done" from the shielded coroutine. Wrap it in a shield, apply a
    timeout shorter than the work, catch the TimeoutError, then await the shielded
    future to completion and return its result.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run a coroutine to completion."""
    raise NotImplementedError
