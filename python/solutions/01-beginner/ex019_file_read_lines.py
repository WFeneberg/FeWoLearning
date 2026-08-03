"""Exercise 019 — Reading text files (reference solution)."""

from pathlib import Path


def read_lines(path: Path) -> list[str]:
    with path.open(encoding="utf-8") as handle:
        # splitlines() drops the separators and, unlike split("\n"), does not
        # invent a trailing empty string for a file ending in a newline.
        return handle.read().splitlines()


def count_lines(path: Path) -> int:
    with path.open(encoding="utf-8") as handle:
        # Iterating the handle streams line by line, so the file is never all in
        # memory at once.
        return sum(1 for _ in handle)


def non_empty_lines(path: Path) -> list[str]:
    with path.open(encoding="utf-8") as handle:
        return [stripped for line in handle if (stripped := line.strip())]


def first_line(path: Path, default: str = "") -> str:
    with path.open(encoding="utf-8") as handle:
        # next() with a default stops after one line instead of reading the rest.
        line = next(handle, None)
    return default if line is None else line.rstrip("\n")


def find_lines(path: Path, needle: str) -> list[tuple[int, str]]:
    with path.open(encoding="utf-8") as handle:
        return [
            (number, line.rstrip("\n"))
            for number, line in enumerate(handle, start=1)
            if needle in line
        ]


def total_of_numbers(path: Path) -> int:
    total = 0
    with path.open(encoding="utf-8") as handle:
        for number, line in enumerate(handle, start=1):
            content = line.strip()
            if not content:
                continue
            try:
                total += int(content)
            except ValueError:
                raise ValueError(f"line {number}: not a number: {content}") from None
    return total
