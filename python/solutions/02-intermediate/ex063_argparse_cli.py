"""Exercise 063 — argparse (reference solution)."""

import argparse
from typing import Any


def build_basic_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="tool")
    parser.add_argument("path")
    # store_true gives a flag that defaults to False without a separate default=.
    parser.add_argument("-v", "--verbose", action="store_true")
    # type=int makes argparse convert *and* reject non-numeric input for us.
    parser.add_argument("-n", "--count", type=int, default=1)
    return parser


def build_choices_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="tool")
    parser.add_argument("--format", choices=["json", "csv", "text"], default="text")
    return parser


def build_nargs_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="tool")
    # "+" means one or more; "*" would have allowed none.
    parser.add_argument("files", nargs="+")
    return parser


def build_subcommand_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="tool")
    # dest names the attribute holding the chosen subcommand; without required=True
    # omitting it simply leaves that attribute None.
    subparsers = parser.add_subparsers(dest="command")

    add = subparsers.add_parser("add")
    add.add_argument("value", type=int)

    listing = subparsers.add_parser("list")
    listing.add_argument("--all", action="store_true")

    return parser


def parse_basic(argv: list[str]) -> dict[str, Any]:
    # Passing argv explicitly rather than letting argparse read sys.argv is what
    # makes the parser testable.
    return vars(build_basic_parser().parse_args(argv))


def run_subcommand(argv: list[str]) -> str:
    args = build_subcommand_parser().parse_args(argv)
    if args.command == "add":
        return f"add:{args.value}"
    if args.command == "list":
        return "list:all" if args.all else "list:some"
    return "none"
