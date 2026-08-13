import pytest

from ex086_heapq_priority_queue import PriorityQueue


def test_pops_in_priority_order():
    pq: PriorityQueue[str] = PriorityQueue()
    pq.push("low", 5)
    pq.push("high", 1)
    pq.push("mid", 3)

    assert pq.pop() == "high"
    assert pq.pop() == "mid"
    assert pq.pop() == "low"


def test_equal_priorities_come_out_in_fifo_order():
    pq: PriorityQueue[str] = PriorityQueue()
    pq.push("first", 1)
    pq.push("second", 1)
    pq.push("third", 1)

    assert [pq.pop(), pq.pop(), pq.pop()] == ["first", "second", "third"]


def test_peek_does_not_remove():
    pq: PriorityQueue[str] = PriorityQueue()
    pq.push("only", 1)

    assert pq.peek() == "only"
    assert pq.peek() == "only"
    assert len(pq) == 1
    assert pq.pop() == "only"


def test_pop_from_empty_raises_key_error():
    pq: PriorityQueue[str] = PriorityQueue()
    with pytest.raises(KeyError):
        pq.pop()


def test_peek_from_empty_raises_key_error():
    pq: PriorityQueue[str] = PriorityQueue()
    with pytest.raises(KeyError):
        pq.peek()


def test_len_reflects_live_entries():
    pq: PriorityQueue[str] = PriorityQueue()
    assert len(pq) == 0
    pq.push("a", 1)
    pq.push("b", 2)
    assert len(pq) == 2
    pq.pop()
    assert len(pq) == 1


def test_pushing_an_already_queued_item_reprioritizes_it():
    pq: PriorityQueue[str] = PriorityQueue()
    pq.push("a", 5)
    pq.push("b", 1)
    pq.push("a", 0)  # a jumps to the front

    assert len(pq) == 2
    assert pq.pop() == "a"
    assert pq.pop() == "b"


def test_remove_skips_the_item_on_pop():
    pq: PriorityQueue[str] = PriorityQueue()
    pq.push("a", 1)
    pq.push("b", 2)
    pq.remove("a")

    assert len(pq) == 1
    assert pq.pop() == "b"
    with pytest.raises(KeyError):
        pq.pop()


def test_removing_an_unqueued_item_raises_key_error():
    pq: PriorityQueue[str] = PriorityQueue()
    with pytest.raises(KeyError):
        pq.remove("nope")


def test_empty_queue_has_length_zero():
    assert len(PriorityQueue()) == 0


class Task:
    """Deliberately not orderable — no `__lt__` — to prove ties never compare items."""

    __slots__ = ("name",)

    def __init__(self, name: str) -> None:
        self.name = name


def test_tie_breaking_never_compares_unorderable_items():
    pq: PriorityQueue[Task] = PriorityQueue()
    first = Task("first")
    second = Task("second")
    third = Task("third")

    pq.push(first, 1)
    pq.push(second, 1)
    pq.push(third, 1)

    assert [pq.pop(), pq.pop(), pq.pop()] == [first, second, third]
