import pytest

from ex049_typing_generics import Pair, Stack, first, group_by, largest


def test_stack_push_and_pop_is_lifo() -> None:
    stack: Stack[int] = Stack()
    stack.push(1)
    stack.push(2)

    assert stack.pop() == 2
    assert stack.pop() == 1


def test_stack_len() -> None:
    stack: Stack[str] = Stack()

    assert len(stack) == 0
    stack.push("a")
    assert len(stack) == 1


def test_stack_bool() -> None:
    stack: Stack[int] = Stack()

    assert bool(stack) is False
    stack.push(1)
    assert bool(stack) is True


def test_stack_peek_does_not_remove() -> None:
    stack: Stack[int] = Stack()
    stack.push(7)

    assert stack.peek() == 7
    assert len(stack) == 1


def test_stack_pop_when_empty_raises() -> None:
    with pytest.raises(IndexError):
        Stack[int]().pop()


def test_stack_peek_when_empty_raises() -> None:
    with pytest.raises(IndexError):
        Stack[int]().peek()


def test_stack_instances_are_independent() -> None:
    a: Stack[int] = Stack()
    b: Stack[int] = Stack()

    a.push(1)

    assert len(b) == 0


def test_stack_holds_any_type() -> None:
    stack: Stack[str] = Stack()
    stack.push("hello")

    assert stack.pop() == "hello"


def test_pair_holds_two_values() -> None:
    pair = Pair(1, "a")

    assert pair.first == 1
    assert pair.second == "a"


def test_pair_swapped() -> None:
    assert Pair(1, "a").swapped() == Pair("a", 1)


def test_pair_swapped_returns_a_new_object() -> None:
    original = Pair(1, "a")

    assert original.swapped() is not original
    assert original == Pair(1, "a")


def test_pair_equality() -> None:
    assert Pair(1, "a") == Pair(1, "a")
    assert Pair(1, "a") != Pair(1, "b")


def test_pair_is_not_equal_to_other_types() -> None:
    assert Pair(1, 2) != (1, 2)


def test_pair_repr() -> None:
    assert repr(Pair(1, "a")) == "Pair(1, 'a')"


@pytest.mark.parametrize(
    "items, expected",
    [([1, 2, 3], 1), (["a"], "a"), ([], None)],
)
def test_first(items: list[object], expected: object) -> None:
    assert first(items) == expected


def test_first_uses_the_default_for_an_empty_input() -> None:
    assert first([], "fallback") == "fallback"


def test_first_works_on_a_generator() -> None:
    assert first(n for n in [5, 6]) == 5


def test_first_consumes_only_one_item() -> None:
    iterator = iter([1, 2, 3])

    first(iterator)

    assert list(iterator) == [2, 3]


@pytest.mark.parametrize(
    "items, expected",
    [([3, 1, 2], 3), (["a", "c", "b"], "c"), ([5], 5), ([], None)],
)
def test_largest(items: list[object], expected: object) -> None:
    assert largest(items) == expected


def test_group_by() -> None:
    words = ["apple", "avocado", "banana", "blueberry", "cherry"]

    assert group_by(words, lambda w: w[0]) == {
        "a": ["apple", "avocado"],
        "b": ["banana", "blueberry"],
        "c": ["cherry"],
    }


def test_group_by_preserves_encounter_order() -> None:
    assert group_by([3, 1, 4, 1, 5], lambda n: n % 2) == {1: [3, 1, 1, 5], 0: [4]}


def test_group_by_empty() -> None:
    assert group_by([], lambda x: x) == {}
