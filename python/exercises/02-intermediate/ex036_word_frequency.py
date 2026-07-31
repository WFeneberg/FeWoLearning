"""Exercise 036 — Word frequency (intermediate).

Goal:   Return the top-n most common words in a text, case-insensitive,
        ignoring punctuation, sorted by count desc then word asc.
Drills: collections.Counter, str normalization, sorting with tie-breakers.
"""
from collections import Counter  # noqa: F401  (available for your solution)


def top_words(text: str, n: int) -> list[tuple[str, int]]:
    """Return the ``n`` most frequent words as ``(word, count)`` pairs.

    Words are lowercased and stripped of surrounding punctuation. Ties are
    broken alphabetically by the word.
    """
    raise NotImplementedError
