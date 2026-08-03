from pathlib import Path

import pytest

from ex019_file_read_lines import (
    count_lines,
    find_lines,
    first_line,
    non_empty_lines,
    read_lines,
    total_of_numbers,
)


def write(tmp_path: Path, content: str, name: str = "data.txt") -> Path:
    path = tmp_path / name
    path.write_text(content, encoding="utf-8")
    return path


def test_read_lines_strips_newlines(tmp_path: Path) -> None:
    path = write(tmp_path, "a\nb\nc\n")
    assert read_lines(path) == ["a", "b", "c"]


def test_read_lines_without_a_trailing_newline(tmp_path: Path) -> None:
    path = write(tmp_path, "a\nb")
    assert read_lines(path) == ["a", "b"]


def test_read_lines_keeps_blank_lines_in_the_middle(tmp_path: Path) -> None:
    path = write(tmp_path, "a\n\nb\n")
    assert read_lines(path) == ["a", "", "b"]


def test_read_lines_of_an_empty_file(tmp_path: Path) -> None:
    path = write(tmp_path, "")
    assert read_lines(path) == []


def test_read_lines_handles_non_ascii(tmp_path: Path) -> None:
    path = write(tmp_path, "Grüße\nWölfe\n")
    assert read_lines(path) == ["Grüße", "Wölfe"]


@pytest.mark.parametrize(
    "content, expected",
    [("a\nb\nc\n", 3), ("a\nb", 2), ("", 0), ("\n", 1), ("a\n\n", 2)],
)
def test_count_lines(tmp_path: Path, content: str, expected: int) -> None:
    assert count_lines(write(tmp_path, content)) == expected


def test_non_empty_lines(tmp_path: Path) -> None:
    path = write(tmp_path, "a\n\n  \nb  \n")
    assert non_empty_lines(path) == ["a", "b"]


def test_non_empty_lines_of_an_empty_file(tmp_path: Path) -> None:
    assert non_empty_lines(write(tmp_path, "")) == []


def test_first_line(tmp_path: Path) -> None:
    assert first_line(write(tmp_path, "first\nsecond\n")) == "first"


def test_first_line_of_an_empty_file(tmp_path: Path) -> None:
    assert first_line(write(tmp_path, "")) == ""


def test_first_line_custom_default(tmp_path: Path) -> None:
    assert first_line(write(tmp_path, ""), "(none)") == "(none)"


def test_find_lines(tmp_path: Path) -> None:
    path = write(tmp_path, "alpha\nbeta\ngamma beta\n")
    assert find_lines(path, "beta") == [(2, "beta"), (3, "gamma beta")]


def test_find_lines_is_case_sensitive(tmp_path: Path) -> None:
    path = write(tmp_path, "Beta\nbeta\n")
    assert find_lines(path, "beta") == [(2, "beta")]


def test_find_lines_without_matches(tmp_path: Path) -> None:
    assert find_lines(write(tmp_path, "a\n"), "zzz") == []


def test_total_of_numbers(tmp_path: Path) -> None:
    assert total_of_numbers(write(tmp_path, "1\n2\n3\n")) == 6


def test_total_of_numbers_ignores_blank_lines(tmp_path: Path) -> None:
    assert total_of_numbers(write(tmp_path, "1\n\n2\n  \n")) == 3


def test_total_of_numbers_of_an_empty_file(tmp_path: Path) -> None:
    assert total_of_numbers(write(tmp_path, "")) == 0


def test_total_of_numbers_reports_the_offending_line(tmp_path: Path) -> None:
    path = write(tmp_path, "1\nnope\n")
    with pytest.raises(ValueError, match=r"^line 2: not a number: nope$"):
        total_of_numbers(path)
