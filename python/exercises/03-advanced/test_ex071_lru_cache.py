import pytest

from ex071_lru_cache import LRUCache


def test_basic_get_put() -> None:
    c: LRUCache[str, int] = LRUCache(2)
    c.put("a", 1)
    c.put("b", 2)
    assert c.get("a") == 1
    assert len(c) == 2


def test_eviction_of_lru() -> None:
    c: LRUCache[str, int] = LRUCache(2)
    c.put("a", 1)
    c.put("b", 2)
    c.get("a")           # 'a' now most-recently-used
    c.put("c", 3)        # evicts 'b'
    assert c.get("b") is None
    assert c.get("a") == 1
    assert c.get("c") == 3


def test_update_refreshes_recency() -> None:
    c: LRUCache[str, int] = LRUCache(2)
    c.put("a", 1)
    c.put("b", 2)
    c.put("a", 10)       # update refreshes 'a'
    c.put("c", 3)        # evicts 'b'
    assert c.get("a") == 10
    assert c.get("b") is None


def test_rejects_bad_capacity() -> None:
    with pytest.raises(ValueError):
        LRUCache(0)
