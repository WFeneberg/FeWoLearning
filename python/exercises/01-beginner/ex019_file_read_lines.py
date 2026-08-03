"""Exercise 019 — Reading text files (beginner).

Goal:   Read files with a context manager, never leaking a handle.
Drills: open() in a `with` block, iterating lines lazily, stripping newlines,
        encoding, counting without loading the whole file.
Passes: when `pytest exercises/01-beginner/test_ex019_file_read_lines.py` is green.
"""

from pathlib import Path


def read_lines(path: Path) -> list[str]:
    """Return the file's lines with their trailing newline removed.

    A trailing newline at the end of the file does **not** produce an extra empty
    line; a blank line in the middle does produce an empty string.
    """
    raise NotImplementedError


def count_lines(path: Path) -> int:
    """Return the number of lines, without building a list of them all."""
    raise NotImplementedError


def non_empty_lines(path: Path) -> list[str]:
    """Return the lines that contain something other than whitespace, stripped."""
    raise NotImplementedError


def first_line(path: Path, default: str = "") -> str:
    """Return the first line without its newline, or `default` for an empty file.

    Must not read the whole file.
    """
    raise NotImplementedError


def find_lines(path: Path, needle: str) -> list[tuple[int, str]]:
    """Return ``(line_number, line)`` for lines containing `needle`, 1-based.

    Matching is case-sensitive and lines are returned without their newline.
    """
    raise NotImplementedError


def total_of_numbers(path: Path) -> int:
    """Sum one integer per line, ignoring blank lines.

    A line that is not an integer raises ValueError whose message is
    ``"line <n>: not a number: <content>"``.
    """
    raise NotImplementedError
