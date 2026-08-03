"""Exercise 003 — String methods (beginner).

Goal:   Clean up and reshape text with the built-in str methods.
Drills: split/join/strip/replace, case folding, str.title.
Passes: when `pytest exercises/01-beginner/test_ex003_string_methods.py` is green.
"""


def normalize_whitespace(text: str) -> str:
    """Collapse every run of whitespace into a single space and strip the ends.

    ``normalize_whitespace("  a \\t b\\n c ")`` -> ``"a b c"``.
    """
    raise NotImplementedError


def slugify(title: str) -> str:
    """Turn a title into a lowercase hyphen-separated slug.

    Words are whitespace-separated; every other character is kept as-is.
    ``slugify("  Hello   World ")`` -> ``"hello-world"``. Empty input yields ``""``.
    """
    raise NotImplementedError


def initials(full_name: str) -> str:
    """Return the uppercase initials of each whitespace-separated word, dot-joined.

    ``initials("ada lovelace")`` -> ``"A.L."``. Empty input yields ``""``.
    """
    raise NotImplementedError


def mask_email(email: str) -> str:
    """Replace all but the first character of the local part with asterisks.

    ``mask_email("wolfgang@example.com")`` -> ``"w*******@example.com"``.
    A local part of a single character stays unmasked. A string without "@" is
    returned unchanged.
    """
    raise NotImplementedError


def count_case_insensitive(text: str, needle: str) -> int:
    """Count non-overlapping occurrences of `needle` in `text`, ignoring case.

    An empty needle counts as 0 rather than raising.
    """
    raise NotImplementedError
