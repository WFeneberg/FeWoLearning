"""Exercise 038 — Generator pipelines (reference solution)."""

from typing import Callable, Iterable, Iterator


def read_lines(text: str) -> Iterator[str]:
    # yield from over splitlines(): the split itself is eager, but the caller still
    # gets a generator, and splitlines() adds no phantom trailing line.
    yield from text.splitlines()


def strip_all(lines: Iterable[str]) -> Iterator[str]:
    for line in lines:
        yield line.strip()


def drop_blank(lines: Iterable[str]) -> Iterator[str]:
    for line in lines:
        if line:
            yield line


def drop_comments(lines: Iterable[str], marker: str = "#") -> Iterator[str]:
    for line in lines:
        if not line.startswith(marker):
            yield line


def parse_ints(lines: Iterable[str]) -> Iterator[int]:
    for line in lines:
        # int() raises on its own; because this is a generator, the error appears
        # when the offending item is pulled, not when the generator is built.
        yield int(line)


def pipeline(text: str) -> Iterator[int]:
    # Each stage wraps the previous one, so one item flows through the whole chain
    # at a time and memory stays constant regardless of input size.
    return parse_ints(drop_comments(drop_blank(strip_all(read_lines(text)))))


def chain_stages(
    source: Iterable[int], *stages: Callable[[Iterable[int]], Iterator[int]]
) -> Iterator[int]:
    current: Iterable[int] = source
    for stage in stages:
        current = stage(current)
    return iter(current)
