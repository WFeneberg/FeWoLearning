"""Exercise 094 — an async worker pool with retries and backpressure (reference solution)."""

import asyncio
from dataclasses import dataclass
from typing import Any, Awaitable, Callable


@dataclass
class TaskResult:
    task_id: int
    value: Any = None
    error: BaseException | None = None
    attempts: int = 0


async def run_task_queue(
    tasks: list[Callable[[], Awaitable[Any]]],
    *,
    workers: int,
    max_retries: int = 0,
    queue_size: int = 0,
) -> list[TaskResult]:
    if workers <= 0:
        raise ValueError(f"workers must be positive, got {workers}")

    queue: asyncio.Queue[tuple[int, Callable[[], Awaitable[Any]]] | None] = asyncio.Queue(
        maxsize=queue_size
    )
    results: list[TaskResult | None] = [None] * len(tasks)

    async def producer() -> None:
        for index, task in enumerate(tasks):
            await queue.put((index, task))
        for _ in range(workers):
            await queue.put(None)

    async def worker() -> None:
        while True:
            item = await queue.get()
            if item is None:
                return
            index, task = item
            attempts = 0
            last_error: BaseException | None = None
            value: Any = None
            while attempts <= max_retries:
                attempts += 1
                try:
                    value = await task()
                    last_error = None
                    break
                except Exception as exc:  # noqa: BLE001 — isolate one task's failure
                    last_error = exc
            results[index] = TaskResult(task_id=index, value=value, error=last_error, attempts=attempts)

    await asyncio.gather(producer(), *(worker() for _ in range(workers)))
    return [result for result in results if result is not None]


def run(coro: Awaitable[Any]) -> Any:
    return asyncio.run(coro)
