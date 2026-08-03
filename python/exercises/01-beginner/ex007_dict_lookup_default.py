"""Exercise 007 — Dict lookup with defaults (beginner).

Goal:   Read and grow dictionaries without tripping over missing keys.
Drills: dict.get, setdefault, KeyError vs default, dict.pop, nested lookups.
Passes: when `pytest exercises/01-beginner/test_ex007_dict_lookup_default.py` is green.
"""


def lookup(scores: dict[str, int], name: str, default: int = 0) -> int:
    """Return the score for `name`, or `default` when the name is absent.

    Must not raise for a missing key.
    """
    raise NotImplementedError


def require(scores: dict[str, int], name: str) -> int:
    """Return the score for `name`, raising KeyError when it is absent.

    This is the opposite of `lookup`: here a missing key is a programming error.
    """
    raise NotImplementedError


def add_to_group(groups: dict[str, list[str]], key: str, member: str) -> dict[str, list[str]]:
    """Append `member` to the list under `key`, creating the list if needed.

    Mutates and returns `groups`. Use setdefault rather than an ``if key in`` check.
    """
    raise NotImplementedError


def increment(counts: dict[str, int], key: str, by: int = 1) -> dict[str, int]:
    """Add `by` to the count under `key`, treating an absent key as 0.

    Mutates and returns `counts`.
    """
    raise NotImplementedError


def take(settings: dict[str, str], key: str, default: str = "") -> tuple[str, dict[str, str]]:
    """Remove `key` and return ``(value_or_default, settings)``.

    Removing an absent key must not raise.
    """
    raise NotImplementedError


def nested_get(data: dict[str, object], path: list[str], default: object = None) -> object:
    """Walk `path` through nested dicts, returning `default` if the walk breaks.

    ``nested_get({"a": {"b": 1}}, ["a", "b"])`` -> ``1``.
    A path that hits a missing key, or a non-dict on the way, yields `default`.
    An empty path returns `data` itself.
    """
    raise NotImplementedError
