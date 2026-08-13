import pytest

from ex089_streaming_generator_pipeline import filter_stage, map_stage, moving_average, pipe, take


def test_pipe_with_no_stages_passes_items_through():
    assert list(pipe([1, 2, 3])) == [1, 2, 3]


def test_pipe_applies_stages_in_order():
    result = pipe([1, 2, 3, 4, 5], filter_stage(lambda x: x % 2 == 0), map_stage(lambda x: x * 100))
    assert list(result) == [200, 400]


def test_pipe_stage_order_matters():
    # map-then-filter sees the transformed values; filter-then-map does not.
    mapped_then_filtered = pipe([1, 2, 3], map_stage(lambda x: x * 10), filter_stage(lambda x: x > 15))
    assert list(mapped_then_filtered) == [20, 30]


def test_take_stops_early():
    calls: list[int] = []

    def source():
        n = 0
        while True:
            calls.append(n)
            yield n
            n += 1

    assert take(source(), 3) == [0, 1, 2]
    assert len(calls) == 3


def test_take_handles_a_shorter_source():
    assert take(iter([1, 2, 3]), 5) == [1, 2, 3]


def test_take_zero():
    assert take([1, 2, 3], 0) == []


def test_moving_average_basic():
    assert list(moving_average([1, 2, 3, 4], 2)) == [1.0, 1.5, 2.5, 3.5]


def test_moving_average_before_the_window_fills():
    assert list(moving_average([2, 4, 6], 5)) == [2.0, 3.0, 4.0]


def test_moving_average_rejects_a_non_positive_window():
    with pytest.raises(ValueError):
        list(moving_average([1, 2, 3], 0))


def test_pipeline_over_an_infinite_source_stays_lazy():
    pulled: list[int] = []

    def counting_source():
        n = 0
        while True:
            pulled.append(n)
            yield n
            n += 1

    stream = pipe(
        counting_source(),
        filter_stage(lambda x: x % 2 == 0),
        map_stage(lambda x: x * 10),
    )

    assert take(stream, 3) == [0, 20, 40]
    # Only enough of the infinite source was pulled to produce 3 even results
    # (0, 2, 4) — a pipeline that first drained the source into a list would have
    # hung, or in a bounded test double, pulled far more than 6 items.
    assert len(pulled) <= 6
