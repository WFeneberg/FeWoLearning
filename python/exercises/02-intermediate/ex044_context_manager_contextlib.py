"""Exercise 044 — contextlib (intermediate).

Goal:   Write context managers as generators, and compose them.
Drills: @contextmanager, try/finally inside a generator, yielding a value,
        suppress/ExitStack, why the cleanup after `yield` needs `finally`.
Passes: when `pytest exercises/02-intermediate/test_ex044_context_manager_contextlib.py` is green.
"""

from contextlib import contextmanager
from typing import Any, Callable, Iterator


@contextmanager
def tracked(log: list[str]) -> Iterator[None]:
    """Append "enter" before the block and "exit" after it.

    The "exit" append must be in a ``finally``: without it, a raising block would
    skip everything after the ``yield``.
    """
    raise NotImplementedError


@contextmanager
def temporary_value(store: dict[str, Any], key: str, value: Any) -> Iterator[Any]:
    """Set ``store[key] = value`` for the block, then restore the previous state.

    A key that did not exist before is removed again, not set to None. Yields the
    value. Restoration must survive an exception.
    """
    raise NotImplementedError


@contextmanager
def collecting(sink: list[Any]) -> Iterator[Callable[[Any], None]]:
    """Yield a function that appends to `sink`, and mark completion afterwards.

    After a clean block, append the sentinel string "done"; after a failing one,
    append "failed" and let the exception through.
    """
    raise NotImplementedError


@contextmanager
def ignore(*exceptions: type[BaseException]) -> Iterator[None]:
    """Swallow the listed exception types.

    Inside a generator-based manager an exception from the block is raised *at* the
    ``yield``, so it is caught with a normal ``except`` around it. Swallowing means
    simply not re-raising — there is no True to return here.
    """
    raise NotImplementedError


def open_all(paths: list[str], opener: Callable[[str], Any]) -> Any:
    """Open every path via `opener` and close them all afterwards, using ExitStack.

    Returns a context manager yielding the list of opened objects. Everything opened
    is closed even if a later `opener` call raises — which is the whole reason
    ExitStack exists rather than a stack of nested ``with`` statements.

    Opened objects have a ``close()`` method.
    """
    raise NotImplementedError
