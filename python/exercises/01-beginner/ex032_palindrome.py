"""Exercise 032 — Palindromes (beginner).

Goal:   Compare a sequence with its reverse after normalising it.
Drills: normalisation before comparison, slicing with a negative step, str vs list
        reversal, two-pointer scanning, Unicode case folding.
Passes: when `pytest exercises/01-beginner/test_ex032_palindrome.py` is green.
"""


def is_palindrome(text: str) -> bool:
    """Report whether `text` reads the same backwards.

    Compares exactly as given: case and spaces matter.
    """
    raise NotImplementedError


def is_palindrome_loose(text: str) -> bool:
    """Report whether `text` is a palindrome ignoring case and non-alphanumerics.

    ``"A man, a plan, a canal: Panama"`` is True. Use ``casefold``, not ``lower``,
    so "Straße"/"STRASSE" fold alike.
    """
    raise NotImplementedError


def is_palindrome_two_pointer(text: str) -> bool:
    """Like `is_palindrome_loose`, but scanning from both ends inwards.

    Walk two indices towards each other, skipping non-alphanumerics, rather than
    building a normalised copy — no extra allocation, and it can stop early.

    Caveat worth knowing: comparing one character at a time cannot reproduce
    multi-character case folding. ``"ß".casefold()`` is ``"ss"``, so this function
    and `is_palindrome_loose` disagree on inputs containing it.
    """
    raise NotImplementedError


def longest_palindromic_prefix(text: str) -> str:
    """Return the longest prefix of `text` that is a palindrome, compared exactly.

    A single character is a palindrome, so a non-empty string always yields at least
    one character. Empty input yields ``""``.
    """
    raise NotImplementedError


def is_list_palindrome(values: list[int]) -> bool:
    """Report whether the list reads the same backwards."""
    raise NotImplementedError


def count_palindromes(words: list[str]) -> int:
    """Count how many words are palindromes, ignoring case and punctuation.

    Words that normalise to nothing (e.g. "!!") do not count.
    """
    raise NotImplementedError
