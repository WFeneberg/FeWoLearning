"""Exercise 061 — JSON round-tripping (reference solution)."""

import json
from datetime import date
from decimal import Decimal
from typing import Any


def to_json(value: Any) -> str:
    # separators drops the default space after ":" and ","; sort_keys makes the
    # output stable enough to compare.
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def to_pretty_json(value: Any) -> str:
    return json.dumps(value, sort_keys=True, indent=2)


def from_json(text: str) -> Any:
    # JSONDecodeError already subclasses ValueError, so nothing needs wrapping.
    return json.loads(text)


def _encode_extra(value: Any) -> Any:
    # default= is called only for values json cannot handle itself.
    if isinstance(value, date):
        return value.isoformat()
    if isinstance(value, Decimal):
        # A string, not a float: float(Decimal("0.1")) would lose the precision the
        # Decimal existed to keep.
        return str(value)
    if isinstance(value, (set, frozenset)):
        # Sorted, so the output does not depend on set iteration order.
        return sorted(value)
    # Raising TypeError is what json expects from an unhandled value.
    raise TypeError(f"Object of type {type(value).__name__} is not JSON serializable")


def to_json_extended(value: Any) -> str:
    return json.dumps(value, sort_keys=True, separators=(",", ":"), default=_encode_extra)


def parse_with_dates(text: str, date_keys: set[str]) -> Any:
    def hook(obj: dict[str, Any]) -> dict[str, Any]:
        # Runs for every JSON object as it is constructed, innermost first, so
        # nesting needs no extra handling.
        return {
            key: date.fromisoformat(value) if key in date_keys and isinstance(value, str) else value
            for key, value in obj.items()
        }

    return json.loads(text, object_hook=hook)


def round_trip(value: Any) -> Any:
    return json.loads(json.dumps(value))


def is_json_safe(value: Any) -> bool:
    try:
        json.dumps(value)
    except TypeError:
        return False
    return True
