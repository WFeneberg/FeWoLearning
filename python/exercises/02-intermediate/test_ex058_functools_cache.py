import pytest

from ex058_functools_cache import (
    Dataset,
    cache_stats,
    make_bounded,
    make_counted_fib,
    memoise_with_stats,
    normalise_for_cache,
)


def test_counted_fib_values() -> None:
    fib, _ = make_counted_fib()

    assert [fib(n) for n in range(8)] == [0, 1, 1, 2, 3, 5, 8, 13]


def test_counted_fib_memoises() -> None:
    fib, call_count = make_counted_fib()

    assert fib(30) == 832040
    # Naive recursion would run the body over a million times.
    assert call_count() <= 31


def test_counted_fib_repeat_calls_are_free() -> None:
    fib, call_count = make_counted_fib()

    fib(20)
    before = call_count()
    fib(20)

    assert call_count() == before


def test_counted_fib_instances_have_separate_caches() -> None:
    fib_a, count_a = make_counted_fib()
    fib_b, count_b = make_counted_fib()

    fib_a(10)

    assert count_b() == 0


def test_make_bounded_returns_its_argument() -> None:
    identity = make_bounded(2)

    assert identity(5) == 5


def test_make_bounded_evicts_beyond_maxsize() -> None:
    identity = make_bounded(2)

    identity(1)
    identity(2)
    identity(3)

    hits, misses, currsize = cache_stats(identity)

    assert currsize == 2
    assert misses == 3
    assert hits == 0


def test_make_bounded_counts_hits() -> None:
    identity = make_bounded(4)

    identity(1)
    identity(1)
    identity(1)

    hits, misses, _ = cache_stats(identity)

    assert (hits, misses) == (2, 1)


@pytest.mark.parametrize("maxsize", [0, -1])
def test_make_bounded_rejects_a_bad_maxsize(maxsize: int) -> None:
    with pytest.raises(ValueError):
        make_bounded(maxsize)


def test_normalise_for_cache_is_hashable() -> None:
    key = normalise_for_cache([1, 2, 3])

    assert key == (1, 2, 3)
    assert hash(key) is not None


def test_normalise_for_cache_empty() -> None:
    assert normalise_for_cache([]) == ()


def test_dataset_total() -> None:
    assert Dataset([1, 2, 3]).total == 6


def test_dataset_total_is_computed_once() -> None:
    dataset = Dataset([1, 2, 3])

    dataset.total
    dataset.total
    dataset.total

    assert dataset.compute_count == 1


def test_dataset_instances_are_independent() -> None:
    a = Dataset([1])
    b = Dataset([2, 2])

    assert a.total == 1
    assert b.total == 4


def test_dataset_empty() -> None:
    assert Dataset([]).total == 0


def test_memoise_with_stats_counts_hits_and_misses() -> None:
    calls: list[int] = []

    @memoise_with_stats
    def square(n: int) -> int:
        calls.append(n)
        return n * n

    assert square(3) == 9
    assert square(3) == 9
    assert square(4) == 16

    assert calls == [3, 4]
    assert square.hits == 1
    assert square.misses == 2


def test_memoise_with_stats_starts_at_zero() -> None:
    @memoise_with_stats
    def identity(n: int) -> int:
        return n

    assert identity.hits == 0
    assert identity.misses == 0


def test_memoise_with_stats_rejects_keyword_calls() -> None:
    @memoise_with_stats
    def add(a: int, b: int = 0) -> int:
        return a + b

    with pytest.raises(TypeError):
        add(1, b=2)


def test_memoise_with_stats_supports_several_positional_arguments() -> None:
    @memoise_with_stats
    def add(a: int, b: int) -> int:
        return a + b

    assert add(1, 2) == 3
    assert add(1, 2) == 3
    assert add.hits == 1
