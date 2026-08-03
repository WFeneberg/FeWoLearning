"""Exercise 059 — Regular expressions with named groups (reference solution)."""

import re
from typing import Iterator

# Compiling once at import beats recompiling on every call. re does cache patterns,
# but naming them also documents what each one is for.
_DATE = re.compile(r"(?P<year>\d{4})-(?P<month>\d{2})-(?P<day>\d{2})")
_NUMBER = re.compile(r"\d+")
_LOG = re.compile(r"^(?P<level>[A-Z]+)(?: \[(?P<module>\w+)\])? (?P<message>.+)$")
_PAIR = re.compile(r"(\w+)=(\S+)")
_WORD = re.compile(r"[A-Za-z]+")
_TAG = re.compile(r"<(.+?)>")
_HALVES = re.compile(r"^(?P<first>[^:]+):(?P<second>[^:]+)$")


def parse_date(text: str) -> dict[str, str] | None:
    # fullmatch anchors both ends, so trailing text is rejected outright.
    match = _DATE.fullmatch(text)
    return None if match is None else match.groupdict()


def find_first_number(text: str) -> str | None:
    # search scans the whole string; match would only try position 0.
    match = _NUMBER.search(text)
    return None if match is None else match.group()


def parse_log_line(line: str) -> dict[str, str | None] | None:
    match = _LOG.fullmatch(line)
    if match is None:
        return None
    # An optional group that did not participate is None, never "".
    return dict(match.groupdict())


def all_pairs(text: str) -> list[tuple[str, str]]:
    return _PAIR.findall(text)


def iter_words(text: str) -> Iterator[str]:
    # finditer stays lazy; findall would materialise everything first.
    for match in _WORD.finditer(text):
        yield match.group()


def first_tag(html: str) -> str | None:
    # `+?` is non-greedy, so it stops at the first ">" instead of the last.
    match = _TAG.search(html)
    return None if match is None else match.group(1)


def named_or_positional(text: str) -> tuple[str, str] | None:
    match = _HALVES.fullmatch(text)
    if match is None:
        return None
    # Reading by name survives a pattern edit that inserts another group and
    # renumbers everything after it.
    return match.group("first"), match.group("second")
