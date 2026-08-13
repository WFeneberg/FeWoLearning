"""Exercise 098 — combined TTL + LRU eviction (expert).

Goal:   A cache entry can die two different ways: pushed out by capacity pressure
        (LRU), or simply outliving its time-to-live (TTL) — whichever comes first.
Drills: an `OrderedDict` for LRU order (as in `ex071`, but composed with expiry
        this time), storing each entry's absolute expiry timestamp rather than a
        remaining duration, and taking the clock as an injected callable so expiry
        is testable without a real `time.sleep`.
Passes: when `pytest exercises/04-expert/test_ex098_ttl_lru_cache.py` is green.

Note:   store *when an entry expires* (``clock() + ttl`` at insert time), not "how
        much time is left" — the latter would need updating on every tick just to
        stay meaningless until you check it again, whereas an absolute deadline is
        write-once and just gets compared against a later `clock()` reading.

Note:   expiry here is lazy — checked on `get`/`__contains__`/`put`, not proactively
        swept in the background. An entry can sit in the cache past its deadline
        until something touches it; `len()` does not scan for that.
"""

from collections import OrderedDict
from typing import Callable, Generic, TypeVar

K = TypeVar("K")
V = TypeVar("V")

_MISSING = object()


class TTLLRUCache(Generic[K, V]):
    """An LRU cache where every entry also expires `ttl` seconds after it was last
    written."""

    def __init__(self, capacity: int, ttl: float, clock: Callable[[], float]) -> None:
        """Set up storage for up to `capacity` live entries, each expiring `ttl`
        seconds (per `clock()`) after it was last `put`.

        `capacity <= 0` or `ttl <= 0` raises ValueError. `clock` is injected rather
        than defaulting to `time.monotonic` so tests can control it directly.
        """
        raise NotImplementedError

    def put(self, key: K, value: V) -> None:
        """Insert or update `key`, resetting its TTL to a fresh `clock() + ttl`.

        Updating an existing key must not count against capacity or disturb
        anything's LRU order beyond making `key` itself most-recently-used.
        Inserting a *new* key while already at capacity evicts the
        least-recently-used entry first (expired or not — capacity eviction does
        not care why an entry might also be stale).
        """
        raise NotImplementedError

    def get(self, key: K, default: V | None = None) -> V | None:
        """Return the live value for `key`, marking it most-recently-used.

        A missing key, or one whose expiry has already passed `clock()`, returns
        `default` — and in the expired case, removes the entry as a side effect
        (lazy cleanup) rather than leaving dead weight behind.
        """
        raise NotImplementedError

    def __contains__(self, key: K) -> bool:
        """Whether `key` is present and not expired. Same lazy-expiry cleanup as
        `get`, but must not otherwise change LRU order — checking membership is
        not "using" the entry the way `get` is."""
        raise NotImplementedError

    def __len__(self) -> int:
        """How many entries are currently stored — including any not-yet-touched
        expired ones `get`/`__contains__` have not cleaned up yet."""
        raise NotImplementedError
