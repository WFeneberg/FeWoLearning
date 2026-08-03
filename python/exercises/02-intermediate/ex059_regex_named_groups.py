"""Exercise 059 — Regular expressions with named groups (intermediate).

Goal:   Extract structured data from text without counting group numbers.
Drills: re.compile, named groups and groupdict, match vs search vs fullmatch,
        finditer, optional groups yielding None, non-greedy quantifiers.
Passes: when `pytest exercises/02-intermediate/test_ex059_regex_named_groups.py` is green.
"""

from typing import Iterator


def parse_date(text: str) -> dict[str, str] | None:
    """Parse ``"2024-08-03"`` into ``{"year": ..., "month": ..., "day": ...}``.

    Returns None when the whole string is not a date — use ``fullmatch``, so
    ``"2024-08-03 extra"`` is rejected rather than partially matched.
    """
    raise NotImplementedError


def find_first_number(text: str) -> str | None:
    """Return the first run of digits anywhere in `text`, or None.

    ``search`` scans; ``match`` would only look at the very beginning.
    """
    raise NotImplementedError


def parse_log_line(line: str) -> dict[str, str | None] | None:
    """Parse ``"LEVEL message"`` or ``"LEVEL [module] message"``.

    Returns ``{"level": ..., "module": ... or None, "message": ...}``, or None when the
    line does not match. An absent optional group comes back as None, not "".
    """
    raise NotImplementedError


def all_pairs(text: str) -> list[tuple[str, str]]:
    """Return every ``key=value`` pair in `text`, in order.

    Keys are word characters; values run to the next whitespace.
    """
    raise NotImplementedError


def iter_words(text: str) -> Iterator[str]:
    """Yield each word (runs of letters) lazily, via ``finditer``.

    ``findall`` would build the whole list up front.
    """
    raise NotImplementedError


def first_tag(html: str) -> str | None:
    """Return the contents of the first ``<...>`` tag, or None.

    Must be non-greedy: for ``"<a><b>"`` the answer is ``"a"``, not ``"a><b"``.
    """
    raise NotImplementedError


def named_or_positional(text: str) -> tuple[str, str] | None:
    """Parse ``"first:second"`` and return both halves.

    Returns the same thing whether read via ``group(1), group(2)`` or by name — the
    point is that names survive a pattern edit that renumbers the groups.
    """
    raise NotImplementedError
