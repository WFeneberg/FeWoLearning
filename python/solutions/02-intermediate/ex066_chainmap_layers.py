"""Exercise 066 — ChainMap (reference solution)."""

from collections import ChainMap
from typing import Any


def layered_config(
    cli: dict[str, Any], env: dict[str, Any], defaults: dict[str, Any]
) -> ChainMap:
    # Left to right is highest to lowest priority.
    return ChainMap(cli, env, defaults)


def resolve(config: ChainMap, key: str, fallback: Any = None) -> Any:
    return config.get(key, fallback)


def which_layer(config: ChainMap, key: str) -> int:
    for index, layer in enumerate(config.maps):
        if key in layer:
            return index
    return -1


def stays_live(base: dict[str, Any]) -> tuple[ChainMap, Any]:
    config = ChainMap(base)
    before = config["a"]
    # The ChainMap references `base` rather than copying it, so this is visible
    # through the chain immediately.
    base["a"] = "changed"
    return config, before


def with_overrides(config: ChainMap, **overrides: Any) -> ChainMap:
    # new_child prepends a layer and returns a new ChainMap; the original's `maps`
    # list is not modified.
    return config.new_child(overrides)


def write_goes_to_the_front(
    config: ChainMap, key: str, value: Any
) -> tuple[Any, dict[str, Any]]:
    # Every write lands in maps[0]; the deeper layers are read-only through a ChainMap.
    config[key] = value
    return config[key], config.maps[0]


def flatten(config: ChainMap) -> dict[str, Any]:
    # Iterating a ChainMap already yields the effective view, so dict() collapses it.
    return dict(config)


def drop_front(config: ChainMap) -> ChainMap:
    # parents is ChainMap(*maps[1:]) — and for a single-layer chain that is an empty
    # chain rather than an error.
    return config.parents
