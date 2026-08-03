"""Exercise 007 — Dict lookup with defaults (reference solution)."""


def lookup(scores: dict[str, int], name: str, default: int = 0) -> int:
    return scores.get(name, default)


def require(scores: dict[str, int], name: str) -> int:
    # Subscripting is the point here: it raises KeyError on its own.
    return scores[name]


def add_to_group(groups: dict[str, list[str]], key: str, member: str) -> dict[str, list[str]]:
    # setdefault inserts the empty list and returns it in one step, so the append
    # lands in the dict without a separate membership check.
    groups.setdefault(key, []).append(member)
    return groups


def increment(counts: dict[str, int], key: str, by: int = 1) -> dict[str, int]:
    counts[key] = counts.get(key, 0) + by
    return counts


def take(settings: dict[str, str], key: str, default: str = "") -> tuple[str, dict[str, str]]:
    # A second argument turns pop from raising into defaulting.
    return settings.pop(key, default), settings


def nested_get(data: dict[str, object], path: list[str], default: object = None) -> object:
    current: object = data
    for step in path:
        if not isinstance(current, dict) or step not in current:
            return default
        current = current[step]
    return current
