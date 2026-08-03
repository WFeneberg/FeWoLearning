"""Exercise 025 — Sorting with a key (reference solution)."""


def by_length(words: list[str]) -> list[str]:
    # sorted() returns a new list; the sort is stable, so equal lengths keep order.
    return sorted(words, key=len)


def by_length_desc(words: list[str]) -> list[str]:
    return sorted(words, key=len, reverse=True)


def case_insensitive(words: list[str]) -> list[str]:
    # casefold() is the aggressive sibling of lower(), meant for comparisons.
    return sorted(words, key=str.casefold)


def by_absolute_value(numbers: list[int]) -> list[int]:
    return sorted(numbers, key=abs)


def by_last_name(names: list[str]) -> list[str]:
    return sorted(names, key=lambda name: name.split()[-1])


def sort_in_place(numbers: list[int]) -> None:
    numbers.sort()


def top_scores(scores: dict[str, int], n: int) -> list[str]:
    if n <= 0:
        return []
    # A tuple key sorts by score descending, then by name ascending — negating the
    # score avoids needing reverse=True, which would also reverse the names.
    ranked = sorted(scores.items(), key=lambda item: (-item[1], item[0]))
    return [name for name, _ in ranked[:n]]
