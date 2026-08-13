import pytest

from ex098_ttl_lru_cache import TTLLRUCache


class FakeClock:
    def __init__(self, start: float = 0.0) -> None:
        self.now = start

    def __call__(self) -> float:
        return self.now

    def advance(self, seconds: float) -> None:
        self.now += seconds


def test_capacity_must_be_positive():
    with pytest.raises(ValueError):
        TTLLRUCache(0, ttl=10, clock=FakeClock())


def test_ttl_must_be_positive():
    with pytest.raises(ValueError):
        TTLLRUCache(2, ttl=0, clock=FakeClock())


def test_put_then_get_returns_the_value():
    cache = TTLLRUCache(2, ttl=10, clock=FakeClock())
    cache.put("a", 1)

    assert cache.get("a") == 1


def test_missing_key_returns_the_default():
    cache = TTLLRUCache(2, ttl=10, clock=FakeClock())

    assert cache.get("nope") is None
    assert cache.get("nope", "fallback") == "fallback"


def test_updating_an_existing_key_does_not_grow_the_cache():
    cache = TTLLRUCache(2, ttl=10, clock=FakeClock())
    cache.put("a", 1)
    cache.put("a", 2)

    assert len(cache) == 1
    assert cache.get("a") == 2


def test_capacity_evicts_the_least_recently_used_entry():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=100, clock=clock)
    cache.put("a", 1)
    cache.put("b", 2)
    cache.get("a")  # "a" is now more recently used than "b"

    cache.put("c", 3)  # capacity 2 is full — "b" gets evicted, not "a"

    assert "a" in cache
    assert "b" not in cache
    assert "c" in cache


def test_value_is_available_before_the_ttl_elapses():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=10, clock=clock)
    cache.put("a", 1)

    clock.advance(5)

    assert cache.get("a") == 1


def test_value_expires_once_the_ttl_elapses():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=10, clock=clock)
    cache.put("a", 1)

    clock.advance(10)

    assert cache.get("a") is None


def test_contains_reflects_expiry_without_reordering_lru():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=10, clock=clock)
    cache.put("a", 1)

    clock.advance(10)

    assert "a" not in cache


def test_put_resets_the_ttl_for_that_key():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=10, clock=clock)
    cache.put("a", 1)

    clock.advance(8)
    cache.put("a", 2)  # fresh deadline: now + 10, i.e. 18
    clock.advance(8)  # total elapsed since first put: 16, but only 8 since refresh

    assert cache.get("a") == 2


def test_len_counts_entries_including_untouched_expired_ones():
    clock = FakeClock()
    cache = TTLLRUCache(2, ttl=10, clock=clock)
    cache.put("a", 1)

    clock.advance(20)

    assert len(cache) == 1  # nothing has accessed "a" to trigger lazy cleanup yet
    cache.get("a")
    assert len(cache) == 0


def test_getting_an_expired_key_removes_it_so_it_no_longer_counts_toward_capacity():
    clock = FakeClock()
    cache = TTLLRUCache(1, ttl=10, clock=clock)
    cache.put("a", 1)
    clock.advance(20)
    cache.get("a")  # lazily evicted

    cache.put("b", 2)  # capacity 1, but "a" is already gone — "b" fits without evicting anything

    assert "b" in cache
    assert len(cache) == 1
