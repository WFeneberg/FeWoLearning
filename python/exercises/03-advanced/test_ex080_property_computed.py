import pytest

from ex080_property_computed import Rectangle, Report, Temperature, lazy_attribute


def test_rectangle_exposes_its_sides() -> None:
    rectangle = Rectangle(3, 4)

    assert rectangle.width == 3
    assert rectangle.height == 4


def test_rectangle_area_is_computed() -> None:
    assert Rectangle(3, 4).area == 12


def test_rectangle_area_follows_a_side_change() -> None:
    rectangle = Rectangle(3, 4)
    rectangle.width = 10

    # Computed on read, so it cannot go stale.
    assert rectangle.area == 40


def test_rectangle_area_is_read_only() -> None:
    rectangle = Rectangle(3, 4)

    with pytest.raises(AttributeError):
        rectangle.area = 99  # type: ignore[misc]


def test_rectangle_is_square() -> None:
    assert Rectangle(4, 4).is_square is True
    assert Rectangle(4, 5).is_square is False


@pytest.mark.parametrize("value", [0, -1, -0.5])
def test_rectangle_rejects_non_positive_sides(value: float) -> None:
    with pytest.raises(ValueError):
        Rectangle(value, 4)
    with pytest.raises(ValueError):
        Rectangle(4, value)


@pytest.mark.parametrize("value", ["3", None, [3]])
def test_rectangle_rejects_non_numbers(value: object) -> None:
    with pytest.raises(TypeError):
        Rectangle(value, 4)  # type: ignore[arg-type]


def test_rectangle_validates_later_assignment_too() -> None:
    rectangle = Rectangle(3, 4)

    with pytest.raises(ValueError):
        rectangle.height = 0

    assert rectangle.height == 4


def test_temperature_defaults_to_zero() -> None:
    assert Temperature().celsius == 0.0


def test_temperature_converts_to_fahrenheit() -> None:
    assert Temperature(100).fahrenheit == pytest.approx(212.0)
    assert Temperature(-40).fahrenheit == pytest.approx(-40.0)


def test_temperature_converts_to_kelvin() -> None:
    assert Temperature(0).kelvin == pytest.approx(273.15)


def test_setting_fahrenheit_writes_back_to_celsius() -> None:
    temperature = Temperature()
    temperature.fahrenheit = 212

    assert temperature.celsius == pytest.approx(100.0)
    assert temperature.kelvin == pytest.approx(373.15)


def test_kelvin_is_read_only() -> None:
    with pytest.raises(AttributeError):
        Temperature().kelvin = 300  # type: ignore[misc]


def test_temperature_rejects_below_absolute_zero() -> None:
    with pytest.raises(ValueError):
        Temperature(-300)


def test_setting_fahrenheit_reuses_the_celsius_validation() -> None:
    temperature = Temperature()

    with pytest.raises(ValueError):
        temperature.fahrenheit = -500  # about -295 °C

    assert temperature.celsius == 0.0


def test_report_total() -> None:
    assert Report([1, 2, 3]).total == 6


def test_report_total_is_computed_once() -> None:
    report = Report([1, 2, 3])

    assert report.total == 6
    assert report.total == 6
    assert report.total == 6
    assert report._computations == 1


def test_report_total_is_not_computed_until_read() -> None:
    assert Report([1, 2, 3])._computations == 0


def test_report_cache_is_per_instance() -> None:
    first, second = Report([1, 2]), Report([10, 20])

    assert first.total == 3
    assert second.total == 30


def test_report_cache_survives_a_stale_value() -> None:
    report = Report([1, 2, 3])

    assert report.total == 6
    report._values.append(100)

    # This is the cost of caching: the cached answer is now wrong on purpose.
    assert report.total == 6
    assert report._computations == 1


def test_report_invalidate_forces_a_recompute() -> None:
    report = Report([1, 2, 3])

    assert report.total == 6
    report._values.append(100)
    report.invalidate()

    assert report.total == 106
    assert report._computations == 2


def test_report_invalidate_before_any_read_is_a_no_op() -> None:
    report = Report([1, 2, 3])
    report.invalidate()

    assert report.total == 6
    assert report._computations == 1


def build_lazy_class() -> type:
    """Build the owner class inside a test body — `lazy_attribute` runs at class creation."""

    class Sensor:
        def __init__(self) -> None:
            self.reads = 0

        def _reading(self) -> int:
            self.reads += 1
            return 42

        reading = lazy_attribute(_reading)

    return Sensor


def test_lazy_attribute_computes_on_first_read() -> None:
    sensor = build_lazy_class()()

    assert sensor.reading == 42
    assert sensor.reads == 1


def test_lazy_attribute_remembers() -> None:
    sensor = build_lazy_class()()

    assert [sensor.reading, sensor.reading, sensor.reading] == [42, 42, 42]
    assert sensor.reads == 1


def test_lazy_attribute_stores_under_a_private_key() -> None:
    sensor = build_lazy_class()()
    _ = sensor.reading

    assert sensor.__dict__["_lazy__reading"] == 42


def test_lazy_attribute_is_per_instance() -> None:
    sensor_class = build_lazy_class()
    first, second = sensor_class(), sensor_class()

    _ = first.reading

    assert second.reads == 0
    assert second.reading == 42
