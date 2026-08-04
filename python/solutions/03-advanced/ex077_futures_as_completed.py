"""Exercise 077 — ThreadPoolExecutor and as_completed (reference solution)."""

import concurrent.futures as cf
import time
from typing import Any, Callable, Iterable


def map_in_order(func: Callable[[Any], Any], items: Iterable[Any], max_workers: int = 4) -> list[Any]:
    if max_workers < 1:
        raise ValueError("map_in_order() max_workers must be at least 1")
    with cf.ThreadPoolExecutor(max_workers=max_workers) as pool:
        # `map` yields in input order and re-raises the first exception it hits, so the
        # list() has to happen inside the `with` for that to surface here.
        return list(pool.map(func, items))


def _sleep_then_index(delays: list[float], index: int) -> int:
    time.sleep(delays[index])
    return index


def completion_order(delays: list[float]) -> list[int]:
    with cf.ThreadPoolExecutor(max_workers=max(1, len(delays))) as pool:
        futures = [pool.submit(_sleep_then_index, delays, index) for index in range(len(delays))]
        # as_completed yields each future the moment it finishes — nothing to do with
        # the order they were submitted in.
        return [future.result() for future in cf.as_completed(futures)]


def results_by_key(func: Callable[[str], Any], keys: Iterable[str]) -> dict[str, Any]:
    keys = list(keys)
    with cf.ThreadPoolExecutor(max_workers=max(1, len(keys))) as pool:
        # A Future carries no memory of its input, so the side table is the whole idiom.
        future_to_key = {pool.submit(func, key): key for key in keys}
        return {
            future_to_key[future]: future.result() for future in cf.as_completed(future_to_key)
        }


def partition_results(
    func: Callable[[Any], Any], items: Iterable[Any]
) -> tuple[list[Any], list[str]]:
    items = list(items)
    results: list[Any] = []
    errors: list[str] = []
    with cf.ThreadPoolExecutor(max_workers=max(1, len(items))) as pool:
        futures = [pool.submit(func, item) for item in items]
        for future in cf.as_completed(futures):
            # exception() hands the failure over as a value instead of raising, which is
            # what lets one bad task not sink the batch.
            error = future.exception()
            if error is None:
                results.append(future.result())
            else:
                errors.append(str(error))
    return sorted(results), sorted(errors)


def first_completed(delays: list[float]) -> int:
    with cf.ThreadPoolExecutor(max_workers=max(1, len(delays))) as pool:
        futures = [pool.submit(_sleep_then_index, delays, index) for index in range(len(delays))]
        done, not_done = cf.wait(futures, return_when=cf.FIRST_COMPLETED)
        winner = next(iter(done)).result()
        # Best-effort only: cancel() is a no-op once a future is running, and with a pool
        # this wide they all started immediately.
        for future in not_done:
            future.cancel()
        return winner


def gather_with_timeout(delays: list[float], timeout: float) -> list[int]:
    pool = cf.ThreadPoolExecutor(max_workers=max(1, len(delays)))
    try:
        futures = [pool.submit(_sleep_then_index, delays, index) for index in range(len(delays))]
        # The timeout is on the whole batch, not per future, and it raises TimeoutError.
        return [future.result() for future in cf.as_completed(futures, timeout=timeout)]
    finally:
        # Not a `with` block: its __exit__ joins every worker, so a stuck task would make
        # the timeout meaningless.
        pool.shutdown(wait=False, cancel_futures=True)
