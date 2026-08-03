import pytest

from ex032_palindrome import (
    count_palindromes,
    is_list_palindrome,
    is_palindrome,
    is_palindrome_loose,
    is_palindrome_two_pointer,
    longest_palindromic_prefix,
)


@pytest.mark.parametrize(
    "text, expected",
    [
        ("racecar", True),
        ("abba", True),
        ("a", True),
        ("", True),
        ("abc", False),
        ("Racecar", False),
        ("race car", False),
    ],
)
def test_is_palindrome_is_exact(text: str, expected: bool) -> None:
    assert is_palindrome(text) is expected


LOOSE_CASES = [
    ("A man, a plan, a canal: Panama", True),
    ("racecar", True),
    ("Racecar", True),
    ("race car", True),
    ("No 'x' in Nixon", True),
    ("hello", False),
    ("", True),
    ("!!", True),
    ("ab", False),
    ("12321", True),
    ("1231", False),
]


@pytest.mark.parametrize("text, expected", LOOSE_CASES)
def test_is_palindrome_loose(text: str, expected: bool) -> None:
    assert is_palindrome_loose(text) is expected


@pytest.mark.parametrize("text, expected", LOOSE_CASES)
def test_is_palindrome_two_pointer_agrees(text: str, expected: bool) -> None:
    assert is_palindrome_two_pointer(text) is expected


def test_case_folding_handles_the_german_sharp_s() -> None:
    # casefold() maps "ß" to "ss"; lower() would not.
    assert is_palindrome_loose("Straße esSarts".replace(" ", "")) is True


@pytest.mark.parametrize(
    "text, expected",
    [
        ("racecarx", "racecar"),
        ("abba!", "abba"),
        ("abc", "a"),
        ("aa", "aa"),
        ("", ""),
        ("xy", "x"),
    ],
)
def test_longest_palindromic_prefix(text: str, expected: str) -> None:
    assert longest_palindromic_prefix(text) == expected


@pytest.mark.parametrize(
    "values, expected",
    [
        ([1, 2, 1], True),
        ([1, 1], True),
        ([1], True),
        ([], True),
        ([1, 2], False),
        ([1, 2, 3, 2, 1], True),
    ],
)
def test_is_list_palindrome(values: list[int], expected: bool) -> None:
    assert is_list_palindrome(values) is expected


def test_count_palindromes() -> None:
    words = ["racecar", "hello", "Abba", "no", "Otto"]
    assert count_palindromes(words) == 3


def test_count_palindromes_ignores_words_that_normalise_to_nothing() -> None:
    assert count_palindromes(["!!", "...", "aba"]) == 1


def test_count_palindromes_empty() -> None:
    assert count_palindromes([]) == 0
