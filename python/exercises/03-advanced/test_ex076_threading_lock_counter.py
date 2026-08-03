import pytest

from ex076_threading_lock_counter import (
    Counter,
    ReentrantAccount,
    run_threads,
    thread_local_values,
    total_with_lock,
    wait_for_signal,
)


def test_counter_starts_at_zero() -> None:
    assert Counter().value == 0


def test_counter_increment() -> None:
    counter = Counter()
    counter.increment()
    counter.increment()

    assert counter.value == 2


def test_counter_increment_by_more() -> None:
    counter = Counter()
    counter.increment(5)

    assert counter.value == 5


def test_counter_instances_are_independent() -> None:
    a, b = Counter(), Counter()
    a.increment()

    assert b.value == 0


def test_run_threads_runs_the_worker_once_per_thread() -> None:
    counter = Counter()

    run_threads(counter.increment, 4)

    assert counter.value == 4


def test_run_threads_waits_for_completion() -> None:
    counter = Counter()

    run_threads(lambda: counter.increment(10), 3)

    # If join() were missing, the value could still be short here.
    assert counter.value == 30


@pytest.mark.parametrize("count", [0, -1])
def test_run_threads_rejects_a_bad_count(count: int) -> None:
    with pytest.raises(ValueError):
        run_threads(lambda: None, count)


@pytest.mark.parametrize(
    "thread_count, per_thread",
    [(4, 1000), (8, 500), (2, 5000), (1, 100)],
)
def test_total_with_lock_is_exact(thread_count: int, per_thread: int) -> None:
    # Without a lock this loses increments: `+= 1` is read-add-write, not atomic.
    assert total_with_lock(thread_count, per_thread) == thread_count * per_thread


def test_reentrant_account_starts_empty() -> None:
    assert ReentrantAccount().balance == 0


def test_reentrant_account_deposit() -> None:
    account = ReentrantAccount()
    account.deposit(10)

    assert account.balance == 10


def test_reentrant_account_deposit_twice_does_not_deadlock() -> None:
    account = ReentrantAccount()

    # With a plain Lock this call would block forever against itself.
    account.deposit_twice(5)

    assert account.balance == 10


def test_reentrant_account_under_threads() -> None:
    account = ReentrantAccount()

    run_threads(lambda: account.deposit_twice(1), 10)

    assert account.balance == 20


def test_wait_for_signal_ordering() -> None:
    assert wait_for_signal() == ["signalled", "waited"]


def test_wait_for_signal_is_repeatable() -> None:
    for _ in range(5):
        assert wait_for_signal() == ["signalled", "waited"]


@pytest.mark.parametrize("thread_count", [1, 4, 10])
def test_thread_local_values_are_per_thread(thread_count: int) -> None:
    assert thread_local_values(thread_count) == list(range(thread_count))
