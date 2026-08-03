"""Exercise 066 — ChainMap (intermediate).

Goal:   Layer several mappings so lookups fall through, without copying them.
Drills: ChainMap search order, new_child/parents, writes going to the first map only,
        maps as a live list, and why ChainMap beats ``{**a, **b}`` when the sources
        keep changing.
Passes: when `pytest exercises/02-intermediate/test_ex066_chainmap_layers.py` is green.
"""

from collections import ChainMap
from typing import Any


def layered_config(cli: dict[str, Any], env: dict[str, Any], defaults: dict[str, Any]) -> ChainMap:
    """Build a config where `cli` wins, then `env`, then `defaults`.

    A ChainMap searches its maps left to right, so the highest-priority layer goes
    first.
    """
    raise NotImplementedError


def resolve(config: ChainMap, key: str, fallback: Any = None) -> Any:
    """Return the effective value for `key`, or `fallback` when no layer has it."""
    raise NotImplementedError


def which_layer(config: ChainMap, key: str) -> int:
    """Return the index of the first layer containing `key`, or -1 when none does."""
    raise NotImplementedError


def stays_live(base: dict[str, Any]) -> tuple[ChainMap, Any]:
    """Return ``(chainmap_over_base, value_before_mutation)``.

    Build a ChainMap over `base`, read key "a", then set ``base["a"] = "changed"``.
    The ChainMap must now see the new value — it holds references, not copies, which is
    exactly what ``{**base}`` would not give you.
    """
    raise NotImplementedError


def with_overrides(config: ChainMap, **overrides: Any) -> ChainMap:
    """Return a new ChainMap with `overrides` as an additional front layer.

    ``new_child`` does this without touching the original.
    """
    raise NotImplementedError


def write_goes_to_the_front(config: ChainMap, key: str, value: Any) -> tuple[Any, dict[str, Any]]:
    """Set ``config[key] = value`` and return ``(effective_value, first_layer)``.

    Writes only ever reach the **first** mapping; the deeper layers are read-only as
    far as a ChainMap is concerned.
    """
    raise NotImplementedError


def flatten(config: ChainMap) -> dict[str, Any]:
    """Collapse the layers into one plain dict with the effective values."""
    raise NotImplementedError


def drop_front(config: ChainMap) -> ChainMap:
    """Return the same chain without its first layer.

    ``parents`` is the built-in way. A single-layer chain yields an empty chain, not
    an error.
    """
    raise NotImplementedError
