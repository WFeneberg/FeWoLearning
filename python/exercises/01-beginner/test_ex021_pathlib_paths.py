from pathlib import Path

import pytest

from ex021_pathlib_paths import (
    change_extension,
    ensure_dir,
    find_by_suffix,
    join,
    list_files,
    relative,
    split_name,
)


def test_join() -> None:
    assert join(Path("base"), "a", "b.txt") == Path("base/a/b.txt")


def test_join_with_no_parts_returns_the_base() -> None:
    assert join(Path("base")) == Path("base")


def test_split_name_peels_one_extension() -> None:
    parent, stem, suffix = split_name(Path("data/report.tar.gz"))

    assert stem == "report.tar"
    assert suffix == ".gz"
    assert parent == str(Path("data"))


def test_split_name_without_a_suffix() -> None:
    _, stem, suffix = split_name(Path("data/README"))

    assert stem == "README"
    assert suffix == ""


@pytest.mark.parametrize(
    "name, suffix, expected",
    [
        ("a.txt", ".md", "a.md"),
        ("a.txt", "md", "a.md"),
        ("a.txt", "", "a"),
        ("a", ".log", "a.log"),
        ("a.tar.gz", ".zip", "a.tar.zip"),
    ],
)
def test_change_extension(name: str, suffix: str, expected: str) -> None:
    assert change_extension(Path(name), suffix) == Path(expected)


def test_relative() -> None:
    assert relative(Path("/tmp/base/a/b.txt"), Path("/tmp/base")) == Path("a/b.txt")


def test_relative_of_the_base_itself() -> None:
    assert relative(Path("/tmp/base"), Path("/tmp/base")) == Path(".")


def test_relative_rejects_an_unrelated_path() -> None:
    with pytest.raises(ValueError):
        relative(Path("/other/x.txt"), Path("/tmp/base"))


def test_list_files_sorted_and_without_directories(tmp_path: Path) -> None:
    (tmp_path / "b.txt").write_text("", encoding="utf-8")
    (tmp_path / "a.txt").write_text("", encoding="utf-8")
    (tmp_path / "sub").mkdir()

    assert list_files(tmp_path) == ["a.txt", "b.txt"]


def test_list_files_of_an_empty_directory(tmp_path: Path) -> None:
    assert list_files(tmp_path) == []


def test_list_files_is_not_recursive(tmp_path: Path) -> None:
    sub = tmp_path / "sub"
    sub.mkdir()
    (sub / "deep.txt").write_text("", encoding="utf-8")
    (tmp_path / "top.txt").write_text("", encoding="utf-8")

    assert list_files(tmp_path) == ["top.txt"]


def test_find_by_suffix_searches_recursively(tmp_path: Path) -> None:
    (tmp_path / "a.txt").write_text("", encoding="utf-8")
    sub = tmp_path / "sub"
    sub.mkdir()
    (sub / "b.txt").write_text("", encoding="utf-8")
    (sub / "c.md").write_text("", encoding="utf-8")

    assert find_by_suffix(tmp_path, ".txt") == ["a.txt", "b.txt"]


def test_find_by_suffix_without_matches(tmp_path: Path) -> None:
    (tmp_path / "a.md").write_text("", encoding="utf-8")

    assert find_by_suffix(tmp_path, ".txt") == []


def test_ensure_dir_creates_parents(tmp_path: Path) -> None:
    target = tmp_path / "a" / "b"

    result = ensure_dir(target)

    assert result == target
    assert target.is_dir()


def test_ensure_dir_is_idempotent(tmp_path: Path) -> None:
    target = tmp_path / "a"
    ensure_dir(target)

    # A second call must not raise FileExistsError.
    assert ensure_dir(target).is_dir()
