from pathlib import Path

import pytest

from ex020_file_write_text import (
    append_line,
    round_trip,
    write_csv_ish,
    write_in_new_dir,
    write_lines,
    write_once,
)


def test_write_lines_creates_the_file(tmp_path: Path) -> None:
    path = tmp_path / "out.txt"

    written = write_lines(path, ["a", "b"])

    assert path.read_text(encoding="utf-8") == "a\nb\n"
    assert written == 4


def test_write_lines_replaces_existing_content(tmp_path: Path) -> None:
    path = tmp_path / "out.txt"
    path.write_text("old content\n", encoding="utf-8")

    write_lines(path, ["new"])

    assert path.read_text(encoding="utf-8") == "new\n"


def test_write_lines_with_no_lines(tmp_path: Path) -> None:
    path = tmp_path / "out.txt"

    assert write_lines(path, []) == 0
    assert path.read_text(encoding="utf-8") == ""


def test_write_lines_uses_utf8(tmp_path: Path) -> None:
    path = tmp_path / "out.txt"

    write_lines(path, ["Grüße"])

    assert path.read_bytes() == "Grüße\n".encode("utf-8")


def test_append_line_creates_the_file(tmp_path: Path) -> None:
    path = tmp_path / "log.txt"

    append_line(path, "first")

    assert path.read_text(encoding="utf-8") == "first\n"


def test_append_line_keeps_existing_content(tmp_path: Path) -> None:
    path = tmp_path / "log.txt"
    append_line(path, "first")
    append_line(path, "second")

    assert path.read_text(encoding="utf-8") == "first\nsecond\n"


def test_write_once_writes(tmp_path: Path) -> None:
    path = tmp_path / "new.txt"

    write_once(path, "hello")

    assert path.read_text(encoding="utf-8") == "hello"


def test_write_once_refuses_to_overwrite(tmp_path: Path) -> None:
    path = tmp_path / "new.txt"
    path.write_text("keep me", encoding="utf-8")

    with pytest.raises(FileExistsError):
        write_once(path, "nope")

    assert path.read_text(encoding="utf-8") == "keep me"


def test_write_in_new_dir_creates_parents(tmp_path: Path) -> None:
    path = tmp_path / "a" / "b" / "c.txt"

    write_in_new_dir(path, "deep")

    assert path.read_text(encoding="utf-8") == "deep"


def test_write_in_new_dir_works_when_the_dir_exists(tmp_path: Path) -> None:
    path = tmp_path / "here.txt"

    write_in_new_dir(path, "flat")

    assert path.read_text(encoding="utf-8") == "flat"


def test_write_csv_ish(tmp_path: Path) -> None:
    path = tmp_path / "data.csv"

    write_csv_ish(path, [("ada", 1), ("grace", 2)])

    assert path.read_text(encoding="utf-8") == "name,value\nada,1\ngrace,2\n"


def test_write_csv_ish_with_no_rows(tmp_path: Path) -> None:
    path = tmp_path / "data.csv"

    write_csv_ish(path, [])

    assert path.read_text(encoding="utf-8") == "name,value\n"


def test_round_trip(tmp_path: Path) -> None:
    path = tmp_path / "rt.txt"

    assert round_trip(path, ["a", "b", "c"]) == ["a", "b", "c"]


def test_round_trip_preserves_blank_lines_and_unicode(tmp_path: Path) -> None:
    path = tmp_path / "rt.txt"

    assert round_trip(path, ["a", "", "Grüße"]) == ["a", "", "Grüße"]


def test_round_trip_empty(tmp_path: Path) -> None:
    assert round_trip(tmp_path / "rt.txt", []) == []
