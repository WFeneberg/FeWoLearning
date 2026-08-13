import gc

from ex088_weakref_cache import Resource, WeakCache


def test_get_or_create_calls_the_factory_only_once_while_referenced():
    cache: WeakCache[str, Resource] = WeakCache()
    calls: list[int] = []

    def factory() -> Resource:
        calls.append(1)
        return Resource("a")

    first = cache.get_or_create("a", factory)
    second = cache.get_or_create("a", factory)

    assert first is second
    assert len(calls) == 1


def test_len_and_contains_reflect_live_entries():
    cache: WeakCache[str, Resource] = WeakCache()
    assert len(cache) == 0
    assert "a" not in cache

    resource = cache.get_or_create("a", lambda: Resource("a"))
    assert len(cache) == 1
    assert "a" in cache
    del resource


def test_entry_disappears_once_the_only_strong_reference_is_dropped():
    cache: WeakCache[str, Resource] = WeakCache()
    resource = cache.get_or_create("a", lambda: Resource("a"))
    assert "a" in cache

    del resource
    gc.collect()

    assert "a" not in cache
    assert len(cache) == 0


def test_get_or_create_recreates_after_eviction():
    cache: WeakCache[str, Resource] = WeakCache()
    calls: list[int] = []

    def factory() -> Resource:
        calls.append(1)
        return Resource("a")

    first = cache.get_or_create("a", factory)
    del first
    gc.collect()

    second = cache.get_or_create("a", factory)

    assert len(calls) == 2
    assert second.name == "a"


def test_different_keys_are_independent():
    cache: WeakCache[str, Resource] = WeakCache()
    a = cache.get_or_create("a", lambda: Resource("a"))
    b = cache.get_or_create("b", lambda: Resource("b"))

    assert a is not b
    assert len(cache) == 2

    del a
    gc.collect()

    assert "a" not in cache
    assert "b" in cache
    assert len(cache) == 1
    del b
