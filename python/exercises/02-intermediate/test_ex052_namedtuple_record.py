import pytest

from ex052_namedtuple_record import (
    Point,
    Segment,
    as_dict,
    field_names,
    from_iterable,
    sort_points,
    total_length,
    with_x,
)


def test_point_constructs_with_a_default_y() -> None:
    assert Point(1).y == 0
    assert Point(1, 2) == Point(1, 2)


def test_point_is_indexable() -> None:
    point = Point(3, 4)

    assert point[0] == 3
    assert point[1] == 4


def test_point_unpacks() -> None:
    x, y = Point(5, 6)

    assert (x, y) == (5, 6)


def test_point_equals_a_plain_tuple() -> None:
    # This is the NamedTuple-versus-frozen-dataclass difference: it *is* a tuple.
    assert Point(1, 2) == (1, 2)


def test_point_is_immutable() -> None:
    point = Point(1, 2)

    with pytest.raises(AttributeError):
        point.x = 9  # type: ignore[misc]


def test_point_is_hashable() -> None:
    assert len({Point(1, 2), Point(1, 2), Point(3, 4)}) == 2


def test_point_repr() -> None:
    assert repr(Point(1, 2)) == "Point(x=1, y=2)"


def test_segment_length_horizontal() -> None:
    assert Segment(Point(0, 0), Point(3, 0)).length == pytest.approx(3.0)


def test_segment_length_diagonal() -> None:
    assert Segment(Point(0, 0), Point(3, 4)).length == pytest.approx(5.0)


def test_segment_length_zero() -> None:
    assert Segment(Point(2, 2), Point(2, 2)).length == pytest.approx(0.0)


def test_as_dict() -> None:
    assert as_dict(Point(1, 2)) == {"x": 1, "y": 2}


def test_with_x() -> None:
    original = Point(1, 2)

    changed = with_x(original, 9)

    assert changed == Point(9, 2)
    assert original == Point(1, 2)


def test_field_names() -> None:
    assert field_names() == ("x", "y")


def test_from_iterable() -> None:
    assert from_iterable([7, 8]) == Point(7, 8)


def test_from_iterable_accepts_a_generator() -> None:
    assert from_iterable(n for n in [1, 2]) == Point(1, 2)


@pytest.mark.parametrize("values", [[1], [1, 2, 3]])
def test_from_iterable_wrong_arity_raises(values: list[int]) -> None:
    with pytest.raises(TypeError):
        from_iterable(values)


def test_sort_points_is_x_then_y() -> None:
    points = [Point(2, 1), Point(1, 5), Point(1, 2)]

    assert sort_points(points) == [Point(1, 2), Point(1, 5), Point(2, 1)]


def test_sort_points_empty() -> None:
    assert sort_points([]) == []


def test_total_length() -> None:
    segments = [
        Segment(Point(0, 0), Point(3, 4)),
        Segment(Point(0, 0), Point(0, 2)),
    ]

    assert total_length(segments) == pytest.approx(7.0)


def test_total_length_empty() -> None:
    assert total_length([]) == 0
