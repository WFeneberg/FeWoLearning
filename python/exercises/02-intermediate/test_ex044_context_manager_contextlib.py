from typing import Any

import pytest

from ex044_context_manager_contextlib import (
    collecting,
    ignore,
    open_all,
    temporary_value,
    tracked,
)


# NotImplementedError subclasses RuntimeError, so a test expecting RuntimeError
# would be satisfied by an unimplemented stub. This type cannot be confused with it.
class Boom(Exception):
    pass


def test_tracked_logs_around_the_block() -> None:
    log: list[str] = []

    with tracked(log):
        log.append("body")

    assert log == ["enter", "body", "exit"]


def test_tracked_still_logs_exit_when_the_block_raises() -> None:
    log: list[str] = []

    with pytest.raises(Boom):
        with tracked(log):
            raise Boom

    assert log == ["enter", "exit"]


def test_temporary_value_sets_and_restores_an_existing_key() -> None:
    store: dict[str, Any] = {"mode": "prod"}

    with temporary_value(store, "mode", "test") as value:
        assert value == "test"
        assert store["mode"] == "test"

    assert store == {"mode": "prod"}


def test_temporary_value_removes_a_key_that_did_not_exist() -> None:
    store: dict[str, Any] = {}

    with temporary_value(store, "new", 1):
        assert store == {"new": 1}

    assert store == {}


def test_temporary_value_restores_after_an_exception() -> None:
    store: dict[str, Any] = {"a": 1}

    with pytest.raises(ValueError):
        with temporary_value(store, "a", 2):
            raise ValueError

    assert store == {"a": 1}


def test_collecting_marks_a_clean_block_done() -> None:
    sink: list[Any] = []

    with collecting(sink) as emit:
        emit("x")
        emit("y")

    assert sink == ["x", "y", "done"]


def test_collecting_marks_a_failing_block_failed() -> None:
    sink: list[Any] = []

    with pytest.raises(Boom):
        with collecting(sink) as emit:
            emit("x")
            raise Boom

    assert sink == ["x", "failed"]


def test_ignore_swallows_a_listed_exception() -> None:
    reached = []

    with ignore(ValueError):
        raise ValueError

    reached.append("after")
    assert reached == ["after"]


def test_ignore_lets_an_unlisted_exception_through() -> None:
    with pytest.raises(TypeError):
        with ignore(ValueError):
            raise TypeError


def test_ignore_with_a_clean_block() -> None:
    with ignore(ValueError):
        value = 1

    assert value == 1


def test_ignore_with_no_types_suppresses_nothing() -> None:
    with pytest.raises(ValueError):
        with ignore():
            raise ValueError


class FakeFile:
    def __init__(self, name: str) -> None:
        self.name = name
        self.closed = False

    def close(self) -> None:
        self.closed = True


def test_open_all_opens_everything() -> None:
    opened: list[FakeFile] = []

    def opener(path: str) -> FakeFile:
        handle = FakeFile(path)
        opened.append(handle)
        return handle

    with open_all(["a", "b"], opener) as handles:
        assert [h.name for h in handles] == ["a", "b"]
        assert all(not h.closed for h in handles)

    assert all(h.closed for h in opened)


def test_open_all_closes_what_it_opened_when_a_later_open_fails() -> None:
    opened: list[FakeFile] = []

    def opener(path: str) -> FakeFile:
        if path == "bad":
            raise OSError("cannot open")
        handle = FakeFile(path)
        opened.append(handle)
        return handle

    with pytest.raises(OSError):
        with open_all(["good", "bad"], opener):
            pass

    # "good" was already open when "bad" failed, and must not have been leaked.
    assert len(opened) == 1
    assert opened[0].closed is True


def test_open_all_with_no_paths() -> None:
    def opener(path: str) -> FakeFile:
        raise AssertionError("should not be called")

    with open_all([], opener) as handles:
        assert handles == []
