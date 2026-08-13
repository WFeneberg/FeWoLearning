import sys
import weakref

import pytest

from ex083_slots_memory import DictPoint, Point, Point3D, declared_slots, has_instance_dict


def test_unknown_attribute_is_rejected():
    p = Point(1, 2)
    with pytest.raises(AttributeError):
        p.z = 3  # type: ignore[attr-defined]


def test_point_has_no_instance_dict():
    assert has_instance_dict(Point(1, 2)) is False


def test_dictpoint_has_an_instance_dict():
    assert has_instance_dict(DictPoint(1, 2)) is True


def test_dictpoint_accepts_arbitrary_attributes():
    assert has_instance_dict(DictPoint(1, 2)) is True
    dp = DictPoint(1, 2)
    dp.z = 3  # type: ignore[attr-defined]
    assert dp.z == 3


def test_declared_slots_of_point():
    assert declared_slots(Point) == {"x", "y"}


def test_declared_slots_of_point3d_includes_the_inherited_ones():
    assert declared_slots(Point3D) == {"x", "y", "z"}


def test_point3d_only_declares_its_own_new_slot():
    # The subclass's *own* __slots__ must not repeat what the base already declared.
    assert Point3D.__dict__["__slots__"] == ("z",)


def test_declared_slots_of_an_unslotted_class_is_empty():
    assert declared_slots(DictPoint) == set()


def test_point_equality():
    assert Point(1, 2) == Point(1, 2)
    assert Point(1, 2) != Point(1, 3)


def test_point3d_equality_includes_z():
    assert Point3D(1, 2, 3) == Point3D(1, 2, 3)
    assert Point3D(1, 2, 3) != Point3D(1, 2, 4)
    assert Point3D(1, 2, 3) != Point(1, 2)


def test_point_repr():
    assert repr(Point(1, 2)) == "Point(x=1, y=2)"


def test_point3d_repr():
    assert repr(Point3D(1, 2, 3)) == "Point3D(x=1, y=2, z=3)"


def test_slotted_instance_uses_less_total_memory_than_the_dict_equivalent():
    # An instance's own getsizeof() can come out equal either way on some CPython
    # builds — the space a dict-based instance actually costs includes the separate
    # __dict__ object it points to.
    point = Point(1, 2)
    dp = DictPoint(1, 2)
    dict_based_total = sys.getsizeof(dp) + sys.getsizeof(dp.__dict__)
    assert sys.getsizeof(point) < dict_based_total


def test_weak_referenceability_differs_by_slots():
    with pytest.raises(TypeError):
        weakref.ref(Point(1, 2))

    dp = DictPoint(1, 2)
    ref = weakref.ref(dp)
    assert ref() is dp
