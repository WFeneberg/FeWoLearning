"""Exercise 017 — Exception handling (reference solution)."""

from typing import Any, Callable


def safe_int(text: str, default: int = 0) -> int:
    try:
        return int(text)
    except ValueError:
        # Deliberately narrow: int(None) raises TypeError and should propagate.
        return default


def safe_divide(a: float, b: float) -> float | None:
    try:
        return a / b
    except ZeroDivisionError:
        return None


def first_int(values: list[str]) -> int | None:
    for value in values:
        try:
            return int(value)
        except ValueError:
            continue
    return None


def parse_pair(text: str) -> tuple[int, int]:
    try:
        left, right = text.split(",")
        return int(left), int(right)
    except ValueError as error:
        # `from error` keeps the underlying cause visible in the traceback instead
        # of hiding why the parse failed.
        raise ValueError(f"invalid pair: {text}") from error


def run_with_cleanup(action: Callable[[], Any], log: list[str]) -> str:
    try:
        result = action()
    except Exception:
        log.append("error")
        # A bare raise re-raises the original exception with its traceback intact.
        raise
    else:
        # `else` runs only when no exception was raised, so "ok" cannot be logged
        # for a failing call.
        log.append("ok")
        return str(result)
    finally:
        # Runs on both paths, after the except/else block.
        log.append("cleanup")


def lookup_or_raise(mapping: dict[str, int], key: str) -> int:
    try:
        return mapping[key]
    except KeyError as error:
        raise KeyError(f"unknown key: {key}") from error
