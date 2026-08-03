"""Exercise 043 — Context managers as classes (intermediate).

Goal:   Implement the context-manager protocol by hand.
Drills: __enter__/__exit__, what the three __exit__ arguments mean, suppressing an
        exception by returning True, cleanup that must run either way, reentrancy.
Passes: when `pytest exercises/02-intermediate/test_ex043_context_manager_class.py` is green.
"""

from types import TracebackType
from typing import Any


class Tracked:
    """Appends "enter" and "exit" to `log` around the block.

    ``__enter__`` returns the instance itself, so ``with Tracked(log) as t`` binds
    the manager. "exit" is appended even when the block raises, and the exception is
    **not** suppressed.
    """

    def __init__(self, log: list[str]) -> None:
        raise NotImplementedError

    def __enter__(self) -> "Tracked":
        raise NotImplementedError

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        raise NotImplementedError


class Suppress:
    """Suppresses the listed exception types raised inside the block.

    ``__exit__`` returning True is what swallows an exception; returning False (or
    None) lets it propagate. Records the suppressed exception on ``self.caught``,
    which stays None when nothing was raised.
    """

    def __init__(self, *exceptions: type[BaseException]) -> None:
        raise NotImplementedError

    def __enter__(self) -> "Suppress":
        raise NotImplementedError

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        raise NotImplementedError


class Transaction:
    """A commit/rollback scope over a dict.

    ``__enter__`` snapshots `target` and returns it for editing. Leaving the block
    normally keeps the edits; leaving it via an exception restores the snapshot and
    lets the exception propagate. ``self.committed`` records which happened.
    """

    def __init__(self, target: dict[str, Any]) -> None:
        raise NotImplementedError

    def __enter__(self) -> dict[str, Any]:
        raise NotImplementedError

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        raise NotImplementedError


class Indent:
    """A reentrant indentation scope.

    Each nested ``with`` deepens ``self.level`` by one and restores it on the way
    out, so the same instance can be entered several times over.
    """

    def __init__(self) -> None:
        raise NotImplementedError

    def __enter__(self) -> int:
        raise NotImplementedError

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        raise NotImplementedError
