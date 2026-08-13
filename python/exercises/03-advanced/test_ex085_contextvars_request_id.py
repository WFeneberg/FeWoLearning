import pytest

from ex085_contextvars_request_id import (
    current_request_id,
    handle_many,
    handle_request,
    request_context,
    run,
    tag,
)


def test_no_active_request_by_default():
    assert current_request_id() is None


def test_request_context_sets_and_restores():
    assert current_request_id() is None
    with request_context("abc"):
        assert current_request_id() == "abc"
    assert current_request_id() is None


def test_request_context_restores_on_exception():
    with pytest.raises(ValueError):
        with request_context("abc"):
            raise ValueError("boom")
    assert current_request_id() is None


def test_nested_contexts_restore_the_outer_value():
    with request_context("outer"):
        assert current_request_id() == "outer"
        with request_context("inner"):
            assert current_request_id() == "inner"
        assert current_request_id() == "outer"
    assert current_request_id() is None


def test_tag_outside_any_request():
    assert tag("hello") == "[-] hello"


def test_tag_inside_a_request():
    with request_context("r1"):
        assert tag("hello") == "[r1] hello"


def test_handle_request_returns_a_tagged_result():
    assert run(handle_request("r1", 0)) == "[r1] done"


def test_handle_many_empty():
    assert run(handle_many([])) == []


def test_handle_many_preserves_argument_order():
    results = run(handle_many([("r1", 0), ("r2", 0), ("r3", 0)]))
    assert results == ["[r1] done", "[r2] done", "[r3] done"]


def test_handle_many_isolates_concurrent_requests_despite_out_of_order_completion():
    # r2 finishes first (delay 0), then r3, then r1 — a shared/global variable would
    # leak the last writer's id into the earlier finishers.
    results = run(handle_many([("r1", 0.03), ("r2", 0.0), ("r3", 0.01)]))
    assert results == ["[r1] done", "[r2] done", "[r3] done"]


def test_handle_many_does_not_leak_into_the_caller():
    run(handle_many([("r1", 0)]))
    assert current_request_id() is None
