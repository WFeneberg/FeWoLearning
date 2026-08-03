from collections import defaultdict
from typing import Any

import pytest

from ex065_defaultdict_grouping import (
    collect_unique,
    count_by,
    group_by,
    invert_multi,
    nested_counts,
    reads_create_keys,
    safe_lookup,
)


def test_group_by() -> None:
    words = ["apple", "avocado", "banana"]

    assert group_by(words, lambda w: w[0]) == {"a": ["apple", "avocado"], "b": ["banana"]}


def test_group_by_returns_a_plain_dict() -> None:
    result = group_by([1], lambda n: n)

    # A defaultdict would insert on a missing-key read, which the caller never asked for.
    assert type(result) is dict
    assert not isinstance(result, defaultdict)


def test_group_by_missing_key_raises() -> None:
    result = group_by([1], lambda n: n)

    with pytest.raises(KeyError):
        result["nope"]


def test_group_by_empty() -> None:
    assert group_by([], lambda x: x) == {}


def test_count_by() -> None:
    assert count_by(["a", "bb", "cc", "d"], len) == {1: 2, 2: 2}


def test_count_by_returns_a_plain_dict() -> None:
    assert type(count_by([1], lambda n: n)) is dict


def test_count_by_empty() -> None:
    assert count_by([], lambda x: x) == {}


def test_collect_unique() -> None:
    pairs = [("a", 1), ("a", 2), ("a", 1), ("b", 3)]

    assert collect_unique(pairs) == {"a": {1, 2}, "b": {3}}


def test_collect_unique_empty() -> None:
    assert collect_unique([]) == {}


def test_nested_counts() -> None:
    triples = [("x", "p", 1), ("x", "p", 2), ("x", "q", 5), ("y", "p", 3)]

    assert nested_counts(triples) == {"x": {"p": 3, "q": 5}, "y": {"p": 3}}


def test_nested_counts_is_plain_all_the_way_down() -> None:
    result = nested_counts([("x", "p", 1)])

    assert type(result) is dict
    assert type(result["x"]) is dict


def test_nested_counts_empty() -> None:
    assert nested_counts([]) == {}


def test_invert_multi() -> None:
    assert invert_multi({"a": ["x", "y"], "b": ["x"]}) == {"x": ["a", "b"], "y": ["a"]}


def test_invert_multi_preserves_first_seen_order() -> None:
    result = invert_multi({"b": ["k"], "a": ["k"]})

    assert result == {"k": ["b", "a"]}


def test_invert_multi_with_empty_lists() -> None:
    assert invert_multi({"a": []}) == {}


def test_invert_multi_empty() -> None:
    assert invert_multi({}) == {}


def test_reads_create_keys_demonstrates_the_trap() -> None:
    assert reads_create_keys() == (1, ["absent"])


def test_safe_lookup_returns_the_value() -> None:
    assert safe_lookup({"a": ["x"]}, "a") == ["x"]


def test_safe_lookup_returns_an_empty_list_for_a_missing_key() -> None:
    assert safe_lookup({}, "nope") == []


def test_safe_lookup_does_not_insert() -> None:
    store: dict[str, list[str]] = {}

    safe_lookup(store, "nope")

    assert store == {}
