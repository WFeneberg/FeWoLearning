"""Exercise 031 — Nested data access (reference solution)."""

from typing import Any


def get_path(data: Any, path: str, default: Any = None) -> Any:
    if not path:
        return data
    current = data
    for step in path.split("."):
        if isinstance(current, dict):
            if step not in current:
                return default
            current = current[step]
        elif isinstance(current, list):
            # A list step has to be an index, and it has to be in range.
            if not step.isdigit():
                return default
            index = int(step)
            if index >= len(current):
                return default
            current = current[index]
        else:
            # Neither a dict nor a list: there is nowhere left to descend.
            return default
    return current


def set_path(data: dict[str, Any], path: str, value: Any) -> dict[str, Any]:
    if not path:
        raise ValueError("set_path() needs a non-empty path")
    *parents, leaf = path.split(".")
    current = data
    for step in parents:
        existing = current.get(step)
        if not isinstance(existing, dict):
            # Create (or replace a non-dict) so the walk can continue.
            existing = {}
            current[step] = existing
        current = existing
    current[leaf] = value
    return data


def flatten_keys(data: dict[str, Any], separator: str = ".") -> dict[str, Any]:
    result: dict[str, Any] = {}
    for key, value in data.items():
        if isinstance(value, dict):
            # Recursing rather than special-casing depth keeps this short; an empty
            # nested dict yields nothing, so it disappears.
            for inner_key, inner_value in flatten_keys(value, separator).items():
                result[f"{key}{separator}{inner_key}"] = inner_value
        else:
            result[key] = value
    return result


def collect_values(data: Any, key: str) -> list[Any]:
    found: list[Any] = []
    if isinstance(data, dict):
        for current_key, value in data.items():
            if current_key == key:
                found.append(value)
            found.extend(collect_values(value, key))
    elif isinstance(data, list):
        for item in data:
            found.extend(collect_values(item, key))
    return found


def deep_merge(base: dict[str, Any], override: dict[str, Any]) -> dict[str, Any]:
    result = dict(base)
    for key, value in override.items():
        existing = result.get(key)
        if isinstance(existing, dict) and isinstance(value, dict):
            result[key] = deep_merge(existing, value)
        else:
            result[key] = value
    return result


def count_leaves(data: Any) -> int:
    if isinstance(data, dict):
        return sum(count_leaves(value) for value in data.values())
    if isinstance(data, list):
        return sum(count_leaves(item) for item in data)
    return 1
