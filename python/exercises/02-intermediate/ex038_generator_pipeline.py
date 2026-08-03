"""Exercise 038 — Generator pipelines (intermediate).

Goal:   Chain lazy stages so a large input is never held in memory at once.
Drills: generators as composable stages, laziness across a whole chain, keeping
        memory constant, feeding one stage into the next.
Passes: when `pytest exercises/02-intermediate/test_ex038_generator_pipeline.py` is green.
"""

from typing import Callable, Iterable, Iterator


def read_lines(text: str) -> Iterator[str]:
    """Yield the lines of `text` without their newline.

    A trailing newline does not produce a final empty line.
    """
    raise NotImplementedError


def strip_all(lines: Iterable[str]) -> Iterator[str]:
    """Yield each line stripped of surrounding whitespace."""
    raise NotImplementedError


def drop_blank(lines: Iterable[str]) -> Iterator[str]:
    """Yield only the lines that are not empty."""
    raise NotImplementedError


def drop_comments(lines: Iterable[str], marker: str = "#") -> Iterator[str]:
    """Yield only the lines that do not start with `marker`."""
    raise NotImplementedError


def parse_ints(lines: Iterable[str]) -> Iterator[int]:
    """Yield each line as an int.

    A line that does not parse raises ValueError — the pipeline does not hide bad
    data. Note the error only surfaces when that item is *pulled*, not when the
    generator is created.
    """
    raise NotImplementedError


def pipeline(text: str) -> Iterator[int]:
    """Compose the stages above: read, strip, drop blanks, drop comments, parse.

    The result must still be lazy — nothing is materialised into a list.
    """
    raise NotImplementedError


def chain_stages(source: Iterable[int], *stages: Callable[[Iterable[int]], Iterator[int]]) -> Iterator[int]:
    """Feed `source` through each stage in order and return the final iterator.

    With no stages, the source passes through unchanged.
    """
    raise NotImplementedError
