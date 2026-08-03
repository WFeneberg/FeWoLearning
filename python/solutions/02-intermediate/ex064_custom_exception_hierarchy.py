"""Exercise 064 — Custom exception hierarchies (reference solution)."""

from typing import Any


class AppError(Exception):
    """Base for every error this module raises."""


class ValidationError(AppError):
    def __init__(self, field: str, value: Any) -> None:
        # Passing the message up to Exception.__init__ is what makes str(error) work.
        super().__init__(f"invalid {field}: {value}")
        self.field = field
        self.value = value


class MissingFieldError(ValidationError):
    def __init__(self, field: str) -> None:
        # Skipping ValidationError.__init__ deliberately: the message differs, but the
        # attributes it would have set are still established here.
        AppError.__init__(self, f"missing field: {field}")
        self.field = field
        self.value = None


class NotFoundError(AppError):
    def __init__(self, key: str) -> None:
        super().__init__(f"not found: {key}")
        self.key = key


def validate_age(value: Any) -> int:
    try:
        age = int(value)
    except (TypeError, ValueError) as error:
        # The conversion error explains why this failed, so keep it as the cause.
        raise ValidationError("age", value) from error

    if not 0 <= age <= 150:
        # Nothing underlying failed here, so `from None` avoids pointing at an
        # unrelated context.
        raise ValidationError("age", value) from None
    return age


def require_field(data: dict[str, Any], field: str) -> Any:
    try:
        return data[field]
    except KeyError:
        # The KeyError says nothing the caller could act on beyond the field name,
        # which the new exception already carries.
        raise MissingFieldError(field) from None


def lookup(store: dict[str, int], key: str) -> int:
    try:
        return store[key]
    except KeyError as error:
        raise NotFoundError(key) from error


def classify(error: Exception) -> str:
    # Most specific first: MissingFieldError is a ValidationError, so checking
    # ValidationError earlier would shadow it.
    if isinstance(error, MissingFieldError):
        return "missing"
    if isinstance(error, ValidationError):
        return "validation"
    if isinstance(error, NotFoundError):
        return "notfound"
    if isinstance(error, AppError):
        return "app"
    return "other"
