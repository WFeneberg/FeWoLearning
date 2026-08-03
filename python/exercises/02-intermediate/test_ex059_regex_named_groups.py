import pytest

from ex059_regex_named_groups import (
    all_pairs,
    find_first_number,
    first_tag,
    iter_words,
    named_or_positional,
    parse_date,
    parse_log_line,
)


def test_parse_date() -> None:
    assert parse_date("2024-08-03") == {"year": "2024", "month": "08", "day": "03"}


@pytest.mark.parametrize(
    "text",
    ["2024-08-03 extra", "not a date", "", "2024/08/03", "24-08-03"],
)
def test_parse_date_rejects_non_dates(text: str) -> None:
    assert parse_date(text) is None


def test_parse_date_requires_the_whole_string() -> None:
    # A `search` would happily find the date inside this string; fullmatch must not.
    assert parse_date("on 2024-08-03 we shipped") is None


@pytest.mark.parametrize(
    "text, expected",
    [
        ("abc 123 def", "123"),
        ("42", "42"),
        ("a1b22", "1"),
        ("no digits", None),
        ("", None),
    ],
)
def test_find_first_number(text: str, expected: str | None) -> None:
    assert find_first_number(text) == expected


def test_parse_log_line_without_a_module() -> None:
    assert parse_log_line("INFO started up") == {
        "level": "INFO",
        "module": None,
        "message": "started up",
    }


def test_parse_log_line_with_a_module() -> None:
    assert parse_log_line("ERROR [db] connection lost") == {
        "level": "ERROR",
        "module": "db",
        "message": "connection lost",
    }


def test_parse_log_line_absent_group_is_none_not_empty() -> None:
    parsed = parse_log_line("WARN careful")

    assert parsed is not None
    assert parsed["module"] is None


@pytest.mark.parametrize("line", ["", "lowercase message", "INFO", "   "])
def test_parse_log_line_rejects_bad_lines(line: str) -> None:
    assert parse_log_line(line) is None


def test_all_pairs() -> None:
    assert all_pairs("a=1 b=two c=3") == [("a", "1"), ("b", "two"), ("c", "3")]


def test_all_pairs_ignores_surrounding_text() -> None:
    assert all_pairs("prefix x=9 suffix") == [("x", "9")]


def test_all_pairs_without_any() -> None:
    assert all_pairs("nothing here") == []


def test_iter_words_yields_lazily() -> None:
    result = iter_words("one two three")

    assert next(result) == "one"
    assert list(result) == ["two", "three"]


def test_iter_words_ignores_digits_and_punctuation() -> None:
    assert list(iter_words("ab, 12 cd!")) == ["ab", "cd"]


def test_iter_words_empty() -> None:
    assert list(iter_words("")) == []


def test_first_tag_is_non_greedy() -> None:
    assert first_tag("<a><b>") == "a"


def test_first_tag_single() -> None:
    assert first_tag("text <div> more") == "div"


def test_first_tag_without_any() -> None:
    assert first_tag("no tags here") is None


def test_named_or_positional() -> None:
    assert named_or_positional("left:right") == ("left", "right")


def test_named_or_positional_without_a_colon() -> None:
    assert named_or_positional("nocolon") is None
