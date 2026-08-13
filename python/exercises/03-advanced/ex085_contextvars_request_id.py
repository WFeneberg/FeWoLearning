"""Exercise 085 — contextvars and async-safe ambient context (advanced).

Goal:   Thread a request id through nested and concurrent async calls without
        passing it explicitly to every function along the way.
Drills: `contextvars.ContextVar`, `.set()`/`.get()`/`.reset()`, pairing a context
        manager with a var so it is always restored, and why `asyncio.gather`
        keeps concurrent tasks from seeing each other's value.
Passes: when `pytest exercises/03-advanced/test_ex085_contextvars_request_id.py` is green.

Note:   a plain module-level variable would fail the concurrency test below —
        every task would fight over the same slot. Each `asyncio.Task` gets its own
        *copy* of the context at creation time, so a `ContextVar` set inside one
        task is invisible to a sibling task, while still being visible to every
        nested `await` within that same task.
"""

import asyncio
from contextlib import contextmanager
from contextvars import ContextVar
from typing import Any, Awaitable, Iterator

request_id_var: ContextVar[str | None] = ContextVar("request_id", default=None)


@contextmanager
def request_context(request_id: str) -> Iterator[None]:
    """Set `request_id_var` to `request_id` for the duration of the block.

    Restore the previous value on the way out — including when the block raises.
    Use the token `.set()` returns and `.reset(token)`, not a manual `.set()` of the
    old value (that would not correctly restore an *unset* var).
    """
    raise NotImplementedError


def current_request_id() -> str | None:
    """The active request id, or None outside any `request_context`."""
    raise NotImplementedError


def tag(message: str) -> str:
    """Prefix `message` with the current request id: ``f"[{id}] {message}"``.

    Outside any request, use ``"-"`` in place of the id.
    """
    raise NotImplementedError


async def handle_request(request_id: str, delay: float) -> str:
    """Simulate handling one request under `request_id`.

    Enter `request_context(request_id)`, `await asyncio.sleep(delay)`, then return
    ``tag("done")``.
    """
    raise NotImplementedError


async def handle_many(requests: list[tuple[str, float]]) -> list[str]:
    """Handle every `(request_id, delay)` pair concurrently via `asyncio.gather`.

    Results come back in argument order — proof that even though the requests
    finish out of order (different delays), each kept its own tagged id.
    """
    raise NotImplementedError


def run(coro: Awaitable[Any]) -> Any:
    """Run `coro` to completion and return its result."""
    raise NotImplementedError
