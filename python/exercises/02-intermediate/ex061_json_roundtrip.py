"""Exercise 061 — JSON round-tripping (intermediate).

Goal:   Serialise and parse JSON, including types json does not know.
Drills: dumps/loads, sort_keys and indent, default= for unsupported types,
        object_hook for parsing, and the asymmetries JSON cannot round-trip.
Passes: when `pytest exercises/02-intermediate/test_ex061_json_roundtrip.py` is green.
"""

from datetime import date
from decimal import Decimal
from typing import Any


def to_json(value: Any) -> str:
    """Serialise with sorted keys and no superfluous whitespace.

    Sorted keys make the output stable enough to compare in a test; the default
    separators leave a space after ": " and ", " which is dropped here.
    """
    raise NotImplementedError


def to_pretty_json(value: Any) -> str:
    """Serialise with sorted keys and two-space indentation."""
    raise NotImplementedError


def from_json(text: str) -> Any:
    """Parse JSON text.

    Invalid JSON raises ValueError — ``json.JSONDecodeError`` already is one, so
    nothing extra is needed.
    """
    raise NotImplementedError


def to_json_extended(value: Any) -> str:
    """Serialise, additionally supporting `date`, `Decimal` and `set`.

    json refuses these outright; a ``default=`` hook is called for exactly the values
    it cannot handle. Encode a date as its ISO string, a Decimal as a string (not a
    float — that would lose precision), and a set as a **sorted** list so the output
    is deterministic. Anything else still raises TypeError.
    """
    raise NotImplementedError


def parse_with_dates(text: str, date_keys: set[str]) -> Any:
    """Parse JSON, converting the listed keys from ISO strings to `date` objects.

    Uses ``object_hook``, which runs for every JSON object as it is built.
    """
    raise NotImplementedError


def round_trip(value: Any) -> Any:
    """Serialise then parse, returning the result.

    Useful for seeing what JSON *cannot* preserve: tuples come back as lists, and
    non-string dict keys come back as strings.
    """
    raise NotImplementedError


def is_json_safe(value: Any) -> bool:
    """Report whether `value` can be serialised without a `default=` hook."""
    raise NotImplementedError
