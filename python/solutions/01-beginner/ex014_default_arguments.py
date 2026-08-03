"""Exercise 014 — Default arguments (reference solution)."""


def greet(name: str, greeting: str = "Hello") -> str:
    return f"{greeting}, {name}!"


def append_item(item: int, target: list[int] | None = None) -> list[int]:
    # None as the sentinel, so a fresh list is built per call. A literal `[]` in
    # the signature would be created once at definition time and shared forever.
    if target is None:
        target = []
    target.append(item)
    return target


def build_config(overrides: dict[str, str] | None = None) -> dict[str, str]:
    config = {"host": "localhost", "port": "8080"}
    if overrides:
        # Copying into a fresh dict leaves the caller's mapping untouched.
        config.update(overrides)
    return config


def repeat(text: str, times: int = 2, separator: str = " ") -> str:
    if times <= 0:
        return ""
    return separator.join([text] * times)


def slice_window(values: list[int], start: int = 0, length: int | None = None) -> list[int]:
    # A default that depends on another argument has to be resolved here: the
    # signature is evaluated before `values` exists.
    if length is None:
        return values[start:]
    return values[start : start + length]


def counter_factory(start: int = 0) -> tuple[list[int], int]:
    return [], start
