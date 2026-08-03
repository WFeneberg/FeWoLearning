from typing import Any

import pytest

from ex043_context_manager_class import Indent, Suppress, Tracked, Transaction


# NotImplementedError subclasses RuntimeError, so a test expecting RuntimeError
# would be satisfied by an unimplemented stub. This type cannot be confused with it.
class Boom(Exception):
    pass


def test_tracked_logs_enter_and_exit() -> None:
    log: list[str] = []

    with Tracked(log):
        log.append("body")

    assert log == ["enter", "body", "exit"]


def test_tracked_returns_itself_from_enter() -> None:
    log: list[str] = []

    with Tracked(log) as tracked:
        assert isinstance(tracked, Tracked)


def test_tracked_logs_exit_even_when_the_body_raises() -> None:
    log: list[str] = []

    with pytest.raises(Boom):
        with Tracked(log):
            raise Boom("boom")

    assert log == ["enter", "exit"]


def test_suppress_swallows_a_listed_exception() -> None:
    with Suppress(ValueError) as guard:
        raise ValueError("gone")

    assert isinstance(guard.caught, ValueError)


def test_suppress_lets_an_unlisted_exception_through() -> None:
    with pytest.raises(TypeError):
        with Suppress(ValueError):
            raise TypeError("stays")


def test_suppress_records_nothing_on_a_clean_block() -> None:
    with Suppress(ValueError) as guard:
        pass

    assert guard.caught is None


def test_suppress_accepts_several_types() -> None:
    with Suppress(ValueError, KeyError) as guard:
        raise KeyError("k")

    assert isinstance(guard.caught, KeyError)


def test_suppress_with_no_types_suppresses_nothing() -> None:
    with pytest.raises(ValueError):
        with Suppress():
            raise ValueError


def test_transaction_keeps_edits_on_a_clean_exit() -> None:
    data: dict[str, Any] = {"a": 1}

    with Transaction(data) as scope:
        scope["b"] = 2

    assert data == {"a": 1, "b": 2}


def test_transaction_rolls_back_on_an_exception() -> None:
    data: dict[str, Any] = {"a": 1}

    with pytest.raises(Boom):
        with Transaction(data) as scope:
            scope["b"] = 2
            del scope["a"]
            raise Boom("abort")

    assert data == {"a": 1}


def test_transaction_records_whether_it_committed() -> None:
    data: dict[str, Any] = {}

    tx = Transaction(data)
    with tx:
        pass
    assert tx.committed is True

    tx2 = Transaction(data)
    with pytest.raises(ValueError):
        with tx2:
            raise ValueError
    assert tx2.committed is False


def test_transaction_yields_the_same_dict_object() -> None:
    data: dict[str, Any] = {}

    with Transaction(data) as scope:
        assert scope is data


def test_indent_deepens_and_restores() -> None:
    indent = Indent()

    assert indent.level == 0
    with indent as level:
        assert level == 1
        assert indent.level == 1
    assert indent.level == 0


def test_indent_is_reentrant() -> None:
    indent = Indent()

    with indent:
        with indent as inner:
            assert inner == 2
            assert indent.level == 2
        assert indent.level == 1
    assert indent.level == 0


def test_indent_restores_on_an_exception() -> None:
    indent = Indent()

    with pytest.raises(Boom):
        with indent:
            raise Boom

    assert indent.level == 0
