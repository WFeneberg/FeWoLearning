"""Exercise 036 — Word frequency (reference solution)."""
import re
from collections import Counter


def top_words(text: str, n: int) -> list[tuple[str, int]]:
    words = re.findall(r"[a-z0-9']+", text.lower())
    counts = Counter(words)
    # sort by count desc, then word asc
    ordered = sorted(counts.items(), key=lambda kv: (-kv[1], kv[0]))
    return ordered[:n]
