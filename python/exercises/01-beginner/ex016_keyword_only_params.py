"""Exercise 016 — Keyword-only and positional-only parameters (beginner).

Goal:   Control *how* callers may pass each argument, using the `*` and `/`
        markers in a signature.
Drills: keyword-only parameters after `*`, positional-only before `/`, mixing
        both, and why either restriction is worth having.
Passes: when `pytest exercises/01-beginner/test_ex016_keyword_only_params.py` is green.

Note:   the signatures below are deliberately missing their markers — adding them
        is half of each task. The tests assert that the wrong call style raises
        TypeError, and Python's own argument binding produces that error once the
        markers are in place.
"""


def connect(host: str, port: int = 5432, timeout: float = 5.0) -> str:
    """Return ``"<host>:<port> (timeout=<timeout>s)"``.

    Make `port` and `timeout` **keyword-only**, so ``connect("db", 1234)`` raises
    TypeError. That is the point: two bare numbers at a call site say nothing about
    which is which.
    """
    raise NotImplementedError


def divide(a: float, b: float) -> float:
    """Return ``a / b``.

    Make both parameters **positional-only**, so ``divide(a=1, b=2)`` raises
    TypeError. Division by zero raises ZeroDivisionError.
    """
    raise NotImplementedError


def clamp(value: int, low: int = 0, high: int = 100) -> int:
    """Clamp `value` into ``[low, high]``.

    Mix all three kinds: `value` positional-only, `low` passable either way,
    `high` keyword-only. Raises ValueError when ``low > high``.
    """
    raise NotImplementedError


def make_url(scheme: str, host: str, path: str = "/", query: dict[str, str] | None = None) -> str:
    """Assemble ``"<scheme>://<host><path>"``, appending ``?k=v&k2=v2`` when
    `query` is non-empty.

    Make `path` and `query` **keyword-only**. Query pairs keep insertion order.
    """
    raise NotImplementedError


def rename(mapping: dict[str, int], **renames: str) -> dict[str, int]:
    """Return a copy of `mapping` with keys renamed.

    ``rename({"a": 1}, a="b")`` -> ``{"b": 1}``. Keys not mentioned stay as they
    are. Make `mapping` **positional-only** — that is what allows a key literally
    called "mapping" to be renamed through **renames.
    """
    raise NotImplementedError
