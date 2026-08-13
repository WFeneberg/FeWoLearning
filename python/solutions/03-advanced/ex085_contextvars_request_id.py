"""Exercise 085 — contextvars and async-safe ambient context (reference solution)."""

import asyncio
from contextlib import contextmanager
from contextvars import ContextVar
from typing import Any, Awaitable, Iterator

request_id_var: ContextVar[str | None] = ContextVar("request_id", default=None)


@contextmanager
def request_context(request_id: str) -> Iterator[None]:
    token = request_id_var.set(request_id)
    try:
        yield
    finally:
        request_id_var.reset(token)


def current_request_id() -> str | None:
    return request_id_var.get()


def tag(message: str) -> str:
    return f"[{current_request_id() or '-'}] {message}"


async def handle_request(request_id: str, delay: float) -> str:
    with request_context(request_id):
        await asyncio.sleep(delay)
        return tag("done")


async def handle_many(requests: list[tuple[str, float]]) -> list[str]:
    return list(
        await asyncio.gather(*(handle_request(request_id, delay) for request_id, delay in requests))
    )


def run(coro: Awaitable[Any]) -> Any:
    return asyncio.run(coro)
