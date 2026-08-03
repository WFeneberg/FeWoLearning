import argparse

import pytest

from ex063_argparse_cli import (
    build_basic_parser,
    build_choices_parser,
    build_nargs_parser,
    build_subcommand_parser,
    parse_basic,
    run_subcommand,
)


def test_basic_parser_defaults() -> None:
    args = build_basic_parser().parse_args(["file.txt"])

    assert args.path == "file.txt"
    assert args.verbose is False
    assert args.count == 1


def test_basic_parser_long_options() -> None:
    args = build_basic_parser().parse_args(["f", "--verbose", "--count", "3"])

    assert args.verbose is True
    assert args.count == 3


def test_basic_parser_short_options() -> None:
    args = build_basic_parser().parse_args(["f", "-v", "-n", "7"])

    assert args.verbose is True
    assert args.count == 7


def test_basic_parser_count_is_an_int_not_a_string() -> None:
    args = build_basic_parser().parse_args(["f", "-n", "5"])

    assert args.count == 5
    assert isinstance(args.count, int)


def test_basic_parser_requires_the_positional() -> None:
    with pytest.raises(SystemExit):
        build_basic_parser().parse_args([])


def test_basic_parser_rejects_a_non_numeric_count() -> None:
    with pytest.raises(SystemExit):
        build_basic_parser().parse_args(["f", "-n", "abc"])


def test_choices_parser_default() -> None:
    assert build_choices_parser().parse_args([]).format == "text"


@pytest.mark.parametrize("value", ["json", "csv", "text"])
def test_choices_parser_accepts_valid_choices(value: str) -> None:
    assert build_choices_parser().parse_args(["--format", value]).format == value


def test_choices_parser_rejects_an_invalid_choice() -> None:
    with pytest.raises(SystemExit):
        build_choices_parser().parse_args(["--format", "xml"])


def test_nargs_parser_one_file() -> None:
    assert build_nargs_parser().parse_args(["a.txt"]).files == ["a.txt"]


def test_nargs_parser_several_files() -> None:
    assert build_nargs_parser().parse_args(["a", "b", "c"]).files == ["a", "b", "c"]


def test_nargs_parser_requires_at_least_one() -> None:
    with pytest.raises(SystemExit):
        build_nargs_parser().parse_args([])


def test_subcommand_parser_add() -> None:
    args = build_subcommand_parser().parse_args(["add", "5"])

    assert args.command == "add"
    assert args.value == 5


def test_subcommand_parser_list() -> None:
    args = build_subcommand_parser().parse_args(["list"])

    assert args.command == "list"
    assert args.all is False


def test_subcommand_parser_list_all() -> None:
    assert build_subcommand_parser().parse_args(["list", "--all"]).all is True


def test_subcommand_parser_without_a_subcommand() -> None:
    assert build_subcommand_parser().parse_args([]).command is None


def test_subcommand_parser_rejects_an_unknown_subcommand() -> None:
    with pytest.raises(SystemExit):
        build_subcommand_parser().parse_args(["nope"])


def test_parse_basic_returns_a_dict() -> None:
    result = parse_basic(["f", "-n", "2"])

    assert result == {"path": "f", "verbose": False, "count": 2}
    assert isinstance(result, dict)


@pytest.mark.parametrize(
    "argv, expected",
    [
        (["add", "5"], "add:5"),
        (["list"], "list:some"),
        (["list", "--all"], "list:all"),
        ([], "none"),
    ],
)
def test_run_subcommand(argv: list[str], expected: str) -> None:
    assert run_subcommand(argv) == expected


def test_parsers_are_argument_parsers() -> None:
    for factory in (
        build_basic_parser,
        build_choices_parser,
        build_nargs_parser,
        build_subcommand_parser,
    ):
        assert isinstance(factory(), argparse.ArgumentParser)
