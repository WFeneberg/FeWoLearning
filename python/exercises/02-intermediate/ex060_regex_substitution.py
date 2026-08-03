"""Exercise 060 — Regular expression substitution (intermediate).

Goal:   Rewrite text with a pattern, including replacements that need to compute.
Drills: re.sub with a template, backreferences, re.sub with a *function* replacement,
        count limits, re.subn, and escaping so user input cannot inject a pattern.
Passes: when `pytest exercises/02-intermediate/test_ex060_regex_substitution.py` is green.
"""

from typing import Callable


def collapse_spaces(text: str) -> str:
    """Replace every run of whitespace with a single space. Ends are not stripped."""
    raise NotImplementedError


def swap_names(text: str) -> str:
    """Rewrite ``"Last, First"`` as ``"First Last"`` using backreferences.

    Applies to every occurrence in the string.
    """
    raise NotImplementedError


def mask_digits(text: str, keep_last: int = 4) -> str:
    """Replace the digits of every number, keeping its last `keep_last` digits.

    ``mask_digits("card 1234567812345678")`` -> ``"card ************5678"``. A number
    shorter than `keep_last` is left alone. This needs a **function** replacement,
    because the output depends on the matched text.
    """
    raise NotImplementedError


def increment_numbers(text: str) -> str:
    """Add one to every integer in the text, using a function replacement.

    ``"a1 b9"`` -> ``"a2 b10"``.
    """
    raise NotImplementedError


def replace_first(text: str, needle: str, replacement: str) -> str:
    """Replace only the first occurrence of the **literal** `needle`.

    `needle` is literal text, so any regex metacharacters in it must be escaped —
    otherwise ``needle="a.c"`` would also match ``"abc"``.
    """
    raise NotImplementedError


def count_and_replace(text: str, pattern: str, replacement: str) -> tuple[str, int]:
    """Return ``(new_text, number_of_replacements)`` using ``re.subn``."""
    raise NotImplementedError


def template(text: str, values: dict[str, str]) -> str:
    """Replace every ``{name}`` with ``values[name]``.

    An unknown name is left exactly as it was, braces included, rather than raising —
    that is the difference between a template engine and ``str.format``.
    """
    raise NotImplementedError


def strip_tags(html: str) -> str:
    """Remove every ``<...>`` tag, keeping the text between them.

    Non-greedy, so ``"<b>hi</b>"`` becomes ``"hi"`` rather than ``""``.
    """
    raise NotImplementedError
