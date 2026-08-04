"""Exercise 079 — descriptors for validation (reference solution)."""

from typing import Any


class Validated:
    # Declared here so the class is honest about the attributes __set_name__ installs.
    public_name: str
    private_name: str

    def __set_name__(self, owner: type, name: str) -> None:
        # Python calls this once, while the owner class is being created, which is how a
        # descriptor learns the name it was bound to without being told twice.
        self.public_name = name
        self.private_name = f"_{name}"

    def __get__(self, instance: Any, owner: type | None = None) -> Any:
        if instance is None:
            # Class-level access: hand back the descriptor so it stays introspectable.
            return self
        try:
            return instance.__dict__[self.private_name]
        except KeyError:
            raise AttributeError(f"{self.public_name!r} has not been set") from None

    def __set__(self, instance: Any, value: Any) -> None:
        # The value goes in the *instance* dict. Assigning to `self` here would make one
        # descriptor object the single storage slot for every instance of the class.
        instance.__dict__[self.private_name] = self.validate(value)

    def validate(self, value: Any) -> Any:
        raise NotImplementedError("Validated subclasses must implement validate()")


class Positive(Validated):
    def validate(self, value: Any) -> Any:
        # bool is a subclass of int, so `isinstance(True, int)` is True — reject it first
        # or `Product("x", True, "tools")` would sail through as price 1.
        if isinstance(value, bool) or not isinstance(value, (int, float)):
            raise TypeError(f"{self.public_name} must be a number, got {type(value).__name__}")
        if value <= 0:
            raise ValueError(f"{self.public_name} must be greater than zero, got {value!r}")
        return value


class NonEmptyString(Validated):
    def validate(self, value: Any) -> Any:
        if not isinstance(value, str):
            raise TypeError(f"{self.public_name} must be a string, got {type(value).__name__}")
        stripped = value.strip()
        if not stripped:
            raise ValueError(f"{self.public_name} must not be blank")
        # validate() returns what to store, so normalisation belongs here.
        return stripped


class OneOf(Validated):
    def __init__(self, *options: Any) -> None:
        if not options:
            raise ValueError("OneOf() requires at least one option")
        self.options = options

    def validate(self, value: Any) -> Any:
        if value not in self.options:
            allowed = ", ".join(repr(option) for option in self.options)
            raise ValueError(f"{self.public_name} must be one of {allowed}, got {value!r}")
        return value


def storage_keys(instance: Any) -> list[str]:
    return sorted(vars(instance))
