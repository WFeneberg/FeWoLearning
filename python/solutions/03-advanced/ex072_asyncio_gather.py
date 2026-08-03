"""Exercise 072 — asyncio.gather (reference solution)."""

import asyncio
from typing import Any, Awaitable, Callable


async def fetch_all(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    # Calling each fetcher creates the coroutines; gather is what schedules them
    # together. Results come back in argument order, not completion order.
    return list(await asyncio.gather(*(fetcher() for fetcher in fetchers)))


async def fetch_all_sequential(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    results: list[Any] = []
    for fetcher in fetchers:
        # Awaiting inside the loop serialises the work: async does not imply
        # concurrent, only that the loop *may* switch at an await.
        results.append(await fetcher())
    return results


async def gather_with_errors(fetchers: list[Callable[[], Awaitable[Any]]]) -> list[Any]:
    # return_exceptions keeps a failure from cancelling its siblings; the exception
    # object itself lands in the results list.
    return list(
        await asyncio.gather(*(fetcher() for fetcher in fetchers), return_exceptions=True)
    )


async def first_error(fetchers: list[Callable[[], Awaitable[Any]]]) -> BaseException | None:
    try:
        await asyncio.gather(*(fetcher() for fetcher in fetchers))
    except BaseException as error:  # noqa: BLE001 - reporting it is the point
        return error
    return None


async def map_concurrently(
    func: Callable[[int], Awaitable[int]], values: list[int]
) -> list[int]:
    return list(await asyncio.gather(*(func(value) for value in values)))


async def bounded_gather(
    fetchers: list[Callable[[], Awaitable[Any]]], limit: int
) -> list[Any]:
    if limit < 1:
        raise ValueError("bounded_gather() limit must be at least 1")

    semaphore = asyncio.Semaphore(limit)

    async def guarded(fetcher: Callable[[], Awaitable[Any]]) -> Any:
        # gather itself has no concurrency limit; the semaphore supplies one while
        # still letting gather preserve result order.
        async with semaphore:
            return await fetcher()

    return list(await asyncio.gather(*(guarded(fetcher) for fetcher in fetchers)))


def run(coro: Awaitable[Any]) -> Any:
    # asyncio.run creates a fresh event loop, runs to completion and closes it.
    return asyncio.run(coro)  # type: ignore[arg-type]
