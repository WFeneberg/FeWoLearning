"""Exercise 027 — min and max with a key (reference solution)."""


def longest(words: list[str]) -> str | None:
    # max() keeps the first item on a tie, and default= covers the empty case
    # without a length check.
    return max(words, key=len, default=None)


def shortest(words: list[str]) -> str | None:
    return min(words, key=len, default=None)


def closest_to(numbers: list[int], target: int) -> int | None:
    return min(numbers, key=lambda n: abs(n - target), default=None)


def highest_scorer(scores: dict[str, int]) -> str | None:
    # Iterating .items() lets the score be the key while the *name* is returned.
    best = max(scores.items(), key=lambda item: item[1], default=None)
    return None if best is None else best[0]


def largest_by_abs(numbers: list[int], default: int = 0) -> int:
    return max(numbers, key=abs, default=default)


def bounds(numbers: list[int]) -> tuple[int, int] | None:
    if not numbers:
        return None
    return min(numbers), max(numbers)


def longest_line(text: str) -> str:
    # splitlines() leaves no phantom empty line for a trailing newline.
    return max(text.splitlines(), key=len, default="")
