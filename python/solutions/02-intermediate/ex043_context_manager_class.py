"""Exercise 043 — Context managers as classes (reference solution)."""

from types import TracebackType
from typing import Any


class Tracked:
    def __init__(self, log: list[str]) -> None:
        self.log = log

    def __enter__(self) -> "Tracked":
        self.log.append("enter")
        return self

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        self.log.append("exit")
        # False lets any exception propagate; the log entry happens either way.
        return False


class Suppress:
    def __init__(self, *exceptions: type[BaseException]) -> None:
        self.exceptions = exceptions
        self.caught: BaseException | None = None

    def __enter__(self) -> "Suppress":
        return self

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        if exc_type is None:
            return False
        if self.exceptions and issubclass(exc_type, self.exceptions):
            self.caught = exc
            # Returning True is the only way to swallow an exception.
            return True
        return False


class Transaction:
    def __init__(self, target: dict[str, Any]) -> None:
        self.target = target
        self.snapshot: dict[str, Any] = {}
        self.committed = False

    def __enter__(self) -> dict[str, Any]:
        # A shallow copy is enough for the flat dict this scope promises to guard.
        self.snapshot = dict(self.target)
        return self.target

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        if exc_type is None:
            self.committed = True
            return False
        # Restore in place: the caller still holds a reference to this same dict.
        self.target.clear()
        self.target.update(self.snapshot)
        self.committed = False
        return False


class Indent:
    def __init__(self) -> None:
        self.level = 0

    def __enter__(self) -> int:
        # Counting rather than storing a flag is what makes this reentrant.
        self.level += 1
        return self.level

    def __exit__(
        self,
        exc_type: type[BaseException] | None,
        exc: BaseException | None,
        tb: TracebackType | None,
    ) -> bool:
        self.level -= 1
        return False
