"""Exercise 060 — Regular expression substitution (reference solution)."""

import re
from typing import Callable

_WHITESPACE = re.compile(r"\s+")
_NAME = re.compile(r"(\w+), (\w+)")
_NUMBER = re.compile(r"\d+")
_PLACEHOLDER = re.compile(r"\{(\w+)\}")
_TAG = re.compile(r"<.+?>")


def collapse_spaces(text: str) -> str:
    return _WHITESPACE.sub(" ", text)


def swap_names(text: str) -> str:
    # \2 and \1 are backreferences to the capture groups, in the replacement.
    return _NAME.sub(r"\2 \1", text)


def mask_digits(text: str, keep_last: int = 4) -> str:
    def replace(match: re.Match[str]) -> str:
        digits = match.group()
        if len(digits) <= keep_last:
            return digits
        # A template string cannot compute this, so the replacement is a function.
        return "*" * (len(digits) - keep_last) + digits[-keep_last:]

    return _NUMBER.sub(replace, text)


def increment_numbers(text: str) -> str:
    return _NUMBER.sub(lambda m: str(int(m.group()) + 1), text)


def replace_first(text: str, needle: str, replacement: str) -> str:
    # re.escape neutralises metacharacters in the needle, so "a.c" cannot match
    # "abc". The replacement is escaped too, so a literal backslash survives.
    return re.sub(re.escape(needle), replacement.replace("\\", "\\\\"), text, count=1)


def count_and_replace(text: str, pattern: str, replacement: str) -> tuple[str, int]:
    return re.subn(pattern, replacement, text)


def template(text: str, values: dict[str, str]) -> str:
    # Returning the whole match leaves an unknown placeholder untouched, where
    # str.format would raise KeyError.
    return _PLACEHOLDER.sub(lambda m: values.get(m.group(1), m.group()), text)


def strip_tags(html: str) -> str:
    return _TAG.sub("", html)
