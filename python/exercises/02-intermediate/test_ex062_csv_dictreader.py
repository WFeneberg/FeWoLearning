from pathlib import Path
from typing import Any

import pytest

from ex062_csv_dictreader import (
    column_values,
    iter_rows,
    read_custom,
    read_rows,
    read_rows_typed,
    round_trip,
    write_rows,
)


def write(tmp_path: Path, content: str, name: str = "data.csv") -> Path:
    path = tmp_path / name
    path.write_text(content, encoding="utf-8", newline="")
    return path


def test_read_rows(tmp_path: Path) -> None:
    path = write(tmp_path, "name,age\nada,36\ngrace,45\n")

    assert read_rows(path) == [
        {"name": "ada", "age": "36"},
        {"name": "grace", "age": "45"},
    ]


def test_read_rows_header_only(tmp_path: Path) -> None:
    assert read_rows(write(tmp_path, "name,age\n")) == []


def test_read_rows_handles_an_embedded_comma(tmp_path: Path) -> None:
    path = write(tmp_path, 'name,note\nada,"hello, world"\n')

    assert read_rows(path) == [{"name": "ada", "note": "hello, world"}]


def test_read_rows_handles_an_embedded_newline(tmp_path: Path) -> None:
    path = write(tmp_path, 'name,note\nada,"line1\nline2"\n')

    assert read_rows(path) == [{"name": "ada", "note": "line1\nline2"}]


def test_read_rows_handles_an_escaped_quote(tmp_path: Path) -> None:
    path = write(tmp_path, 'name,note\nada,"say ""hi"""\n')

    assert read_rows(path) == [{"name": "ada", "note": 'say "hi"'}]


def test_read_rows_typed(tmp_path: Path) -> None:
    path = write(tmp_path, "name,age,active\nada,36,true\ngrace,45,no\n")

    assert read_rows_typed(path) == [
        {"name": "ada", "age": 36, "active": True},
        {"name": "grace", "age": 45, "active": False},
    ]


@pytest.mark.parametrize("raw, expected", [("TRUE", True), ("1", True), ("Yes", True), ("0", False), ("false", False)])
def test_read_rows_typed_boolean_spellings(tmp_path: Path, raw: str, expected: bool) -> None:
    path = write(tmp_path, f"name,age,active\nx,1,{raw}\n")

    assert read_rows_typed(path)[0]["active"] is expected


def test_write_rows(tmp_path: Path) -> None:
    path = tmp_path / "out.csv"

    write_rows(path, [{"a": "1", "b": "2"}], ["a", "b"])

    assert path.read_text(encoding="utf-8", newline="") == "a,b\r\n1,2\r\n"


def test_write_rows_header_only(tmp_path: Path) -> None:
    path = tmp_path / "out.csv"

    write_rows(path, [], ["a", "b"])

    assert path.read_text(encoding="utf-8", newline="") == "a,b\r\n"


def test_write_rows_quotes_only_where_needed(tmp_path: Path) -> None:
    path = tmp_path / "out.csv"

    write_rows(path, [{"a": "plain", "b": "has,comma"}], ["a", "b"])

    assert path.read_text(encoding="utf-8", newline="") == 'a,b\r\nplain,"has,comma"\r\n'


def test_read_custom_delimiter(tmp_path: Path) -> None:
    path = write(tmp_path, "name;age\nada;36\n")

    assert read_custom(path, ";") == [{"name": "ada", "age": "36"}]


def test_read_custom_tab(tmp_path: Path) -> None:
    path = write(tmp_path, "name\tage\nada\t36\n")

    assert read_custom(path, "\t") == [{"name": "ada", "age": "36"}]


def test_iter_rows_is_lazy(tmp_path: Path) -> None:
    path = write(tmp_path, "n\n1\n2\n3\n")

    result = iter_rows(path)

    assert next(result) == {"n": "1"}
    assert list(result) == [{"n": "2"}, {"n": "3"}]


def test_iter_rows_closes_the_file(tmp_path: Path) -> None:
    path = write(tmp_path, "n\n1\n")

    # Fully consuming the generator must release the handle, so the file can be
    # replaced on Windows, which refuses to unlink an open file.
    assert list(iter_rows(path)) == [{"n": "1"}]
    path.unlink()


def test_column_values(tmp_path: Path) -> None:
    path = write(tmp_path, "name,age\nada,36\ngrace,45\n")

    assert column_values(path, "age") == ["36", "45"]


def test_column_values_unknown_column_raises(tmp_path: Path) -> None:
    path = write(tmp_path, "name\nada\n")

    with pytest.raises(KeyError):
        column_values(path, "nope")


def test_round_trip_survives_awkward_values(tmp_path: Path) -> None:
    rows = [
        {"a": "has,comma", "b": 'has "quotes"'},
        {"a": "has\nnewline", "b": "plain"},
    ]

    assert round_trip(tmp_path / "rt.csv", rows, ["a", "b"]) == rows


def test_round_trip_empty(tmp_path: Path) -> None:
    assert round_trip(tmp_path / "rt.csv", [], ["a"]) == []
