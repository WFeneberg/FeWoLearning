"""Exercise 063 — argparse (intermediate).

Goal:   Define a command-line interface declaratively and parse into typed values.
Drills: ArgumentParser, positional vs optional arguments, type= and choices=,
        store_true, nargs, subcommands, and testing a parser without touching sys.argv.
Passes: when `pytest exercises/02-intermediate/test_ex063_argparse_cli.py` is green.

Note:   every function returns the parser or the parsed result, so tests can pass an
        explicit argument list. Never read sys.argv here — that would make the CLI
        untestable, which is the real lesson.
"""

import argparse
from typing import Any


def build_basic_parser() -> argparse.ArgumentParser:
    """A parser with prog="tool" and:

    - a required positional `path`
    - ``--verbose`` / ``-v`` as a flag defaulting to False
    - ``--count`` / ``-n`` as an int defaulting to 1
    """
    raise NotImplementedError


def build_choices_parser() -> argparse.ArgumentParser:
    """A parser with prog="tool" and ``--format`` restricted to json/csv/text.

    Defaults to "text". An invalid choice must make argparse exit rather than return —
    that is a SystemExit, not an exception you catch.
    """
    raise NotImplementedError


def build_nargs_parser() -> argparse.ArgumentParser:
    """A parser with prog="tool" taking one or more positional `files`.

    ``nargs="+"`` means at least one; passing none is a usage error.
    """
    raise NotImplementedError


def build_subcommand_parser() -> argparse.ArgumentParser:
    """A parser with prog="tool" and two subcommands, stored in `command`:

    - ``add`` with a positional int `value`
    - ``list`` with an optional ``--all`` flag

    A missing subcommand leaves `command` as None rather than failing.
    """
    raise NotImplementedError


def parse_basic(argv: list[str]) -> dict[str, Any]:
    """Parse with the basic parser and return the result as a plain dict.

    ``vars(namespace)`` is the usual conversion.
    """
    raise NotImplementedError


def run_subcommand(argv: list[str]) -> str:
    """Dispatch on the parsed subcommand and describe what would happen.

    - ``add 5`` -> ``"add:5"``
    - ``list`` -> ``"list:some"``
    - ``list --all`` -> ``"list:all"``
    - no subcommand -> ``"none"``
    """
    raise NotImplementedError
