"""Exercise 044 — contextlib (reference solution)."""

from contextlib import ExitStack, contextmanager
from typing import Any, Callable, Iterator

_MISSING = object()


@contextmanager
def tracked(log: list[str]) -> Iterator[None]:
    log.append("enter")
    try:
        yield
    finally:
        # Without finally, a raising block would propagate straight out of the
        # yield and this append would never run.
        log.append("exit")


@contextmanager
def temporary_value(store: dict[str, Any], key: str, value: Any) -> Iterator[Any]:
    previous = store.get(key, _MISSING)
    store[key] = value
    try:
        yield value
    finally:
        if previous is _MISSING:
            # The key did not exist before, so remove it rather than leaving None.
            store.pop(key, None)
        else:
            store[key] = previous


@contextmanager
def collecting(sink: list[Any]) -> Iterator[Callable[[Any], None]]:
    try:
        yield sink.append
    except BaseException:
        sink.append("failed")
        raise
    else:
        sink.append("done")


@contextmanager
def ignore(*exceptions: type[BaseException]) -> Iterator[None]:
    try:
        yield
    except exceptions:
        # An exception from the block surfaces at the yield, so a plain except
        # catches it. Not re-raising is what suppresses it — no True involved.
        # An empty `exceptions` tuple matches nothing, so nothing is suppressed.
        pass


def open_all(paths: list[str], opener: Callable[[str], Any]) -> Any:
    @contextmanager
    def manager() -> Iterator[list[Any]]:
        with ExitStack() as stack:
            handles: list[Any] = []
            for path in paths:
                handle = opener(path)
                # Registering immediately is the point: if the *next* opener call
                # raises, everything registered so far is still closed.
                stack.callback(handle.close)
                handles.append(handle)
            yield handles

    return manager()
