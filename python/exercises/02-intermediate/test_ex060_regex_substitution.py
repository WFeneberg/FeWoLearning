import pytest

from ex060_regex_substitution import (
    collapse_spaces,
    count_and_replace,
    increment_numbers,
    mask_digits,
    replace_first,
    strip_tags,
    swap_names,
    template,
)


@pytest.mark.parametrize(
    "text, expected",
    [
        ("a   b", "a b"),
        ("a\t\nb", "a b"),
        (" a b ", " a b "),
        ("already fine", "already fine"),
        ("", ""),
    ],
)
def test_collapse_spaces(text: str, expected: str) -> None:
    assert collapse_spaces(text) == expected


def test_swap_names() -> None:
    assert swap_names("Lovelace, Ada") == "Ada Lovelace"


def test_swap_names_every_occurrence() -> None:
    assert swap_names("Lovelace, Ada; Hopper, Grace") == "Ada Lovelace; Grace Hopper"


def test_swap_names_leaves_other_text_alone() -> None:
    assert swap_names("no comma here") == "no comma here"


def test_mask_digits() -> None:
    assert mask_digits("card 1234567812345678") == "card ************5678"


def test_mask_digits_keeps_short_numbers() -> None:
    assert mask_digits("pin 123") == "pin 123"


def test_mask_digits_exactly_keep_last() -> None:
    assert mask_digits("x 1234") == "x 1234"


def test_mask_digits_custom_keep() -> None:
    assert mask_digits("1234567", keep_last=2) == "*****67"


def test_mask_digits_several_numbers() -> None:
    assert mask_digits("12345 67890", keep_last=1) == "****5 ****0"


@pytest.mark.parametrize(
    "text, expected",
    [
        ("a1 b9", "a2 b10"),
        ("0", "1"),
        ("no numbers", "no numbers"),
        ("99 bottles", "100 bottles"),
        ("", ""),
    ],
)
def test_increment_numbers(text: str, expected: str) -> None:
    assert increment_numbers(text) == expected


def test_replace_first_only_the_first() -> None:
    assert replace_first("a a a", "a", "b") == "b a a"


def test_replace_first_treats_the_needle_as_literal() -> None:
    # "a.c" must not match "abc" — the dot is literal here.
    assert replace_first("abc a.c", "a.c", "X") == "abc X"


def test_replace_first_with_regex_metacharacters() -> None:
    assert replace_first("cost (1+2)", "(1+2)", "3") == "cost 3"


def test_replace_first_when_absent() -> None:
    assert replace_first("abc", "z", "y") == "abc"


def test_count_and_replace() -> None:
    assert count_and_replace("a1b2c3", r"\d", "#") == ("a#b#c#", 3)


def test_count_and_replace_without_matches() -> None:
    assert count_and_replace("abc", r"\d", "#") == ("abc", 0)


def test_template_substitutes() -> None:
    assert template("Hello {name}!", {"name": "Ada"}) == "Hello Ada!"


def test_template_several_placeholders() -> None:
    result = template("{a}-{b}-{a}", {"a": "1", "b": "2"})

    assert result == "1-2-1"


def test_template_leaves_unknown_placeholders_intact() -> None:
    assert template("Hi {who}", {}) == "Hi {who}"


def test_template_without_placeholders() -> None:
    assert template("plain", {"a": "1"}) == "plain"


@pytest.mark.parametrize(
    "html, expected",
    [
        ("<b>hi</b>", "hi"),
        ("<div><span>x</span></div>", "x"),
        ("no tags", "no tags"),
        ("", ""),
        ("a<br>b", "ab"),
    ],
)
def test_strip_tags(html: str, expected: str) -> None:
    assert strip_tags(html) == expected
