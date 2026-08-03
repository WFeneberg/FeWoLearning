import pytest

from ex003_string_methods import (
    count_case_insensitive,
    initials,
    mask_email,
    normalize_whitespace,
    slugify,
)


@pytest.mark.parametrize(
    "text, expected",
    [
        ("  a \t b\n c ", "a b c"),
        ("already clean", "already clean"),
        ("   ", ""),
        ("", ""),
        ("one\n\n\ntwo", "one two"),
    ],
)
def test_normalize_whitespace(text: str, expected: str) -> None:
    assert normalize_whitespace(text) == expected


@pytest.mark.parametrize(
    "title, expected",
    [
        ("  Hello   World ", "hello-world"),
        ("Python", "python"),
        ("", ""),
        ("   ", ""),
        ("Mixed CASE Words", "mixed-case-words"),
    ],
)
def test_slugify(title: str, expected: str) -> None:
    assert slugify(title) == expected


@pytest.mark.parametrize(
    "full_name, expected",
    [
        ("ada lovelace", "A.L."),
        ("Grace Brewster Murray Hopper", "G.B.M.H."),
        ("single", "S."),
        ("", ""),
    ],
)
def test_initials(full_name: str, expected: str) -> None:
    assert initials(full_name) == expected


@pytest.mark.parametrize(
    "email, expected",
    [
        ("wolfgang@example.com", "w*******@example.com"),
        ("a@b.com", "a@b.com"),
        ("ab@c.org", "a*@c.org"),
        ("no-at-sign", "no-at-sign"),
    ],
)
def test_mask_email(email: str, expected: str) -> None:
    assert mask_email(email) == expected


@pytest.mark.parametrize(
    "text, needle, expected",
    [
        ("Hello hello HELLO", "hello", 3),
        ("aaa", "aa", 1),
        ("abc", "z", 0),
        ("abc", "", 0),
        ("", "a", 0),
    ],
)
def test_count_case_insensitive(text: str, needle: str, expected: int) -> None:
    assert count_case_insensitive(text, needle) == expected
