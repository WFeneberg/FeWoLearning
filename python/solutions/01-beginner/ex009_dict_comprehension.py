"""Exercise 009 — Dict and set comprehensions (reference solution)."""


def lengths_by_word(words: list[str]) -> dict[str, int]:
    return {word: len(word) for word in words}


def invert(mapping: dict[str, int]) -> dict[int, str]:
    # Later items overwrite earlier ones, which is the documented last-wins rule.
    return {value: key for key, value in mapping.items()}


def filter_by_value(scores: dict[str, int], minimum: int) -> dict[str, int]:
    return {name: score for name, score in scores.items() if score >= minimum}


def zip_to_dict(keys: list[str], values: list[int]) -> dict[str, int]:
    # zip stops at the shorter input, which is exactly the "drop extras" rule.
    return dict(zip(keys, values))


def upper_keys(mapping: dict[str, int]) -> dict[str, int]:
    return {key.upper(): value for key, value in mapping.items()}


def unique_lengths(words: list[str]) -> set[int]:
    return {len(word) for word in words}


def index_of_each(values: list[str]) -> dict[str, int]:
    # A straight comprehension over enumerate() would leave the *last* index for a
    # repeated value. Walking in reverse makes the earliest index win instead.
    return {value: index for index, value in reversed(list(enumerate(values)))}
