"""Exercise 008 — List comprehensions (reference solution)."""


def squares(numbers: list[int]) -> list[int]:
    return [n * n for n in numbers]


def even_squares(numbers: list[int]) -> list[int]:
    return [n * n for n in numbers if n % 2 == 0]


def clamp_all(numbers: list[int], ceiling: int) -> list[int]:
    # A conditional expression in the output slot transforms every item; an `if`
    # clause after the `for` would instead drop the ones that fail it.
    return [ceiling if n > ceiling else n for n in numbers]


def pairs(xs: list[int], ys: list[str]) -> list[tuple[int, str]]:
    # Clause order matches nested for-loops: the leftmost is the outer loop.
    return [(x, y) for x in xs for y in ys]


def flatten_and_filter(rows: list[list[int]], minimum: int) -> list[int]:
    return [value for row in rows for value in row if value >= minimum]


def word_lengths(sentence: str) -> list[tuple[str, int]]:
    return [(word, len(word)) for word in sentence.split()]


def diagonal(matrix: list[list[int]]) -> list[int]:
    return [row[i] for i, row in enumerate(matrix)]
