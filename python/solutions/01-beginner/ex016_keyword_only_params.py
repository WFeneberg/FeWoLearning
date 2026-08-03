"""Exercise 016 — Keyword-only and positional-only parameters (reference solution)."""


def connect(host: str, *, port: int = 5432, timeout: float = 5.0) -> str:
    # Everything after the bare * can only be passed by keyword.
    return f"{host}:{port} (timeout={timeout}s)"


def divide(a: float, b: float, /) -> float:
    # Everything before the / can only be passed positionally.
    return a / b


def clamp(value: int, /, low: int = 0, *, high: int = 100) -> int:
    if low > high:
        raise ValueError("clamp() low must not exceed high")
    return max(low, min(value, high))


def make_url(
    scheme: str, host: str, *, path: str = "/", query: dict[str, str] | None = None
) -> str:
    url = f"{scheme}://{host}{path}"
    if query:
        url += "?" + "&".join(f"{key}={value}" for key, value in query.items())
    return url


def rename(mapping: dict[str, int], /, **renames: str) -> dict[str, int]:
    # Because `mapping` is positional-only, a **renames key named "mapping" does
    # not collide with the parameter.
    return {renames.get(key, key): value for key, value in mapping.items()}
