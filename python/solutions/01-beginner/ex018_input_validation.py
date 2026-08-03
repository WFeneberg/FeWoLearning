"""Exercise 018 — Input validation (reference solution)."""


def require_positive(value: int, name: str = "value") -> int:
    if value <= 0:
        raise ValueError(f"{name} must be positive, got {value}")
    return value


def require_in_range(value: int, low: int, high: int) -> int:
    # The range itself is validated first: a broken range is a caller bug, and
    # reporting it beats reporting a value that only looks wrong because of it.
    if low > high:
        raise ValueError(f"invalid range: {low} > {high}")
    if not low <= value <= high:
        raise ValueError(f"value must be between {low} and {high}, got {value}")
    return value


def require_str(value: object) -> str:
    if not isinstance(value, str):
        # Wrong type -> TypeError. Right type, wrong value -> ValueError.
        raise TypeError(f"expected str, got {type(value).__name__}")
    return value


def require_non_empty(values: list[int]) -> list[int]:
    if not values:
        raise ValueError("must not be empty")
    return values


def parse_age(text: str) -> int:
    try:
        age = int(text)
    except ValueError:
        raise ValueError(f"not a number: {text}") from None
    if not 0 <= age <= 150:
        raise ValueError(f"age out of range: {age}")
    return age


def average(values: list[float]) -> float:
    if not values:
        # Guarding beats letting a ZeroDivisionError leak out of the arithmetic:
        # the caller learns what they did wrong, not how it broke.
        raise ValueError("cannot average an empty list")
    return sum(values) / len(values)
