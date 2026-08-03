from typing import Any

import pytest

from ex064_custom_exception_hierarchy import (
    AppError,
    MissingFieldError,
    NotFoundError,
    ValidationError,
    classify,
    lookup,
    require_field,
    validate_age,
)


def test_hierarchy_is_wired_up() -> None:
    assert issubclass(ValidationError, AppError)
    assert issubclass(MissingFieldError, ValidationError)
    assert issubclass(NotFoundError, AppError)


def test_validation_error_message_and_attributes() -> None:
    error = ValidationError("age", "abc")

    assert str(error) == "invalid age: abc"
    assert error.field == "age"
    assert error.value == "abc"


def test_missing_field_error_message_and_attributes() -> None:
    error = MissingFieldError("email")

    assert str(error) == "missing field: email"
    assert error.field == "email"
    assert error.value is None


def test_not_found_error_message_and_attribute() -> None:
    error = NotFoundError("user:1")

    assert str(error) == "not found: user:1"
    assert error.key == "user:1"


def test_catching_the_base_catches_a_subclass() -> None:
    with pytest.raises(AppError):
        raise MissingFieldError("x")


def test_catching_validation_error_catches_missing_field() -> None:
    with pytest.raises(ValidationError):
        raise MissingFieldError("x")


@pytest.mark.parametrize("value", ["0", 0, "150", 150, 42])
def test_validate_age_accepts_valid_values(value: Any) -> None:
    assert validate_age(value) == int(value)


def test_validate_age_non_numeric_keeps_the_cause() -> None:
    with pytest.raises(ValidationError) as info:
        validate_age("abc")

    assert info.value.field == "age"
    # The underlying ValueError explains *why* the conversion failed.
    assert info.value.__cause__ is not None


@pytest.mark.parametrize("value", [-1, 151, 1000])
def test_validate_age_out_of_range_has_no_cause(value: int) -> None:
    with pytest.raises(ValidationError) as info:
        validate_age(value)

    # Nothing underlying failed, so pointing at a cause would be misleading.
    assert info.value.__cause__ is None


def test_require_field_returns_the_value() -> None:
    assert require_field({"a": 1}, "a") == 1


def test_require_field_raises_missing_field_error() -> None:
    with pytest.raises(MissingFieldError) as info:
        require_field({}, "email")

    assert info.value.field == "email"


def test_require_field_suppresses_the_key_error() -> None:
    with pytest.raises(MissingFieldError) as info:
        require_field({}, "email")

    assert info.value.__cause__ is None
    # `from None` also clears the implicit context, not just the cause.
    assert info.value.__suppress_context__ is True


def test_lookup_returns_the_value() -> None:
    assert lookup({"a": 1}, "a") == 1


def test_lookup_keeps_the_key_error_as_the_cause() -> None:
    with pytest.raises(NotFoundError) as info:
        lookup({}, "missing")

    assert isinstance(info.value.__cause__, KeyError)


@pytest.mark.parametrize(
    "factory, expected",
    [
        (lambda: MissingFieldError("f"), "missing"),
        (lambda: ValidationError("f", 1), "validation"),
        (lambda: NotFoundError("k"), "notfound"),
        (lambda: AppError("x"), "app"),
        (lambda: ValueError("x"), "other"),
        (lambda: TypeError("x"), "other"),
    ],
)
def test_classify(factory: Any, expected: str) -> None:
    assert classify(factory()) == expected
