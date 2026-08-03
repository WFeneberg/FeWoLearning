"""Exercise 032 — Palindromes (reference solution)."""


def _normalise(text: str) -> str:
    # casefold() is the comparison-oriented sibling of lower(): it maps "ß" to
    # "ss", so "Straße" and "STRASSE" fold to the same thing.
    return "".join(char for char in text.casefold() if char.isalnum())


def is_palindrome(text: str) -> bool:
    return text == text[::-1]


def is_palindrome_loose(text: str) -> bool:
    normalised = _normalise(text)
    return normalised == normalised[::-1]


def is_palindrome_two_pointer(text: str) -> bool:
    left, right = 0, len(text) - 1
    while left < right:
        # Skip anything that does not take part in the comparison.
        if not text[left].isalnum():
            left += 1
            continue
        if not text[right].isalnum():
            right -= 1
            continue
        if text[left].casefold() != text[right].casefold():
            return False
        left += 1
        right -= 1
    return True


def longest_palindromic_prefix(text: str) -> str:
    # Walk from the longest prefix down, so the first hit is the answer.
    for end in range(len(text), 0, -1):
        prefix = text[:end]
        if prefix == prefix[::-1]:
            return prefix
    return ""


def is_list_palindrome(values: list[int]) -> bool:
    return values == values[::-1]


def count_palindromes(words: list[str]) -> int:
    return sum(1 for word in words if _normalise(word) and is_palindrome_loose(word))
