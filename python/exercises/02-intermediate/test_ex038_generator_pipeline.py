from typing import Iterable, Iterator

import pytest

from ex038_generator_pipeline import (
    chain_stages,
    drop_blank,
    drop_comments,
    parse_ints,
    pipeline,
    read_lines,
    strip_all,
)

SAMPLE = """
# a comment
  1
2

# another
   3
"""


def test_read_lines() -> None:
    assert list(read_lines("a\nb\nc")) == ["a", "b", "c"]


def test_read_lines_ignores_a_trailing_newline() -> None:
    assert list(read_lines("a\nb\n")) == ["a", "b"]


def test_read_lines_keeps_inner_blank_lines() -> None:
    assert list(read_lines("a\n\nb")) == ["a", "", "b"]


def test_read_lines_empty() -> None:
    assert list(read_lines("")) == []


def test_strip_all() -> None:
    assert list(strip_all(["  a  ", "b\t"])) == ["a", "b"]


def test_strip_all_turns_whitespace_into_empty() -> None:
    assert list(strip_all(["   "])) == [""]


def test_drop_blank() -> None:
    assert list(drop_blank(["a", "", "b"])) == ["a", "b"]


def test_drop_blank_everything() -> None:
    assert list(drop_blank(["", ""])) == []


def test_drop_comments() -> None:
    assert list(drop_comments(["# x", "a", "#", "b"])) == ["a", "b"]


def test_drop_comments_custom_marker() -> None:
    assert list(drop_comments(["// x", "a"], "//")) == ["a"]


def test_parse_ints() -> None:
    assert list(parse_ints(["1", "2", "-3"])) == [1, 2, -3]


def test_parse_ints_raises_only_when_pulled() -> None:
    result = parse_ints(["1", "nope"])

    # Creating the generator runs no body at all.
    assert next(result) == 1
    with pytest.raises(ValueError):
        next(result)


def test_pipeline_end_to_end() -> None:
    assert list(pipeline(SAMPLE)) == [1, 2, 3]


def test_pipeline_is_lazy() -> None:
    result = pipeline(SAMPLE)

    assert iter(result) is result
    assert next(result) == 1


def test_pipeline_on_empty_input() -> None:
    assert list(pipeline("")) == []


def test_pipeline_on_comments_only() -> None:
    assert list(pipeline("# a\n# b\n")) == []


def test_chain_stages_with_no_stages() -> None:
    assert list(chain_stages(iter([1, 2, 3]))) == [1, 2, 3]


def test_chain_stages_applies_in_order() -> None:
    def double(values: Iterable[int]) -> Iterator[int]:
        return (v * 2 for v in values)

    def add_one(values: Iterable[int]) -> Iterator[int]:
        return (v + 1 for v in values)

    # double then add_one: (1*2)+1 == 3
    assert list(chain_stages(iter([1, 2]), double, add_one)) == [3, 5]
    # add_one then double: (1+1)*2 == 4
    assert list(chain_stages(iter([1, 2]), add_one, double)) == [4, 6]


def test_chain_stages_stays_lazy() -> None:
    pulled: list[int] = []

    def spy(values: Iterable[int]) -> Iterator[int]:
        for value in values:
            pulled.append(value)
            yield value

    result = chain_stages(iter([1, 2, 3]), spy)

    assert next(result) == 1
    # Only the first item was consumed, not all three.
    assert pulled == [1]
